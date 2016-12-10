// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using System.Reflection;

namespace KSPDev.ConfigUtils {

/// <summary>Descriptor of a persitent field.</summary>
sealed class PersistentField {
  /// <summary>Annotated fields metadata.</summary>
  public readonly FieldInfo fieldInfo;
  /// <summary>Parsed configuration paths.</summary>
  public readonly string[] cfgPath;

  /// <summary>Instance of ordianl field proto as specified in the annotation.</summary>
  internal readonly OrdinaryFieldHandler ordinaryFieldHandler;
  /// <summary>Instance of collection field proto as specified in the annotation.</summary>
  internal readonly CollectionFieldHandler collectionFieldHandler;

  readonly PersistentField[] compoundTypeFields;

  /// <param name="fieldInfo">An annotated field metadata.</param>
  /// <param name="fieldAttr">An annotation of the field.</param>
  public PersistentField(FieldInfo fieldInfo, PersistentFieldAttribute fieldAttr) {
    this.fieldInfo = fieldInfo;
    cfgPath = fieldAttr.path;
    var ordinaryType = fieldInfo.FieldType;

    if (fieldAttr.collectionTypeProto != null) {
      collectionFieldHandler =
          new CollectionFieldHandler(this, fieldInfo.FieldType, fieldAttr.collectionTypeProto);
      ordinaryType = collectionFieldHandler.GetItemType();
    }

    ordinaryFieldHandler =
        new OrdinaryFieldHandler(this, ordinaryType, fieldAttr.ordinaryTypeProto);

    if (ordinaryFieldHandler.IsCompound()) {
      // Ignore static fields of the compound type since it can be used by multiple persistent
      // fields or as an item in a collection field.
      // Also, ignore groups in the compound types to reduce configuration complexity.
      compoundTypeFields =
          PersistentFieldsFactory.GetPersistentFields(
              ordinaryType, false /* needStatic */, true /* needInstance */, null /* group */)
          // Parent nodes have to be handled before children!
          .OrderBy(x => string.Join("/", x.cfgPath))
          .ToArray();
    }
  }

  /// <summary>Writes field into a config node.</summary>
  /// <param name="node">A node to write state to.</param>
  /// <param name="instance">An owner of the field. Can be <c>null</c> for static fields.</param>
  public void WriteToConfig(ConfigNode node, object instance) {
    var value = fieldInfo.GetValue(instance);
    if (value == null) {
      return;
    }
    if (collectionFieldHandler != null) {
      collectionFieldHandler.SerializeValues(node, value);
    } else {
      var cfgData = ordinaryFieldHandler.SerializeValue(value);
      if (cfgData != null) {
        if (ordinaryFieldHandler.IsCompound()) {
          ConfigAccessor.SetNodeByPath(node, cfgPath, (ConfigNode) cfgData);
        } else {
          ConfigAccessor.SetValueByPath(node, cfgPath, (string) cfgData);
        }
      }
    }
  }

  /// <summary>Reads field from a config node.</summary>
  /// <param name="node">A node to read state from.</param>
  /// <param name="instance">An owner of the field. Can be <c>null</c> for static fields.</param>
  public void ReadFromConfig(ConfigNode node, object instance) {
    object value = null;
    if (collectionFieldHandler != null) {
      value = collectionFieldHandler.DeserializeValues(node);
    } else {
      var cfgData = ordinaryFieldHandler.IsCompound()
          ? ConfigAccessor.GetNodeByPath(node, cfgPath) as object
          : ConfigAccessor.GetValueByPath(node, cfgPath) as object;
      if (cfgData != null) {
        value = ordinaryFieldHandler.DeserializeValue(cfgData);
      }
    }
    if (value != null) {
      fieldInfo.SetValue(instance, value);
    }
  }
  
  /// <summary>Makes a config node from the compound type fields.</summary>
  /// <param name="instance">An owner ofthe fields. Can be <c>null</c> for static fields.</param>
  /// <returns>New configuration node with the data.</returns>
  public ConfigNode SerializeCompoundFieldsToNode(object instance) {
    ConfigNode node = null;
    if (compoundTypeFields.Length > 0) {
      node = new ConfigNode();
      foreach (var compoundTypeField in compoundTypeFields) {
        compoundTypeField.WriteToConfig(node, instance);
      }
    }
    return node;
  }
  
  /// <summary>Sets compound type field values from the config node.</summary>
  /// <param name="node">A node to read values from.</param>
  /// <param name="instance">An owner ofthe fields. Can be <c>null</c> for static fields.</param>
  internal void DeserializeCompoundFieldsFromNode(ConfigNode node, object instance) {
    foreach (var compoundTypeField in compoundTypeFields) {
      compoundTypeField.ReadFromConfig(node, instance);
    }
  }
}

}  // namespace
