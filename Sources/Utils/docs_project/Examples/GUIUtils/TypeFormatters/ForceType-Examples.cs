// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using KSPDev.GUIUtils.TypeFormatters;
using UnityEngine;

namespace Examples {

#region ForceTypeDemo1
public class ForceTypeDemo1 : PartModule {
  static readonly Message<ForceType> msg1 = new Message<ForceType>(
      "#TypeDemo_msg1", defaultTemplate: "Force is: <<1>>");

  // Depending on the current language in the system, this method will present different unit names. 
  void Show() {
    Debug.Log(msg1.Format(0.051));
    // Prints: "Force is: 0.051 kN"
    Debug.Log(msg1.Format(0.45));
    // Prints: "Force is: 0.45 kN"
    Debug.Log(msg1.Format(95.45));
    // Prints: "Force is: 95.5 kN"
    Debug.Log(msg1.Format(120.45));
    // Prints: "Force is: 120 kN"
    Debug.Log(msg1.Format(9535.45));
    // Prints: "Force is: 9536 kN"
  }
}
#endregion

public class ForceTypeDemo2 {
  void FormatDefault() {
    #region ForceTypeDemo2_FormatDefault
    Debug.Log(ForceType.Format(0.051));
    // Prints: "0.051 kN"
    Debug.Log(ForceType.Format(0.45));
    // Prints: "0.45 kN"
    Debug.Log(ForceType.Format(95.45));
    // Prints: "95.5 kN"
    Debug.Log(ForceType.Format(120.45));
    // Prints: "120 kN"
    Debug.Log(ForceType.Format(9535.45));
    // Prints: "9536 kN"
    #endregion
  }

  void FormatFixed() {
    #region ForceTypeDemo2_FormatFixed
    Debug.Log(ForceType.Format(1234.5678, format: "0.0000"));
    // Prints: "1234.5678 kN"
    Debug.Log(ForceType.Format(1234.5678, format: "0.00"));
    // Prints: "1234.57 kN"
    #endregion
  }
}

}  // namespace
