// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System.Collections.Generic;
using KSPDev.LogUtils;

namespace Examples {

#region HostedLogExample1
public class HostedLogExample1 : PartModule {
  public override void OnAwake() {
    base.OnAwake();
    // The logging below will identify the owning part instance.
    HostedDebugLog.Info(part, "Part is being created");
    // The logging below will identify the part instance and the specific module in it.
    HostedDebugLog.Info(this, "Module created");
  }

  void Destroy() {
    // The logging below will will identify the game object name by it's full hierarchy path. 
    HostedDebugLog.Warning(part.transform.Find("model"), "Part's model is being destroyed");
  }
}
#endregion

}  // namespace
