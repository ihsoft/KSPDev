// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.LogUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KSPDev.ConfigUtils {

/// <summary>A helper class to gather persistent field attributes.</summary>
static class PersistentFieldsFactory {
  /// <summary>Gathers persitent fields for a type.</summary>
  /// <remarks>It will only find the persistent fields of the same Utils version!</remarks>
  /// <param name="type">A type of to gather persistent fields for.</param>
  /// <param name="needStatic">Specifies if static fields need to be returned.</param>
  /// <param name="needInstance">Specifies if non-static fields need to be returned.</param>
  /// <param name="group">A filter group for the persitent fields. Note that group is ignored for
  /// the inner fields of a compound type.</param>
  /// <returns>List of persitent fields.</returns>
  public static List<PersistentField> GetPersistentFields(
      Type type, bool needStatic, bool needInstance, string group) {
    var result = new List<PersistentField>();
    var fieldsInfo = FindAnnotatedFields(type, needStatic, needInstance, group);
    foreach (var fieldInfo in fieldsInfo) {
      var fieldAttr =
          fieldInfo.GetCustomAttributes(typeof(BasePersistentFieldAttribute), true).First()
          as PersistentFieldAttribute;
      try {
        var persistentField = new PersistentField(fieldInfo, fieldAttr);
        result.Add(persistentField);
      } catch (Exception ex) {
        DebugEx.Error(
            "Ignoring field {0}.{1}: {2}\n{3}",
            type.FullName, fieldInfo.Name, ex.Message, ex.StackTrace);
      }
    }
    
    // Sort by config path to ensure the most top level nodes are handled before the children.
    result = result.OrderBy(x => string.Join("/", x.cfgPath)).ToList();

    return result;
  }

  /// <summary>Finds and returns peristent fields of the requested group.</summary>
  static IEnumerable<FieldInfo> FindAnnotatedFields(
      IReflect type, bool needStatic, bool needInstance, string group = null) {
    var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
    if (needStatic) {
      flags |= BindingFlags.Static;
    }
    if (needInstance) {
      flags |= BindingFlags.Instance;
    }
    return type.GetFields(flags).Where(f => FieldFilter(f, group));
  }

  /// <summary>Filters only persitent fields of the required group.</summary>
  static bool FieldFilter(ICustomAttributeProvider fieldInfo, string group) {
    // We need descendants of AbstractPersistentFieldAttribute as well.
    var attributes = fieldInfo.GetCustomAttributes(typeof(BasePersistentFieldAttribute), true)
        as BasePersistentFieldAttribute[];
    if (attributes.Length == 0) {
      return false;
    }
    return group == null || attributes[0].group.ToLowerInvariant().Equals(group.ToLowerInvariant());
  }
}

}  // namespace
