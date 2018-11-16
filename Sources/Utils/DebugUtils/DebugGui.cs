// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KSPDev.DebugUtils {

/// <summary>Helper class to deal with the debug GUI functionality.</summary>
/// <seealso cref="DebugAdjustableAttribute"/>
public static class DebugGui {

  /// <summary>Metadata about the member that is available for debugging.</summary>
  public struct DebugMemberInfo {
    /// <summary>Attribute, that describes the member.</summary>
    public DebugAdjustableAttribute attr;

    /// <summary>Field info for the field member.</summary>
    public FieldInfo fieldInfo;

    /// <summary>Property info for the property member.</summary>
    public PropertyInfo propertyInfo;

    /// <summary>Method info fro the method member.</summary>
    public MethodInfo methodInfo;
  }

  /// <summary>Gets the fields, available for debugging.</summary>
  /// <param name="obj">The instance to get the fielda from.</param>
  /// <returns>The member metainfo for all the available fields.</returns>
  public static List<DebugMemberInfo> GetAdjustableFields(object obj) {
    var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy
                | BindingFlags.Instance;
    var attrType = typeof(DebugAdjustableAttribute);
    return obj.GetType()
        .GetFields(flags)
        .Where(f => f.GetCustomAttributes(attrType, true).Length > 0)
        .Select(f => new DebugMemberInfo() {
          attr = f.GetCustomAttributes(attrType, true)[0] as DebugAdjustableAttribute,
          fieldInfo = f
        })
        .ToList();
  }
}

}  // namespace
