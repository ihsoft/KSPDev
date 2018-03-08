// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ProcessingUtils;
using System;
using System.Collections;
using UnityEngine;

namespace Examples {

#region EndOfFrame
class AsyncCall_EndOfFrame : MonoBehaviour {
  void Update() {
    // Execute at the end of the current frame.
    AsyncCall.CallOnEndOfFrame(this, () => Debug.Log("Same frame async call!"));
    AsyncCall.CallOnEndOfFrame(
        this,
        () => AsyncCall.CallOnEndOfFrame(this, () => Debug.Log("Same frame, end of queue call!")));
    // Execute at the end of the next frame.
    AsyncCall.CallOnEndOfFrame(this, () => Debug.Log("Next frame async call!"), skipFrames: 1);
  }
}
#endregion

#region FixedFrame
class AsyncCall_FixedFrame : MonoBehaviour {
  void Update() {
    // Call the method at the next fixed frame update.
    AsyncCall.CallOnFixedUpdate(this, () => Debug.Log("Async call!"));
    // Wait for one fixed frame update and then call.
    AsyncCall.CallOnFixedUpdate(this, () => Debug.Log("Next frame async call!"), skipFrames: 1);
  }
}
#endregion

#region WaitForPhysics1
class AsyncCall_WaitForPhysics1 : MonoBehaviour {
  void Awake() {
    var count = 5;
    Debug.Log("Before start waiting");
    AsyncCall.WaitForPhysics(
        this, 10,
        () => --count == 0,
        success: () => Debug.Log("Success!"),
        failure: () => Debug.Log("Failure!"),
        update: x => Debug.LogFormat("...waiting: {0}", x));
    Debug.Log("After start waiting");

    // The output in the logs will be:
    // Before start waiting
    // ...waiting: 0
    // After start waiting
    // ...waiting: 1
    // ...waiting: 2
    // ...waiting: 3
    // ...waiting: 4
    // Success!
  }
}
#endregion

#region WaitForPhysics2
class AsyncCall_WaitForPhysics2 : MonoBehaviour {
  void Awake() {
    var count = 10;
    Debug.Log("Before start waiting");
    AsyncCall.WaitForPhysics(
        this, 5,
        () => --count == 0,
        success: () => Debug.Log("Success!"),
        failure: () => Debug.Log("Failure!"),
        update: x => Debug.LogFormat("...waiting: {0}", x));
    Debug.Log("After start waiting");

    // The output in the logs will be:
    // Before start waiting
    // ...waiting: 0
    // After start waiting
    // ...waiting: 1
    // ...waiting: 2
    // ...waiting: 3
    // ...waiting: 4
    // Failure!
  }
}
#endregion

#region CallOnTimeout
class AsyncCall_CallOnTimeout : MonoBehaviour {
  void Update() {
    AsyncCall.CallOnTimeout(this, 5.0f, () => Debug.Log("5 seconds has elapsed"));
  }
}
#endregion

#region AsyncWaitForPhysics
class AsyncCall_AsyncWaitForPhysics : MonoBehaviour {
  void Awake() {
    StartCoroutine(MyDelayedFn());
  }
  IEnumerator MyDelayedFn() {
    Debug.Log("Started!");
    // Do some stuff...
    yield return AsyncCall.AsyncWaitForPhysics(
       10,
       () => false,
       update: frame => Debug.LogFormat("...waiting frame {0}...", frame));
    // Do some more stuff after the wait is done...
    Debug.Log("Ended!");
  }
}
#endregion
}  // namespace
