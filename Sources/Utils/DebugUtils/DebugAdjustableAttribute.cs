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

  /// <summary>
  /// Creates an attribute that marks the member as availabel for the runtime changes. 
  /// </summary>
  /// <param name="caption">The user freindly string to present in GUI.</param>
  public DebugAdjustableAttribute(string caption) {
    this.caption = caption;
  }
}

}  // namespace
