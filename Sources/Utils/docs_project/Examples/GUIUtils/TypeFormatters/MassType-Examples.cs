// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using KSPDev.GUIUtils.TypeFormatters;
using UnityEngine;

namespace Examples {

#region MassTypeDemo1
public class MassTypeDemo1 : PartModule {
  static readonly Message<MassType> msg1 = new Message<MassType>(
      "#TypeDemo_msg1", defaultTemplate: "Mass is: <<1>>");

  // Depending on the current language in the system, this method will present different unit names. 
  void Show() {
    Debug.Log(msg1.Format(1.0));
    // Prints: "Mass is: 1.0 t"
    Debug.Log(msg1.Format(1.567));
    // Prints: "Mass is: 1.57 t"
    Debug.Log(msg1.Format(10.0));
    // Prints: "Mass is: 10 t"
    Debug.Log(msg1.Format(10.56));
    // Prints: "Mass is: 10.6 t"
    Debug.Log(msg1.Format(100.0));
    // Prints: "Mass is: 100 t"
    Debug.Log(msg1.Format(100.5));
    // Prints: "Mass is: 101 t"

    Debug.Log(msg1.Format(0.1234567));
    // Prints: "Mass is: 124 kg"
    Debug.Log(msg1.Format(0.0123456));
    // Prints: "Mass is: 12.4 kg"
    Debug.Log(msg1.Format(0.0012345));
    // Prints: "Mass is: 1.24 kg"

    Debug.Log(msg1.Format(0.0001234567));
    // Prints: "Mass is: 124 g"
    Debug.Log(msg1.Format(0.0000123456));
    // Prints: "Mass is: 12.4 g"
    Debug.Log(msg1.Format(0.0000012356));
    // Prints: "Mass is: 1.24 g"
    Debug.Log(msg1.Format(0.0000001235));
    // Prints: "Mass is: 0.124 g"
  }
}
#endregion

public class MassTypeDemo2 {
  void FormatDefault() {
    #region MassTypeDemo2_FormatDefault
    Debug.Log(MassType.Format(1.0));
    // Prints: "1.0 t"
    Debug.Log(MassType.Format(1.567));
    // Prints: "1.57 t"
    Debug.Log(MassType.Format(10.0));
    // Prints: "10 t"
    Debug.Log(MassType.Format(10.56));
    // Prints: "10.6 t"
    Debug.Log(MassType.Format(100.0));
    // Prints: "100 t"
    Debug.Log(MassType.Format(100.5));
    // Prints: "101 t"

    Debug.Log(MassType.Format(0.1234567));
    // Prints: "124 kg"
    Debug.Log(MassType.Format(0.0123456));
    // Prints: "12.4 kg"
    Debug.Log(MassType.Format(0.0012345));
    // Prints: "1.24 kg"

    Debug.Log(MassType.Format(0.0001234567));
    // Prints: "124 g"
    Debug.Log(MassType.Format(0.0000123456));
    // Prints: "12.4 g"
    Debug.Log(MassType.Format(0.0000012356));
    // Prints: "1.24 g"
    Debug.Log(MassType.Format(0.0000001235));
    // Prints: "0.124 g"
    #endregion
  }

  void FormatWithScale() {
    #region MassTypeDemo2_FormatWithScale
    Debug.Log(DistanceType.Format(0.12345678, scale: 1));
    // Prints: "0.124 t"
    Debug.Log(DistanceType.Format(0.12345678, scale: 0.001));
    // Prints: "124 kg"
    Debug.Log(DistanceType.Format(0.12345678, scale: 0.0001));
    // Scale 0.0001, so it's roudned up to 0.001
    // Prints: "124 kg"
    Debug.Log(DistanceType.Format(0.12345678, scale: 0.000001));
    // Prints: "123457 g"
    Debug.Log(DistanceType.Format(0.12345678, scale: 0.0000001));
    // Scale 0.0000001, so it's roudned up to 0.000001
    // Prints: "123457 g"
    #endregion
  }

  void FormatFixed() {
    #region MassTypeDemo2_FormatFixed
    Debug.Log(MassType.Format(0.12345678, format: "0.0000"));
    // Prints: "0.1235 t"
    Debug.Log(MassType.Format(0.12345678, format: "0.00"));
    // Prints: "0.12 t"
    Debug.Log(MassType.Format(0.12345678, format: "0.0000", scale: 0.001));
    // Prints: "123.4568 kg"
    Debug.Log(MassType.Format(0.12345678, format: "0.0000", scale: 0.000001));
    // Prints: "123456.7800 g"
    #endregion
  }
}

}  // namespace
