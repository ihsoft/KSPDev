// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using KSPDev.GUIUtils.TypeFormatters;
using UnityEngine;

namespace Examples {

#region PressureTypeDemo1
public class PressureTypeDemo1 : PartModule {
  static readonly Message<PressureType> msg1 = new Message<PressureType>(
      "#TypeDemo_msg1", defaultTemplate: "Pressure is: <<1>>");

  void Show() {
    Debug.Log(msg1.Format(0.051));
    // Prints: "Pressure is: 0.051 kPa"
    Debug.Log(msg1.Format(0.45));
    // Prints: "Pressure is: 0.45 kPa"
    Debug.Log(msg1.Format(95.45));
    // Prints: "Pressure is: 95.5 kPa"
    Debug.Log(msg1.Format(120.45));
    // Prints: "Pressure is: 121 kPa"
    Debug.Log(msg1.Format(9535.45));
    // Prints: "Pressure is: 9,536 kPa"
  }
}
#endregion

public class PressureTypeDemo2 {
  void FormatDefault() {
    #region PressureTypeDemo2_FormatDefault
    Debug.Log(PressureType.Format(0.051));
    // Prints: "Pressure is: 0.051 kPa"
    Debug.Log(PressureType.Format(0.45));
    // Prints: "Pressure is: 0.45 kPa"
    Debug.Log(PressureType.Format(95.45));
    // Prints: "Pressure is: 95.5 kPa"
    Debug.Log(PressureType.Format(120.45));
    // Prints: "Pressure is: 121 kPa"
    Debug.Log(PressureType.Format(9535.45));
    // Prints: "Pressure is: 9,536 kPa"
    #endregion
  }

  void FormatFixed() {
    #region PressureTypeDemo2_FormatFixed
    Debug.Log(PressureType.Format(1234.5678, format: "0.0000"));
    // Prints: "1234.5678 kPa"
    Debug.Log(PressureType.Format(1234.5678, format: "0.00"));
    // Prints: "1234.57 kPa"
    Debug.Log(PressureType.Format(1234.5678, format: "#,##0.00"));
    // Prints: "1,234.57 kPa"
    #endregion
  }
}

}  // namespace
