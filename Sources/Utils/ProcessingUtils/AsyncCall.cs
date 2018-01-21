// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.LogUtils;
using System;
using System.Collections;
using UnityEngine;

namespace KSPDev.ProcessingUtils {

/// <summary>Set of tools to execute a delayed code.</summary>
/// <remarks>
/// Use these tools when the code needs to be executed with some delay or at the specific moment
/// of time.
/// </remarks>
public static class AsyncCall {
  /// <summary>Delays execution of the delegate till the end of the frame.</summary>
  /// <remarks>
  /// The delegate will trigger at the end of the selected frame update.
  /// If <paramref name="skipFrames"/> is set to <c>0</c>, then the delegate will be
  /// called at the end of the current frame. Calling this method in the "end of frame" callback
  /// will <i>not</i> schedule the callback on the next frame, the execution will just be placed at
  /// the end of the current frame execution queue. This behavior can be used to execute a logic
  /// that depends on some other delayed logic. In order to schedule the execution on the frame
  /// different from the current, specify the <paramref name="skipFrames"/> parameter.
  /// </remarks>
  /// <param name="mono">
  /// The Unity object to run the coroutine on. If this object dies, then the async call will not be
  /// invoked.
  /// </param>
  /// <param name="action">The delegate to execute.</param>
  /// <param name="skipFrames">The number of frames to skip.</param>
  /// <returns>The coroutine instance.</returns>
  /// <seealso href="https://docs.unity3d.com/Manual/Coroutines.html">Unity 3D: Coroutines</seealso>
  /// <seealso href="https://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html">
  /// Unity 3D: WaitForEndOfFrame</seealso>
  /// <example><code source="Examples/ProcessingUtils/AsyncCall-Examples.cs" region="EndOfFrame"/></example>
  public static Coroutine CallOnEndOfFrame(MonoBehaviour mono, Action action, int skipFrames = 0) {
    return mono.StartCoroutine(WaitForEndOfFrameCoroutine(action, skipFrames));
  }

  /// <summary>Delays execution of the delegate for the specified amount of time.</summary>
  /// <remarks>The delegate will be called when the timeout is expired.</remarks>
  /// <param name="mono">
  /// The Unity object to run the coroutine on. If this object dies, then the async call will not be
  /// invoked.
  /// </param>
  /// <param name="seconds">The timeout in seconds.</param>
  /// <param name="action">The delegate to execute.</param>
  /// <returns>The coroutine instance.</returns>
  /// <seealso href="https://docs.unity3d.com/Manual/Coroutines.html">Unity 3D: Coroutines</seealso>
  /// <seealso href="https://docs.unity3d.com/ScriptReference/WaitForSeconds.html">
  /// Unity 3D: WaitForSeconds</seealso>
  /// <example><code source="Examples/ProcessingUtils/AsyncCall-Examples.cs" region="CallOnTimeout"/></example>
  public static Coroutine CallOnTimeout(MonoBehaviour mono, float seconds, Action action) {
    return mono.StartCoroutine(WaitForSecondsCoroutine(seconds, action));
  }

  /// <summary>Delays execution of the delegate till the next fixed update.</summary>
  /// <remarks>
  /// The delegate will be called during the following fixed (physics) update.
  /// </remarks>
  /// <param name="mono">
  /// The Unity object to run the coroutine on. If this object dies, then the async call will not be
  /// invoked.
  /// </param>
  /// <param name="action">The delegate to execute.</param>
  /// <param name="skipFrames">The number of fixed frames to skip.</param>
  /// <returns>The coroutine instance.</returns>
  /// <seealso href="https://docs.unity3d.com/Manual/Coroutines.html">Unity 3D: Coroutines</seealso>
  /// <seealso href="https://docs.unity3d.com/ScriptReference/WaitForFixedUpdate.html">
  /// Unity 3D: WaitForFixedUpdate</seealso>
  /// <example><code source="Examples/ProcessingUtils/AsyncCall-Examples.cs" region="FixedFrame"/></example>
  public static Coroutine CallOnFixedUpdate(MonoBehaviour mono, Action action, int skipFrames = 0) {
    return mono.StartCoroutine(WaitForFixedUpdateCoroutine(action, skipFrames));
  }

  /// <summary>
  /// Delays execution until the specified condition is reached. Waiting is limited by the specified
  /// number of fixed frame updates.
  /// </summary>
  /// <remarks>
  /// Can be used when the code expects some specific physical state of the game. The method
  /// allows to define for how long to wait, what to do while waiting, and what to execute when
  /// target state is reached or missed.
  /// </remarks>
  /// <param name="mono">
  /// The Unity object to run the coroutine on. If this object dies, then the async call will not be
  /// invoked.
  /// </param>
  /// <param name="maxFrames">The number of fixed frame updates to wait before giving up.</param>
  /// <param name="waitUntilFn">
  /// The state checking function. It should return <c>true</c> once the target state is reached.
  /// The very first execution happens immediately on the method call, <i>before</i> exiting from
  /// the method. If this execution returns <c>true</c>, then the successful callback is also called
  /// immediately.
  /// </param>
  /// <param name="success">
  /// The callback to execute when the state has been successfully reached.
  /// </param>
  /// <param name="failure">
  /// The callabck to execute when the state has not been reached before the frame update limit is
  /// exhausted.
  /// </param>
  /// <param name="update">
  /// The callback to execute every fixed frame update while waiting. This callabck will be called
  /// at least once, and the first call happens immediately. The argument tells how many frames the
  /// method was waiting so far. For the very first call it's <c>0</c>.
  /// </param>
  /// <returns>The coroutine instance.</returns>
  /// <seealso cref="AsyncWaitForPhysics"/>
  /// <seealso href="https://docs.unity3d.com/Manual/Coroutines.html">Unity 3D: Coroutines</seealso>
  /// <seealso href="https://docs.unity3d.com/ScriptReference/WaitForFixedUpdate.html">
  /// Unity 3D: WaitForFixedUpdate</seealso>
  /// <example><code source="Examples/ProcessingUtils/AsyncCall-Examples.cs" region="WaitForPhysics1"/></example>
  /// <example><code source="Examples/ProcessingUtils/AsyncCall-Examples.cs" region="WaitForPhysics2"/></example>
  public static Coroutine WaitForPhysics(
      MonoBehaviour mono,  int maxFrames,
      Func<bool> waitUntilFn,
      Action success = null,
      Action failure = null,
      Action<int> update = null) {
    return mono.StartCoroutine(
        AsyncWaitForPhysics(maxFrames, waitUntilFn, success, failure, update));
  }

  /// <summary>Async version of <see cref="WaitForPhysics"/>.</summary>
  /// <param name="maxFrames">The number of fixed frame updates to wait before giving up.</param>
  /// <param name="waitUntilFn">
  /// The state checking function. It should return <c>true</c> once the target state is reached.
  /// The very first execution happens immediately on the method call, <i>before</i> exiting from
  /// the method. If this execution returns <c>true</c>, then the successful callback is also called
  /// immediately.
  /// </param>
  /// <param name="success">
  /// The callback to execute when the state has been successfully reached.
  /// </param>
  /// <param name="failure">
  /// The callabck to execute when the state has not been reached before the frame update limit is
  /// exhausted.
  /// </param>
  /// <param name="update">
  /// The callback to execute every fixed frame update while waiting. This callabck will be called
  /// at least once, and the first call happens immediately. The argument tells how many frames the
  /// method was waiting so far. For the very first call it's <c>0</c>.
  /// </param>
  /// <returns>The enumerator that can be used as a coroutine target.</returns>
  /// <seealso cref="WaitForPhysics"/>
  /// <example>
  /// This method is useful when synchronous wait is needed within a coroutine. Instead of
  /// implementing own loops just return the waiting enumerator. The code below will log 10 waiting
  /// lines between "Started" and "Ended" records. 
  /// <code><![CDATA[
  /// class MyComponent : MonoBehaviour {
  ///   void Awake() {
  ///     StartCoroutine(MyDelayedFn());
  ///   }
  ///   IEnumerator MyDelayedFn() {
  ///     Debug.Log("Started!");
  ///     yield return AsyncCall.AsyncWaitForPhysics(
  ///        10,
  ///        () => false,
  ///        update: frame => Debug.LogFormat("...waiting frame {0}...", frame));
  ///     Debug.Log("Ended!");
  ///   }
  /// }
  /// ]]></code>
  /// </example>
  /// <seealso cref="WaitForPhysics"/>
  public static IEnumerator AsyncWaitForPhysics(int maxFrames, Func<bool> waitUntilFn,
                                                Action success = null,
                                                Action failure = null,
                                                Action<int> update = null) {
    bool res = false;
    for (var i = 0; i < maxFrames; i++) {
      if (update != null) {
        update(i);
      }
      res = waitUntilFn();
      DebugEx.Fine("Waiting for physics: frame={0}, condition={1}", i, res);
      if (res) {
        break;
      }
      yield return new WaitForFixedUpdate();
    }
    if (res) {
      if (success != null) {
        success();
      }
    } else {
      if (failure != null) {
        failure();
      }
    }
  }

  #region Coroutines
  static IEnumerator WaitForEndOfFrameCoroutine(Action action, int skipFrames) {
    while (skipFrames-- > 0) {
      yield return null;
    }
    yield return new WaitForEndOfFrame();
    action();
  }

  static IEnumerator WaitForSecondsCoroutine(float seconds, Action action) {
    yield return new WaitForSeconds(seconds);
    action();
  }

  static IEnumerator WaitForFixedUpdateCoroutine(Action action, int skipFrames) {
    while (skipFrames-- > 0) {
      yield return new WaitForFixedUpdate();
    }
    yield return new WaitForFixedUpdate();
    action();
  }
  #endregion
}

}  // namespace
