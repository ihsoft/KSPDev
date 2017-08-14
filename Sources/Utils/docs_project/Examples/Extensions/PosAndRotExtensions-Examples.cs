// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.Extensions;
using KSPDev.Types;
using UnityEngine;

namespace Examples {

public static class PosAndRotExtensions {
  #region ToWorld
  public static void ToWorld() {
    var parent = new GameObject();
    parent.transform.position = Vector3.one;
    parent.transform.rotation = Quaternion.LookRotation(Vector3.up);
    var pr = new PosAndRot();
    Debug.LogFormat("Local: {0}", pr);
    Debug.LogFormat("World: {0}", parent.transform.TransformPosAndRot(pr));
  }
  #endregion

  #region ToLocal
  public static void ToLocal() {
    var parent = new GameObject();
    parent.transform.position = Vector3.one;
    parent.transform.rotation = Quaternion.LookRotation(Vector3.up);
    var pr = new PosAndRot(Vector3.one, new Vector3(90, 0, 0));
    Debug.LogFormat("Local: {0}", parent.transform.InverseTransformPosAndRot(pr));
    Debug.LogFormat("World: {0}", pr);
  }
  #endregion
}

}  // namespace
