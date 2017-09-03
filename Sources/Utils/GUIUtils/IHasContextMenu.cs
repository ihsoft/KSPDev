// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.KSPInterfaces;  // For the XML docs.

namespace KSPDev.GUIUtils {

/// <summary>Generic interface for the modules that implement a UI context menu.</summary>
/// <seealso href="https://kerbalspaceprogram.com/api/class_game_events.html#ae6daaa6f39473078514543a2f8485513">
/// KPS: GameEvents.onPartActionUICreate</seealso>
/// <seealso href="https://kerbalspaceprogram.com/api/class_game_events.html#a7ccbd16e2aee0176a4431f0b5f9d63e5">
/// KPS: GameEvents.onPartActionUIDismiss</seealso>
public interface IHasContextMenu {
  /// <summary>
  /// A callback which is called every time the module's context menu items need to update. 
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a part needs to update its context menu, it must not be doing it in the methods other
  /// this one. By doing the update in a single method, the part ensures there will be a
  /// consistency.
  /// </para>
  /// <para>
  /// It's very implementation dependent when and why the update is needed. However, at the very
  /// least this callback must be called from the <see cref="IPartModule.OnLoad">OnLoad</see> method
  /// to let the module to update the state and the titles.
  /// </para>
  /// <para>
  /// The method implemenation must expect to be called at any arbitrary moment of the module's
  /// life. If there are members that can be null, then they must be checked in this method to not
  /// throw an NPE. A special attention should be paid in the implementations that depend on the
  /// other modules since this method can get called when they are not initialized or created yet.
  /// </para>
  /// <para>
  /// Unless the implementing class is <i>sealed</i>, the method must be declared as <i>virtual</i>.
  /// The descendants may want to react on the callback as well.
  /// </para>
  /// </remarks>
  void UpdateContextMenu();
}

}  // namespace
