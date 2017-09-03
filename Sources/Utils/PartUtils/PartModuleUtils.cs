// This is an intermediate module for methods and classes that are considred as candidates for
// KSPDev Utilities. Ideally, this module is always empty but there may be short period of time
// when new functionality lives here and not in KSPDev.

using KSPDev.LogUtils;
using System;

namespace KSPDev.PartUtils {

/// <summary>
/// Utility class to deals with the attributed fields and methods of the KPS part modules.
/// </summary>
/// <example><code source="Examples/PartUtils/PartModuleUtils-Examples.cs" region="PartModuleUtils_SetupEvent"/></example>
/// <example><code source="Examples/PartUtils/PartModuleUtils-Examples.cs" region="PartModuleUtils_GetEvent"/></example>
public static class PartModuleUtils {
  /// <summary>Returns an event for the requested method.</summary>
  /// <remarks>
  /// This method requests a KPS event of the part's module by its signature instead of a string
  /// literal name. The same result could be achieved by accessing the <c>Events</c> field of the
  /// <c>PartModule</c> object. However, in case of using a string literal the refactoring and the
  /// source tracking tools won't be able to track the reference. The main goal of this method is to
  /// provide a compile time checking mechanism for the cases when the exact method is known at the
  /// compile time (e.g. in the class descendants).
  /// </remarks>
  /// <param name="partModule">The module to get the event for.</param>
  /// <param name="eventFn">The signature of the event in scope of the module.</param>
  /// <returns>An event, or <c>null</c> if nothing found for the method provided.</returns>
  /// <seealso cref="SetupEvent"/>
  /// <example><code source="Examples/PartUtils/PartModuleUtils-Examples.cs" region="PartModuleUtils_GetEvent"/></example>
  public static BaseEvent GetEvent(PartModule partModule, Action eventFn) {
    return partModule.Events[eventFn.Method.Name];
  }

  /// <summary>Applies a setup function on a KSP part module event.</summary>
  /// <param name="partModule">The module to find the event in.</param>
  /// <param name="eventFn">The event's method signature.</param>
  /// <param name="setupFn">The function to apply to the event if the one is found.</param>
  /// <returns>
  /// <c>true</c> if the event was found and the function was applied, <c>false</c> otherwise.
  /// </returns>
  /// <seealso cref="GetEvent"/>
  /// <example><code source="Examples/PartUtils/PartModuleUtils-Examples.cs" region="PartModuleUtils_SetupEvent"/></example>
  public static bool SetupEvent(
      PartModule partModule, Action eventFn, Action<BaseEvent> setupFn) {
    var moduleEvent = partModule.Events[eventFn.Method.Name];
    if (moduleEvent == null) {
      HostedDebugLog.Error(partModule, "Cannot find event: {0}", eventFn.Method.Name);
      return false;
    }
    setupFn.Invoke(moduleEvent);
    return true;
  }

  /// <summary>Returns an action for the requested method.</summary>
  /// <remarks>
  /// This method requests a KPS action of the part's module by its signature instead of a string
  /// literal name. The same result could be achieved by accessing the <c>Actions</c> field of the
  /// <c>PartModule</c> object. However, in case of using a string literal the refactoring and the
  /// source tracking tools won't be able to track the reference. The main goal of this method is to
  /// provide a compile time checking mechanism for the cases when the exact method is known at the
  /// compile time (e.g. in the class descendants).
  /// </remarks>
  /// <param name="partModule">The module to find the action in.</param>
  /// <param name="actionFn">The actions's method signature.</param>
  /// <returns>An action, or <c>null</c> if nothing found for the method provided.</returns>
  /// <seealso cref="SetupAction"/>
  public static BaseAction GetAction(PartModule partModule, Action<KSPActionParam> actionFn) {
    return partModule.Actions[actionFn.Method.Name];
  }

  /// <summary>Applies a setup function on a KSP part module action.</summary>
  /// <param name="partModule">The module to find the action in.</param>
  /// <param name="actionFn">The actions's method signature.</param>
  /// <param name="setupFn">The function to apply to the action if the one is found.</param>
  /// <returns>
  /// <c>true</c> if the action was found and the function was applied, <c>false</c> otherwise.
  /// </returns>
  /// <seealso cref="GetAction"/>
  public static bool SetupAction(
      PartModule partModule, Action<KSPActionParam> actionFn, Action<BaseAction> setupFn) {
    var moduleEvent = partModule.Actions[actionFn.Method.Name];
    if (moduleEvent == null) {
      HostedDebugLog.Error(partModule, "Cannot find action: {0}", actionFn.Method.Name);
      return false;
    }
    setupFn.Invoke(moduleEvent);
    return true;
  }
}
  
}  // namespace
