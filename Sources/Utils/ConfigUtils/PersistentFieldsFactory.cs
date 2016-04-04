// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KSPDev.LogUtils;

namespace KSPDev.ConfigUtils {

/// <summary>A helper class to gather persistent field attributes.</summary>
public static class PersistentFieldsFactory {
  /// <summary>Gathers persitent fields for an instance of a type.</summary>
  /// <remarks>Static and instance fields are gathered.</remarks>
  /// <param name="obj">An instance of the object to gather persistent fields for.</param>
  /// <param name="group">A filter group for the persitent fields. Note that group is ignored for
  /// the compound type fields.</param>
  /// <returns>List of persitent fields.</returns>
  public static List<PersistentField> GetPersistentFields(object obj, string group = null) {
    return GetPersistentFields(obj.GetType(), BindingFlags.Static | BindingFlags.Instance, group);
  }

  /// <summary>Gathers persitent fields for a type.</summary>
  /// <remarks>Only static fields are gathered.</remarks>
  /// <param name="type">A type of to gather persistent fields for.</param>
  /// <param name="group">A filter group for the persitent fields. Note that group is ignored for
  /// the compound type fields.</param>
  /// <returns>List of persitent fields.</returns>
  public static List<PersistentField> GetPersistentFields(Type type, string group = null) {
    return GetPersistentFields(type, BindingFlags.Static, group);
  }

  /// <summary>Gathers persitent fields for a type.</summary>
  /// <remarks>Gives static or/and instance fields depedning on
  /// <paramref name="fieldModifierFlags"/>.</remarks>
  /// <param name="type">A type of to gather persistent fields for.</param>
  /// <param name="fieldModifierFlags">A set of field modifiers to fetch.</param>
  /// <param name="group">A filter group for the persitent fields. Note that group is ignored for
  /// the compound type fields.</param>
  /// <returns>List of persitent fields.</returns>
  public static List<PersistentField> GetPersistentFields(
      Type type, BindingFlags fieldModifierFlags, string group) {
    var result = new List<PersistentField>();
    var fieldsInfo = FindAnnotatedFields(type, fieldModifierFlags, group);
    foreach (var fieldInfo in fieldsInfo) {
      var fieldAttr =
          fieldInfo.GetCustomAttributes(typeof(AbstractPersistentFieldAttribute), true).First()
          as PersistentFieldAttribute;
      try {
        var persistentField = new PersistentField(fieldInfo, fieldAttr);
        result.Add(persistentField);
      } catch (Exception ex) {
        Logger.logError("Ignoring field {0}.{1}: {2}\n{3}",
                        type.FullName, fieldInfo.Name, ex.Message, ex.StackTrace);
      }
    }
    
    // Sort by config path to ensure the most top level nodes are handled before the children.
    result = result.OrderBy(x => string.Join("/", x.cfgPath)).ToList();

    return result;
  }

  /// <summary>Finds and returns peristent fields of the requested group.</summary>
  private static IEnumerable<FieldInfo> FindAnnotatedFields(
      IReflect type, BindingFlags fieldModifierFlags, string group = null) {
    BindingFlags flags = fieldModifierFlags | BindingFlags.NonPublic | BindingFlags.Public;
    return type.GetFields(flags).Where(f => FieldFilter(f, group));
  }

  /// <summary>Filters only persitent fields of the required group.</summary>
  private static bool FieldFilter(ICustomAttributeProvider fieldInfo, string group) {
    // We need descendants of AbstractPersistentFieldAttribute as well.
    var attributes = fieldInfo.GetCustomAttributes(typeof(AbstractPersistentFieldAttribute), true)
        as AbstractPersistentFieldAttribute[];
    if (attributes.Length == 0) {
      return false;
    }
    return group == null || attributes[0].group.ToLowerInvariant().Equals(group.ToLowerInvariant());
  }
}

}  // namespace
