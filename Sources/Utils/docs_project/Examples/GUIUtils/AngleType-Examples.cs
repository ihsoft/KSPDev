// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region AngleTypeDemo1
public class AngleTypeDemo1 : PartModule {
  static readonly Message<AngleType> msg1 = new Message<AngleType>(
      "#TypeDemo_msg1", defaultTemplate: "Angle is: <<1>>");

  // Depending on the current language in the system, this method will present different unit names. 
  void Show() {
    Debug.Log(msg1.Format(0.4));
    // Prints: "Angle is: 0.4°"
    Debug.Log(msg1.Format(0.41));
    // Prints: "Angle is: 0.41°"
    Debug.Log(msg1.Format(1.0));
    // Prints: "Angle is: 1°"
    Debug.Log(msg1.Format(1.41));
    // Prints: "Angle is: 1.4°"
    Debug.Log(msg1.Format(12.555));
    // Prints: "Angle is: 13°"
  }
}
#endregion

public class AngleTypeDemo2 {
  void FormatDefault() {
    #region AngleTypeDemo2_FormatDefault
    Debug.Log(AngleType.Format(0.4));
    // Prints: "0.4°"
    Debug.Log(AngleType.Format(0.41));
    // Prints: "0.41°"
    Debug.Log(AngleType.Format(1.0));
    // Prints: "1°"
    Debug.Log(AngleType.Format(1.41));
    // Prints: "1.4°"
    Debug.Log(AngleType.Format(12.555));
    // Prints: "13°"
    #endregion
  }

  void FormatFixed() {
    #region AngleTypeDemo2_FormatFixed
    Debug.Log(AngleType.Format(1234.5678, format: "0.0000"));
    // Prints: "1234.5678°"
    Debug.Log(AngleType.Format(1234.5678, format: "0.00"));
    // Prints: "1234.57°"
    #endregion
  }
}

}  // namespace
