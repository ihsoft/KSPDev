// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.Extensions;
using UnityEngine;

namespace Examples {

public static class RectExtensions {
  #region SimpleUsage
  public static void Intersect() {
    var r1 = new Rect(10, 10, 200, 200);
    Debug.Log(r1.Intersect(new Rect(20, 20, 200, 200)));
    // Prints: (x:20.00, y:20.00, width:190.00, height:190.00)
    Debug.Log(r1.Intersect(new Rect(50, 50, 20, 20)));
    // Prints: (x:20.00, y:20.00, width:20.00, height:20.00)
    Debug.Log(r1.Intersect(new Rect(0, 0, 100, 100)));
    // Prints: (x:10.00, y:10.00, width:90.00, height:90.00)
    Debug.Log(r1.Intersect(new Rect(300, 300, 200, 200)));
    // Prints: (x:110.00, y:110.00, width:0.00, height:0.00)
  }
  #endregion
}

}  // namespace
