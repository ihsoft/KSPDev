// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ResourceUtils;
using UnityEngine;

namespace Examples {

class StockResourceNames1 {
  void PrintResourcesExample() {
    #region StockResourceNames1
    Debug.Log(StockResourceNames.ElectricCharge);
    // Prints: ElectricCharge

    Debug.Log(StockResourceNames.GetId(StockResourceNames.ElectricCharge));
    Debug.Log(PartResourceLibrary.ElectricityHashcode);
    // Prints the same integers: the resource ID of the electricity resource.

    Debug.Log(StockResourceNames.GetResourceTitle(StockResourceNames.ElectricCharge));
    Debug.Log(StockResourceNames.GetResourceTitle(PartResourceLibrary.ElectricityHashcode));
    // Prints the same display name: Electric Charge
    // The value is localized to the current game's locale.

    Debug.Log(StockResourceNames.GetResourceAbbreviation(StockResourceNames.ElectricCharge));
    Debug.Log(StockResourceNames.GetResourceAbbreviation(PartResourceLibrary.ElectricityHashcode));
    // Prints the same display name: EC
    // The value is localized to the current game's locale.
    #endregion
  }
}

}  // namespace

