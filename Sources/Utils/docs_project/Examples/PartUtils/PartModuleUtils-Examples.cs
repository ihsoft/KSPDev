// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ProcessingUtils;
using KSPDev.LogUtils;
using KSPDev.PartUtils;
using System;
using UnityEngine;

namespace Examples {

#region PartModuleUtils_SetupEvent
class PartModuleUtils_SetupEvent : PartModule {
  [KSPEvent(guiName = "test", guiActive = true, active = false)]
  void TestEvent() {
    Debug.Log("Test event triggered");
  }

  void SetupEvents() {
    // This call will activate the event.
    PartModuleUtils.SetupEvent(this, TestEvent, x => x.active = true);
  }
}
#endregion

#region PartModuleUtils_GetEvent
class PartModuleUtils_GetEvent : PartModule {
  [KSPEvent(guiName = "test", guiActive = true, active = false)]
  void TestEvent() {
    Debug.Log("Test event triggered");
  }

  void SetupEvents() {
    var e = PartModuleUtils.GetEvent(this, TestEvent);
    e.active = true;  // Activates the event.
  }
}
#endregion

}  // namespace

