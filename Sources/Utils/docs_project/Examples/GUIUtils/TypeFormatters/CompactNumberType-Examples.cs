// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region CompactNumberType1
public class CompactNumberType1 : PartModule {
  static readonly Message<CompactNumberType> msg1 = new Message<CompactNumberType>(
      "#TypeDemo_msg1", defaultTemplate: "Value is: <<1>>");

  void Show() {
    Debug.Log(msg1.Format(0.051));
    // Prints: "Value is: 0.051"
    Debug.Log(msg1.Format(0.45));
    // Prints: "Value is: 0.45"
    Debug.Log(msg1.Format(95.45));
    // Prints: "Value is: 95.5"
    Debug.Log(msg1.Format(120.45));
    // Prints: "Value is: 121"
    Debug.Log(msg1.Format(9535.45));
    // Prints: "Value is: 9,536"
  }
}
#endregion

public class CompactNumberType2 {
  void FormatDefault() {
    #region CompactNumberType2_FormatDefault
    Debug.Log(CompactNumberType.Format(0.051));
    // Prints: "Value is: 0.051"
    Debug.Log(CompactNumberType.Format(0.45));
    // Prints: "Value is: 0.45"
    Debug.Log(CompactNumberType.Format(95.45));
    // Prints: "Value is: 95.5"
    Debug.Log(CompactNumberType.Format(120.45));
    // Prints: "Value is: 121"
    Debug.Log(CompactNumberType.Format(9535.45));
    // Prints: "Value is: 9,536"
    #endregion
  }

  void FormatFixed() {
    #region CompactNumberType2_FormatFixed
    Debug.Log(DistanceType.Format(1234.5678, format: "0.0000"));
    // Prints: "1234.5678"
    Debug.Log(DistanceType.Format(1234.5678, format: "0.00"));
    // Prints: "1234.57"
    Debug.Log(DistanceType.Format(1234.5678, format: "#,##0.00"));
    // Prints: "1,234.57"
    #endregion
  }
}

}  // namespace
