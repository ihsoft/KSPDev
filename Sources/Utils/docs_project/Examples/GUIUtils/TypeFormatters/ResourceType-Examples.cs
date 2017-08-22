// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using KSPDev.ResourceUtils;
using UnityEngine;

namespace Examples {

#region ResourceTypeDemo1
public class ResourceTypeDemo1 : PartModule {
  static readonly Message<ResourceType> msg1 = new Message<ResourceType>(
      "#TypeDemo_msg1", defaultTemplate: "Resource is: <<1>>");

  void Show() {
    Debug.Log(msg1.Format(StockResourceNames.ElectricCharge));
    // Prints: "Resource is: Electric Charge"
    Debug.Log(msg1.Format(PartResourceLibrary.ElectricityHashcode));
    // Prints: "Resource is: Electric Charge"
  }
}
#endregion

public class ResourceTypeDemo2 {
  void FormatDefault() {
    #region ResourceTypeDemo2_FormatDefault
    Debug.Log(ResourceType.Format(StockResourceNames.ElectricCharge));
    // Prints: "Resource is: Electric Charge"
    Debug.Log(ResourceType.Format(PartResourceLibrary.ElectricityHashcode));
    // Prints: "Resource is: Electric Charge"
    #endregion
  }
}


}  // namespace
