// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Reflection;

namespace KSPDev.ConfigUtils {

/// <summary>A base class for repeated fields handlers.</summary>
/// <remarks>You may provide own handler to deal with repeated values fields. Custom handler must
/// inherit from this class. See examples in <seealso cref="PersistentFieldAttribute"/>.</remarks>
public abstract class RepeatableFieldHandler {
  /// <summary>A field this handler is bound to.</summary>
  protected readonly PersistentField persistentField;
  /// <summary>A method to use to add new values into a field value.</summary>
  protected readonly MethodInfo addMethod;
  /// <summary>A type of the repeated values.</summary>
  protected readonly Type itemType;

  protected RepeatableFieldHandler(PersistentField field, Type itemType, MethodInfo addMethod) {
    this.persistentField = field;
    this.itemType = itemType;
    this.addMethod = addMethod;
    if (addMethod == null) {
      throw new ArgumentException(
          string.Format("Cannot find method Add() on type {0} in field {1}",
                        field.fieldInfo.FieldType, field.fieldInfo.Name));
    }
  }

  /// <summary>A method to add a value into the field.</summary>
  /// <param name="obj">An instance owning the field. Can be <c>null</c> if field is declared as
  /// static.</param>
  /// <param name="item">An item instance to add.</param>
  public void AddItem(object obj, object item) {
    addMethod.Invoke(obj, new[] {item});    
  }
  
  /// <summary>Returns container item type.</summary>
  /// <returns>Item type.</returns>
  public Type GetItemType() {
    return itemType;
  }

  public abstract object CreateFromConfigNode(ConfigNode node);
  public abstract void SaveToConfigNode(ConfigNode node, string[] path, object obj);
}
 
}  // namespace
