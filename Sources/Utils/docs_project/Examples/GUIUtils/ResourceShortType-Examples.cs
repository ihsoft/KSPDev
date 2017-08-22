// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using KSPDev.ResourceUtils;
using UnityEngine;

namespace Examples {

#region ResourceShortTypeDemo1
public class ResourceShortTypeDemo1 : PartModule {
  static readonly Message<ResourceShortType> msg1 = new Message<ResourceShortType>(
      "#TypeDemo_msg1", defaultTemplate: "Resource is: <<1>>");

  void Show() {
    Debug.Log(msg1.Format(StockResourceNames.ElectricCharge));
    // Prints: "Resource is: EC"
    Debug.Log(msg1.Format(PartResourceLibrary.ElectricityHashcode));
    // Prints: "Resource is: EC"
  }
}
#endregion

public class ResourceShortTypeDemo2 {
  void FormatDefault() {
    #region ResourceShortTypeDemo2_FormatDefault
    Debug.Log(ResourceShortType.Format(StockResourceNames.ElectricCharge));
    // Prints: "Resource is: EC"
    Debug.Log(ResourceShortType.Format(PartResourceLibrary.ElectricityHashcode));
    // Prints: "Resource is: EC"
    #endregion
  }
}


}  // namespace
