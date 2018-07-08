// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region VelocityTypeDemo1
public class VelocityTypeDemo1 : PartModule {
  static readonly Message<VelocityType> msg1 = new Message<VelocityType>(
      "#TypeDemo_msg1", defaultTemplate: "Speed is: <<1>>");

  void Show() {
    Debug.Log(msg1.Format(0.051));
    // Prints: "Speed is: 0.051m/s"
    Debug.Log(msg1.Format(0.45));
    // Prints: "Speed is: 0.45m/s"
    Debug.Log(msg1.Format(95.45));
    // Prints: "Speed is: 95.5m/s"
    Debug.Log(msg1.Format(120.45));
    // Prints: "Speed is: 121m/s"
    Debug.Log(msg1.Format(9535.45));
    // Prints: "Speed is: 9.54km/s"
  }
}
#endregion

public class VelocityTypeDemo2 {
  void FormatDefault() {
    #region VelocityTypeDemo2_FormatDefault
    Debug.Log(VelocityType.Format(0.051));
    // Prints: "Speed is: 0.051m/s"
    Debug.Log(VelocityType.Format(0.45));
    // Prints: "Speed is: 0.45m/s"
    Debug.Log(VelocityType.Format(95.45));
    // Prints: "Speed is: 95.5m/s"
    Debug.Log(VelocityType.Format(120.45));
    // Prints: "Speed is: 121m/s"
    Debug.Log(VelocityType.Format(9535.45));
    // Prints: "Speed is: 9.54km/s"
    #endregion
  }

  void FormatFixed() {
    #region VelocityTypeDemo2_FormatFixed
    Debug.Log(VelocityType.Format(1234.5678, format: "0.0000"));
    // Prints: "1234.5678m/s"
    Debug.Log(VelocityType.Format(1234.5678, format: "0.00"));
    // Prints: "1234.57m/s"
    Debug.Log(VelocityType.Format(1234.5678, format: "#,##0.00"));
    // Prints: "1,234.57m/s"
    #endregion
  }
}

}  // namespace
