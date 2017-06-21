// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region PercentTypeDemo1
public class PercentTypeDemo1 : PartModule {
  // This message uses a force type as a parameter.
  static readonly Message<PercentType> msg1 = new Message<PercentType>(
      "#TypeDemo_msg1", defaultTemplate: "Ratio is: <<1>>");

  // Depending on the current language in the system, this method will present different unit names. 
  void Show() {
    Debug.Log(msg1.Format(0.004));
    // Prints: "Ratio is: 0.40%"
    Debug.Log(msg1.Format(0.0041));
    // Prints: "Ratio is: 0.41%"
    Debug.Log(msg1.Format(0.01));
    // Prints: "Ratio is: 1.0%"
    Debug.Log(msg1.Format(0.014));
    // Prints: "Ratio is: 1.4%"
    Debug.Log(msg1.Format(0.0145));
    // Prints: "Ratio is: 1.45%"
    Debug.Log(msg1.Format(0.01456));
    // Prints: "Ratio is: 1.46%"
    Debug.Log(msg1.Format(0.1));
    // Prints: "Ratio is: 10%"
    Debug.Log(msg1.Format(0.105));
    // Prints: "Ratio is: 11%"
    Debug.Log(msg1.Format(5.5));
    // Prints: "Ratio is: 550%"
  }
}
#endregion

public class PercentTypeDemo2 {
  void FormatDefault() {
    #region PercentTypeDemo2_FormatDefault
    Debug.Log(PercentType.Format(0.004));
    // Prints: "0.40%"
    Debug.Log(PercentType.Format(0.0041));
    // Prints: "0.41%"
    Debug.Log(PercentType.Format(0.01));
    // Prints: "1.0%"
    Debug.Log(PercentType.Format(0.014));
    // Prints: "1.4%"
    Debug.Log(PercentType.Format(0.0145));
    // Prints: "1.45%"
    Debug.Log(PercentType.Format(0.01456));
    // Prints: "1.46%"
    Debug.Log(PercentType.Format(0.1));
    // Prints: "10%"
    Debug.Log(PercentType.Format(0.105));
    // Prints: "11%"
    Debug.Log(PercentType.Format(5.5));
    // Prints: "550%"
    #endregion
  }

  void FormatFixed() {
    #region PercentTypeDemo2_FormatFixed
    Debug.Log(PercentType.Format(0.12345678, format: "0.0000"));
    // Prints: "12.3457%"
    Debug.Log(PercentType.Format(0.12345678, format: "0.00"));
    // Prints: "12.35%"
    #endregion
  }
}

}  // namespace
