// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

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
  /// <summary>Notifies that one or more of the debug adjustable fields have changed.</summary>
  void OnDebugAdjustablesUpdated();
}

}  // namespace
