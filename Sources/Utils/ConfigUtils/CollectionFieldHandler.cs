// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>A handler that manages collections in persitent fields.</summary>
internal sealed class CollectionFieldHandler {
  private readonly Type collectionType;
  private readonly PersistentField persistentField;
  private readonly AbstractCollectionTypeProto collectionProto;
  
  public CollectionFieldHandler(
      PersistentField persistentField, Type containerType, Type collectionProtoType) {
    this.containerType = containerType;
    this.persistentField = persistentField;

    this.collectionProto = Activator.CreateInstance(collectionProtoType, new[] {containerType})
        as AbstractCollectionTypeProto;
    if (this.collectionProto == null) {
      throw new ArgumentException(string.Format("Bad collection proto {0}", collectionProtoType));
    }
  }

  /// <summary>Stores collection values into a config node.</summary>
  /// <param name="node">A node to add values into.</param>
  /// <param name="value">A collection instance of type <see cref="containerType"/> to get
  /// values from.</param>
  internal void SerializeValues(ConfigNode node, object value) {
    var proto = collectionProto as GenericCollectionTypeProto;
    foreach (var itemValue in proto.GetEnumerator(value)) {
      if (itemValue == null) {
        continue;
      }
      var cfgData = persistentField.ordinaryFieldHandler.SerializeValue(itemValue);
      if (cfgData != null) {
        if (cfgData is ConfigNode) {
          ConfigAccessor.AddNodeByPath(node, persistentField.cfgPath, (ConfigNode) cfgData);
        } else {
          ConfigAccessor.AddValueByPath(node, persistentField.cfgPath, (string) cfgData);
        }
      }
    }
  }
  
  /// <summary>Creates a collection from the config node.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <returns>An collection instance of type <see cref="containerType"/>.</returns>
  internal object DeserializeValues(ConfigNode node) {
    object instance = null;
    var values = persistentField.ordinaryFieldHandler.IsCompound()
        ? ConfigAccessor.GetNodesByPath(node, persistentField.cfgPath) as object[]
        : ConfigAccessor.GetValuesByPath(node, persistentField.cfgPath) as object[];
    if (values != null) {
      instance = Activator.CreateInstance(containerType);
      foreach (var value in values) {
        var item = persistentField.ordinaryFieldHandler.DeserializeValue(value);
        if (item != null) {
          collectionProto.AddItem(instance, item);
        }
      }
    }
    return instance;
  }
  
  /// <summary>Returns type of an item in the colelction.</summary>
  /// <returns>Item's type.</returns>
  internal Type GetItemType() {
    return collectionProto.GetItemType();
  }
}

}  // namespace
