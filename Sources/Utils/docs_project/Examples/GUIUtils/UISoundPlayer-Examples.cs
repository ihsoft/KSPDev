// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region UISoundPlayerDemo1
public class UISoundPlayerDemo1 : PartModule {
  public override void OnAwake() {
    // To not loose the latency on the sound play pre-cache it.
    UISoundPlayer.instance.CacheSound("ooo.ogg");
  }

  public override void OnUpdate() {
    if (Input.GetKeyDown("O")) {
       UISoundPlayer.instance.Play("ooo.ogg");  // Played from cache. No delay.
    }
    if (Input.GetKeyDown("P")) {
      UISoundPlayer.instance.Play("ppp.ogg");  // May delay the game while loading the resource.
    }
  }
}
#endregion

}  // namespace
