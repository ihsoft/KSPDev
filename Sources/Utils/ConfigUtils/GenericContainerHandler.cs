// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

  /// <summary>A method to add a value into the field.</summary>
  /// <remarks>Default implementation uses method <c>Add</c> which is looked up by name in the
  /// field's value metadata. You can override this method to use own logic for adding new items
  /// into the container.</remarks>
  /// <param name="obj">An instance owning the field. Can be <c>null</c> if fields is declared as
  /// static.</param>
  /// <param name="item">An item instance to add.</param>
public class GenericContainerHandler : RepeatableFieldHandler {
  public GenericContainerHandler(PersistentField field)
    : base(field, DetectItemType(field), field.fieldInfo.FieldType.GetMethod("Add")) {
  }

  public override object CreateFromConfigNode(ConfigNode node) {
    object instance = null;
    if (persistentField.simpleValueHandler != null) {
      // Simple values are read from strings.
      var values = ConfigAccessor.GetValues(node, persistentField.cfgPath);
      if (values != null) {
        instance = Activator.CreateInstance(persistentField.fieldInfo.FieldType);
        foreach (var value in values) {
          AddItem(instance,
              persistentField.simpleValueHandler.CreateFromString(GetItemType(), value));
        }
      }
    } else {
      // Compound values are read from config nodes.
      var nodes = ConfigAccessor.GetNodes(node, persistentField.cfgPath);
      if (nodes != null) {
        instance = Activator.CreateInstance(persistentField.fieldInfo.FieldType);
        foreach (var itemNode in nodes) {
          AddItem(instance,
              persistentField.compoundValueHandler.CreateFromConfigNode(itemNode, GetItemType()));
        }
      }
    }
    return instance;
  }

  public override void SaveToConfigNode(ConfigNode node, string[] path, object obj) {
    // TODO: implement.
  }

  /// <summary>Deducts generic's item type.</summary>
  /// <remarks>Expects the generic to be of exactly one argument, and that argument is the item
  /// type.</remarks>
  /// <param name="field">A field to deduct type for.</param>
  /// <returns>Item's type.</returns>
  private static Type DetectItemType(PersistentField field) {
    var genericArgs = field.fieldInfo.FieldType.GetGenericArguments();
    if (genericArgs.Length != 1) {
      throw new ArgumentException(string.Format(
          "Generic type {0} must have exactly one argument. Found: {1}",
          field.fieldInfo.FieldType, genericArgs.Length));
    }
    return genericArgs[0];
  }
}
  
}  // namespace
