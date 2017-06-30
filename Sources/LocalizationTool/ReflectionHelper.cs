// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Reflection;

namespace KSPDev.LocalizationTool {

/// <summary>A set of the methods to deal with a type via the reflections.</summary>
static class ReflectionHelper {
  /// <summary>Checks if the specified type is a descendant of the parent class.</summary>
  /// <remarks>
  /// Different modules may use own versions of the KSPDev Utils library. So access the data through
  /// the reflections rather than casting to a type (which would be much more simpler).
  /// </remarks>
  /// <param name="type">The descendant type to check.</param>
  /// <param name="fullClassName">The full name of the parent type.</param>
  /// <returns><c>true</c> if the type is a descendant of the parent.</returns>
  public static bool CheckReflectionParent(Type type, string fullClassName) {
    // Search the class by a prefix since it may have an assembly version part.
    while (type != null && !type.FullName.StartsWith(fullClassName, StringComparison.Ordinal)) {
      type = type.BaseType;
    }
    return type != null;
  }

  /// <summary>Returns a string value of a field or property via reflections.</summary>
  /// <param name="instance">The instance to get the value from.</param>
  /// <param name="memberName">The name of the member (either a field or a property).</param>
  /// <returns>A string value or <c>null</c> if the value cannot be casted to string.</returns>
  public static string GetReflectedString(object instance, string memberName) {
    var fieldInfo = instance.GetType().GetField(memberName);
    if (fieldInfo != null) {
      return fieldInfo.GetValue(instance) as string;
    }
    var propertyInfo = instance.GetType().GetProperty(memberName);
    if (propertyInfo != null) {
      return propertyInfo.GetValue(instance, null) as string;
    }
    return null;
  }

  /// <summary>Sets a string value to a field or porperty via reflections.</summary>
  /// <param name="instance">The instance to get the value from.</param>
  /// <param name="memberName">The name of the member (either a field or a property).</param>
  /// <param name="newValue">The value to set.</param>
  public static void SetReflectedString(object instance, string memberName, string newValue) {
    var fieldInfo = instance.GetType().GetField(memberName);
    if (fieldInfo != null) {
      fieldInfo.SetValue(instance, newValue);
      return;
    }
    var propertyInfo = instance.GetType().GetProperty(memberName);
    if (propertyInfo != null) {
      propertyInfo.SetValue(instance, newValue, null);
    }
  }
}

}  // namespace
