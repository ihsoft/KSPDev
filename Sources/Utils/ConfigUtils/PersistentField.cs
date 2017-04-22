// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KSPDev.ConfigUtils {

/// <summary>Descriptor of a persitent field.</summary>
sealed class PersistentField {
  /// <summary>Annotated fields metadata.</summary>
  public readonly FieldInfo fieldInfo;
  /// <summary>Parsed configuration paths.</summary>
  public readonly string[] cfgPath;

  readonly AbstractOrdinaryValueTypeProto simpleTypeProto;
  readonly AbstractCollectionTypeProto collectionProto;
  readonly bool isCompound;
  readonly bool isCustomSimpleType;
  readonly PersistentField[] compoundTypeFields;
  readonly bool isDisabled;

  /// <param name="fieldInfo">An annotated field metadata.</param>
  /// <param name="fieldAttr">An annotation of the field.</param>
  public PersistentField(FieldInfo fieldInfo, PersistentFieldAttribute fieldAttr) {
    this.fieldInfo = fieldInfo;
    cfgPath = fieldAttr.path;
    var ordinaryType = fieldInfo.FieldType;

    if (fieldAttr.collectionTypeProto != null) {
      collectionProto =
          Activator.CreateInstance(fieldAttr.collectionTypeProto, new[] {fieldInfo.FieldType})
          as AbstractCollectionTypeProto;
      ordinaryType = collectionProto.GetItemType();
    }
    simpleTypeProto =
        Activator.CreateInstance(fieldAttr.ordinaryTypeProto)
        as AbstractOrdinaryValueTypeProto;
    isCustomSimpleType = typeof(IPersistentField).IsAssignableFrom(ordinaryType);

    // Determine if field's or collection item's type is a class (compound type).
    isCompound = !simpleTypeProto.CanHandle(ordinaryType) && !isCustomSimpleType
        && (ordinaryType.IsValueType && !ordinaryType.IsEnum  // IsStruct
            || ordinaryType.IsClass && ordinaryType != typeof(string)
               && !ordinaryType.IsEnum);  // IsClass

    if (!isCompound && !simpleTypeProto.CanHandle(ordinaryType) && !isCustomSimpleType) {
      Debug.LogErrorFormat(
          "Proto {0} cannot handle value in field {1}.{2}",
          ordinaryType.FullName, fieldInfo.DeclaringType.FullName, fieldInfo.Name);
      isDisabled = true;
    }

    // For a compound type retrieve all its persisted fields. 
    if (isCompound && !isDisabled) {
      // Ignore static fields of the compound type since it can be used by multiple persistent
      // fields or as an item in a collection field.
      // Also, ignore groups in the compound types to reduce configuration complexity.
      compoundTypeFields =
          PersistentFieldsFactory.GetPersistentFields(
              ordinaryType, false /* needStatic */, true /* needInstance */, null /* group */)
          // Parent nodes have to be handled before children!
          .OrderBy(x => string.Join("/", x.cfgPath))
          .ToArray();

      // Disable if cannot deal with the field anyways.
      if (compoundTypeFields.Length == 0 && !typeof(IConfigNode).IsAssignableFrom(ordinaryType)) {
        Debug.LogErrorFormat(
            "Compound type in field {0}.{1} has no persistent fields and doesn't implement"
            + " IConfigNode",
            fieldInfo.DeclaringType.FullName, fieldInfo.Name);
        isDisabled = true;
      }
    }
  }

  /// <summary>Writes field into a config node.</summary>
  /// <remarks>
  /// This method is not expected to fail since converting any type into string is expected to
  /// succeeed on any value.
  /// </remarks>
  /// <param name="node">A node to write state to.</param>
  /// <param name="instance">An owner of the field. Can be <c>null</c> for static fields.</param>
  public void WriteToConfig(ConfigNode node, object instance) {
    if (isDisabled) {
      return;  // Field is not supported.
    }
    var value = fieldInfo.GetValue(instance);
    if (value == null) {
      Debug.LogWarningFormat("Skip writing field {0}.{1} due to its value is NULL",
                             fieldInfo.DeclaringType.FullName, fieldInfo.Name);
      return;
    }
    if (collectionProto != null) {
      // For collections iterative via proto class and serialize item values.
      foreach (var itemValue in collectionProto.GetEnumerator(value)) {
        if (itemValue != null) {
          if (isCompound) {
            ConfigAccessor.AddNodeByPath(node, cfgPath, SerializeCompoundFieldsToNode(itemValue));
          } else if (isCustomSimpleType) {
            ConfigAccessor.AddValueByPath(
                node, cfgPath, ((IPersistentField)itemValue).SerializeToString());
          } else {
            ConfigAccessor.AddValueByPath(
                node, cfgPath, simpleTypeProto.SerializeToString(itemValue));
          }
        }
      }
    } else {
      // For ordinal values just serialize the value.
      if (isCompound) {
        ConfigAccessor.SetNodeByPath(node, cfgPath, SerializeCompoundFieldsToNode(value));
      } else if (isCustomSimpleType) {
        ConfigAccessor.SetValueByPath(node, cfgPath, ((IPersistentField)value).SerializeToString());
      } else {
        ConfigAccessor.SetValueByPath(node, cfgPath, simpleTypeProto.SerializeToString(value));
      }
    }
  }

  /// <summary>Reads field from a config node.</summary>
  /// <param name="node">A node to read state from.</param>
  /// <param name="instance">An owner of the field. Can be <c>null</c> for static fields.</param>
  public void ReadFromConfig(ConfigNode node, object instance) {
    var value = fieldInfo.GetValue(instance);
    if (collectionProto != null) {
      // For collection field use existing object and restore its items. 
      if (value == null) {
        Debug.LogWarningFormat("Skip reading collection field {0}.{1} due to it's not initalized",
                               fieldInfo.DeclaringType.FullName, fieldInfo.Name);
        return;
      }
      collectionProto.ClearItems(value);
      if (isCompound) {
        // For compound items read nodes and have them parsed.
        var itemCfgs = ConfigAccessor.GetNodesByPath(node, cfgPath);
        if (itemCfgs != null) {
          foreach (var itemCfg in itemCfgs) {
            var itemValue = Activator.CreateInstance(collectionProto.GetItemType());
            DeserializeCompoundFieldsFromNode(itemCfg, itemValue);
            collectionProto.AddItem(value, itemValue);
          }
        }
      } else {
        // For ordinary items read strings and have them parsed. 
        var itemCfgs = ConfigAccessor.GetValuesByPath(node, cfgPath);
        if (itemCfgs != null) {
          foreach (var itemCfg in itemCfgs) {
            try {
              object itemValue;
              if (isCustomSimpleType) {
                itemValue = Activator.CreateInstance(collectionProto.GetItemType());
                ((IPersistentField)itemValue).ParseFromString(itemCfg);
              } else {
                itemValue = simpleTypeProto.ParseFromString(itemCfg, collectionProto.GetItemType());
              }
              collectionProto.AddItem(value, itemValue);
            } catch (Exception ex) {
              Debug.LogErrorFormat("Cannot parse value \"{0}\" as {1}: {2}",
                                   itemCfgs, collectionProto.GetItemType().FullName, ex.Message);
            }
          }
        }
      }
    } else {
      // For ordinary field just restore value and assign it to the field.
      if (isCompound) {
        if (value != null) {
          DeserializeCompoundFieldsFromNode(ConfigAccessor.GetNodeByPath(node, cfgPath), value);
        } else {
          Debug.LogWarningFormat("Skip reading compound field {0}.{1} due to it's not initalized",
                                 fieldInfo.DeclaringType.FullName, fieldInfo.Name);
        }
      } else {
        var cfgValue = ConfigAccessor.GetValueByPath(node, cfgPath);
        if (cfgValue != null) {
          try {
            object fieldValue;
            if (isCustomSimpleType) {
              // Prefer the existing instance of the field value when available.
              fieldValue = value ?? Activator.CreateInstance(fieldInfo.FieldType);
              ((IPersistentField)fieldValue).ParseFromString(cfgValue);
            } else {
              fieldValue = simpleTypeProto.ParseFromString(cfgValue, fieldInfo.FieldType);
            }
            fieldInfo.SetValue(instance, fieldValue);
          } catch (Exception ex) {
            Debug.LogErrorFormat("Cannot parse value \"{0}\" as {1}: {2}",
                                 cfgValue, fieldInfo.FieldType.FullName, ex.Message);
          }
        }
      }
    }
  }

  /// <summary>Makes a config node from the compound type fields.</summary>
  /// <param name="instance">Owner of the fields. Can be <c>null</c> for static fields.</param>
  /// <returns>New configuration node with the data.</returns>
  ConfigNode SerializeCompoundFieldsToNode(object instance) {
    var node = new ConfigNode();
    if (compoundTypeFields.Length > 0) {
      foreach (var compoundTypeField in compoundTypeFields) {
        compoundTypeField.WriteToConfig(node, instance);
      }
    }
    var configNode = instance as IConfigNode;
    if (configNode != null) {
      configNode.Save(node);
    }
    return node;
  }
  
  /// <summary>Sets a compound type field values from the config node.</summary>
  /// <param name="node">Node to read values from.</param>
  /// <param name="instance">Owner of the fields.</param>
  void DeserializeCompoundFieldsFromNode(ConfigNode node, object instance) {
    if (compoundTypeFields.Length > 0) {
      foreach (var compoundTypeField in compoundTypeFields) {
        compoundTypeField.ReadFromConfig(node, instance);
      }
    }
    var configNode = instance as IConfigNode;
    if (configNode != null) {
      configNode.Load(node);
    }
  }
}

}  // namespace
