// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.DebugUtils {

/// <summary>
/// Attribute to mark a member as available for the runtime interaction. It may be exposed in the
/// debugging GUI.
/// </summary>
/// <seealso cref="DebugGui"/>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
public class DebugAdjustableAttribute : Attribute {

  /// <summary>User friendly name of the member. It will be presented in GUI.</summary>
  public readonly string caption;

  /// <summary>Any string to use to group the controls.</summary>
  public readonly string group;

  /// <summary>
  /// Creates an attribute that marks the member as availabel for the runtime changes. 
  /// </summary>
  /// <param name="caption">The user freindly string to present in GUI.</param>
  /// <param name="group">
  /// The group of the controls. It may be used to filter the found mebers in debug GUI.
  /// </param>
  public DebugAdjustableAttribute(string caption, string group = "") {
    this.caption = caption;
    this.group = group;
  }
}

}  // namespace
