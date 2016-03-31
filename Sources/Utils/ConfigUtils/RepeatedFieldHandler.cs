// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>
/// A handler that manages repeated fields. All type specific handling is done via a proto.
/// </summary>
internal sealed class RepeatedFieldHandler {
  public readonly Type containerType;
  public readonly PersistentField persistentField;
  
  private readonly AbstractCollectionTypeProto repeatedProto;
  
  public RepeatedFieldHandler(
      PersistentField persistentField, Type containerType, Type repeatedProtoType) {
    this.containerType = containerType;
    this.persistentField = persistentField;

    this.repeatedProto = Activator.CreateInstance(repeatedProtoType, new[] {containerType})
        as AbstractCollectionTypeProto;
    if (this.repeatedProto == null) {
      throw new ArgumentException(string.Format("Bad collection proto {0}", repeatedProtoType));
    }
  }

  /// <summary>Stores collection values into a config node.</summary>
  /// <param name="node">A node to add values into.</param>
  /// <param name="value">A collection instance of type <see cref="containerType"/> to get
  /// values from.</param>
  public void SerializeValues(ConfigNode node, object value) {
    var proto = repeatedProto as GenericCollectionTypeProto;
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
  public object DeserializeValues(ConfigNode node) {
    object instance = null;
    var values = persistentField.ordinaryFieldHandler.IsCompound()
        ? ConfigAccessor.GetNodesByPath(node, persistentField.cfgPath) as object[]
        : ConfigAccessor.GetValuesByPath(node, persistentField.cfgPath) as object[];
    if (values != null) {
      instance = Activator.CreateInstance(containerType);
      foreach (var value in values) {
        var item = persistentField.ordinaryFieldHandler.DeserializeValue(value);
        if (item != null) {
          repeatedProto.AddItem(instance, item);
        }
      }
    }
    return instance;
  }
  
  /// <summary>Returns type of an item in the colelction.</summary>
  /// <returns>Item's type.</returns>
  public Type GetItemType() {
    return repeatedProto.GetItemType();
  }
}

}  // namespace
