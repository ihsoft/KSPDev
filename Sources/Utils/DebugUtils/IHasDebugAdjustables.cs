// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.DebugUtils {

/// <summary>
/// Interface for the components that wants to know when their debug adjustable fields or properties
/// have changed.
/// </summary>
/// <remarks>
/// The methods of this interface not normally called during the game. It's used by the debug code
/// to update the components in runtime.
/// </remarks>
/// <seealso cref="DebugGui"/>
public interface IHasDebugAdjustables {
  /// <summary>
  /// Notifies that one or more of the debug adjustable fields is about to be updated.
  /// </summary>
  /// <exception cref="InvalidOperationException">
  /// If the change must be rejected due to the module is not able to handle the update in its
  /// current state. The description of the exception may be presented in GUI.
  /// </exception>
  void OnBeforeDebugAdjustablesUpdate();

  /// <summary>Notifies that one or more of the debug adjustable fields have changed.</summary>
  void OnDebugAdjustablesUpdated();
}

}  // namespace
