// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using KSPDev.GUIUtils.TypeFormatters;
using UnityEngine;

namespace Examples {

#region CostTypeDemo1
public class CostTypeDemo1 : PartModule {
  static readonly Message<CostType> msg1 = new Message<CostType>(
      "#TypeDemo_msg1", defaultTemplate: "Cost is: <<1>>");

  void Show() {
    Debug.Log(msg1.Format(0.4));
    // Prints: "Cost is: √ 0.40"
    Debug.Log(msg1.Format(0.41));
    // Prints: "Cost is: √ 0.41"
    Debug.Log(msg1.Format(1.0));
    // Prints: "Cost is: √ 1.00"
    Debug.Log(msg1.Format(1.41));
    // Prints: "Cost is: √ 1.41"
    Debug.Log(msg1.Format(1234.555));
    // Prints: "Cost is: √ 1,234.56"
  }
}
#endregion

public class CostTypeDemo2 {
  void FormatDefault() {
    #region CostTypeDemo2_FormatDefault
    Debug.Log(CostType.Format(0.4));
    // Prints: "Cost is: √ 0.40"
    Debug.Log(CostType.Format(0.41));
    // Prints: "Cost is: √ 0.41"
    Debug.Log(CostType.Format(1.0));
    // Prints: "Cost is: √ 1.00"
    Debug.Log(CostType.Format(1.41));
    // Prints: "Cost is: √ 1.41"
    Debug.Log(CostType.Format(1234.555));
    // Prints: "Cost is: √ 1,234.56"
    #endregion
  }

  void FormatFixed() {
    #region CostTypeDemo2_FormatFixed
    Debug.Log(CostType.Format(1234.5678, format: "0.0000"));
    // Prints: "√ 1234.5678"
    Debug.Log(CostType.Format(1234.5678, format: "#,##0.00"));
    // Prints: "√ 1,234.57"
    #endregion
  }
}

}  // namespace
