// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.LogUtils;
using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region DistanceTypeDemo1
public class DistanceTypeDemo1 : PartModule {
  // This message uses a distance type as a parameter.
  static readonly Message<DistanceType> msg1 = new Message<DistanceType>(
      "#DistanceTypeDemo_msg1", defaultTemplate: "Distance is: <<0>>");

  // Depending on the current language in the system, this method will present different unit names. 
  void ShowDistance() {
    HostedDebugLog.Info(this, msg1.Format(0.051));
    // Prints: "Distance is: 0.051 m"
    HostedDebugLog.Info(this, msg1.Format(0.45));
    // Prints: "Distance is: 0.45 m"
    HostedDebugLog.Info(this, msg1.Format(95.45));
    // Prints: "Distance is: 95.5 m"
    HostedDebugLog.Info(this, msg1.Format(120.45));
    // Prints: "Distance is: 120 m"
    HostedDebugLog.Info(this, msg1.Format(9535.45));
    // Prints: "Distance is: 9535 m"
    HostedDebugLog.Info(this, msg1.Format(12345.45));
    // Prints: "Distance is: 12.4 km"
    HostedDebugLog.Info(this, msg1.Format(123456.45));
    // Prints: "Distance is: 123456 km"
  }
}
#endregion

public class DistanceTypeDemo2 {
  void FormatDefault() {
    #region DistanceTypeDemo2_FormatDefault
    Debug.Log(DistanceType.Format(0.051));
    // Prints: "Distance is: 0.051 m"
    Debug.Log(DistanceType.Format(0.45));
    // Prints: "Distance is: 0.45 m"
    Debug.Log(DistanceType.Format(95.45));
    // Prints: "Distance is: 95.5 m"
    Debug.Log(DistanceType.Format(120.45));
    // Prints: "Distance is: 120 m"
    Debug.Log(DistanceType.Format(9535.45));
    // Prints: "Distance is: 9535 m"
    Debug.Log(DistanceType.Format(12345.45));
    // Prints: "Distance is: 12.4 km"
    Debug.Log(DistanceType.Format(123456.45));
    // Prints: "Distance is: 123456 km"
    #endregion
  }

  void FormatWithScale() {
    #region DistanceTypeDemo2_FormatWithScale
    Debug.Log(DistanceType.Format(123456.56, scale: 1000));
    // Prints: "Distance is: 123.5 km"
    Debug.Log(DistanceType.Format(123456.56, scale: 1));
    // Prints: "Distance is: 123456.6 km"
    Debug.Log(DistanceType.Format(123456.56, scale: 10));
    // Scale 10 is not known, so it's rounded down to 1.
    // Prints: "Distance is: 123456.6 km"
    Debug.Log(DistanceType.Format(123.56, scale: 1000));
    // Prints: "Distance is: 0.1 km"
    #endregion
  }

  void FormatFixed() {
    #region DistanceTypeDemo2_FormatFixed
    Debug.Log(DistanceType.Format(1234.5678, format: "0.0000"));
    // Prints: "Distance is: 1234.5678 m"
    Debug.Log(DistanceType.Format(1234.5678, format: "0.00"));
    // Prints: "Distance is: 1234.57 m"
    Debug.Log(DistanceType.Format(1234.5678, format: "0.0000", scale: 1000));
    // Prints: "Distance is: 1.2346 km"
    Debug.Log(DistanceType.Format(1234.5678, format: "0.00", scale: 1000));
    // Prints: "Distance is: 1.24 km"
    #endregion
  }
}

}  // namespace
