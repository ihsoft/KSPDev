// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections;
using UnityEngine;

namespace KSPDev.ProcessingUtils {

/// <summary>Set of tools to execute a delayed code.</summary>
/// <remarks>
/// Use these tools when code needs to be executed with some delay or at a specific moment of the
/// game.
/// </remarks>
public static class AsyncCall {
  /// <summary>Delays execution of the delegate till the end of the current frame.</summary>
  /// <remarks>
  /// Caller can continue executing its logic. The delegate will be called at the end of
  /// the frame via Unity StartCoroutine mechanism. The delegate will be called only once.
  /// </remarks>
  /// <param name="mono">
  /// Unity object to run coroutine on. If this object dies then the async call will not be invoked.
  /// </param>
  /// <param name="action">Delegate to execute.</param>
  /// <returns>Coroutine instance.</returns>
  /// <example>
  /// <code><![CDATA[
  /// class MyComponent : MonoBehaviour {
  ///   void Update() {
  ///     // Call the method at the end of the current frame.
  ///     AsyncCall.CallOnEndOfFrame(this, () => Debug.LogFormat("Async call!"));
  ///   }
  /// }
  /// ]]></code>
  /// </example>
  /// <seealso href="https://docs.unity3d.com/Manual/Coroutines.html">Unity 3D: Coroutines</seealso>
  /// <seealso href="https://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html">
  /// Unity 3D: WaitForEndOfFrame</seealso>
  public static Coroutine CallOnEndOfFrame(MonoBehaviour mono, Action action) {
    return mono.StartCoroutine(WaitForEndOfFrameCoroutine(action));
  }

  /// <summary>Delays execution of the delegate for the specified amount of time.</summary>
  /// <remarks>
  /// Caller can continue executing its logic. The delegate will be called once the timeout is
  /// expired via Unity StartCoroutine mechanism. The delegate will be called only once.
  /// <para>Using returned instance caller may cancel the call before the timeout expired.</para>
  /// </remarks>
  /// <param name="mono">
  /// Unity object to run coroutine on. If this object dies then the async call will not be invoked.
  /// </param>
  /// <param name="seconds">Timeout in seconds.</param>
  /// <param name="action">Delegate to execute.</param>
  /// <returns>Coroutine instance.</returns>
  /// <example>
  /// <code><![CDATA[
  /// class MyComponent : MonoBehaviour {
  ///   void Update() {
  ///     // Call the method at the end of the current frame.
  ///     AsyncCall.CallOnTimeout(this, 5.0f, () => Debug.LogFormat("Async call!"));
  ///   }
  /// }
  /// ]]></code>
  /// </example>
  /// <seealso href="https://docs.unity3d.com/Manual/Coroutines.html">Unity 3D: Coroutines</seealso>
  /// <seealso href="https://docs.unity3d.com/ScriptReference/WaitForSeconds.html">
  /// Unity 3D: WaitForSeconds</seealso>
  public static Coroutine CallOnTimeout(MonoBehaviour mono, float seconds, Action action) {
    return mono.StartCoroutine(WaitForSecondsCoroutine(seconds, action));
  }

  /// <summary>Delays execution of the delegate till the next fixed update.</summary>
  /// <remarks>
  /// Caller can continue executing its logic. The delegate will be called at the beginning of the
  /// next fixed (physics) update via Unity StartCoroutine mechanism. The delegate will be called
  /// only once.
  /// </remarks>
  /// <param name="mono">
  /// Unity object to run coroutine on. If this object dies then the async call will not be invoked.
  /// </param>
  /// <param name="action">Delegate to execute.</param>
  /// <returns>Coroutine instance.</returns>
  /// <example>
  /// <code><![CDATA[
  /// class MyComponent : MonoBehaviour {
  ///   void Update() {
  ///     // Call the method at the end of the current frame.
  ///     AsyncCall.CallOnFixedUpdate(this, () => Debug.LogFormat("Async call!"));
  ///   }
  /// }
  /// ]]></code>
  /// </example>
  /// <seealso href="https://docs.unity3d.com/Manual/Coroutines.html">Unity 3D: Coroutines</seealso>
  /// <seealso href="https://docs.unity3d.com/ScriptReference/WaitForFixedUpdate.html">
  /// Unity 3D: WaitForFixedUpdate</seealso>
  public static Coroutine CallOnFixedUpdate(MonoBehaviour mono, Action action) {
    return mono.StartCoroutine(WaitForFixedUpdateCoroutine(action));
  }

  /// <summary>
  /// Delays execution until the specified condition is reached. Waiting is limited by the specified
  /// number of fixed frame updates.
  /// </summary>
  /// <remarks>
  /// Can be used when a particular state of the game is required to perform an action. Method
  /// provides ability to define for how long to wait, what to do while waiting, and what to execute
  /// when target state is reached or missed.
  /// </remarks>
  /// <param name="mono">
  /// Unity object to run coroutine on. If this object dies then waiting will be aborted without
  /// calling any callbacks.
  /// </param>
  /// <param name="maxFrames">Number of fixed frame updates to wait before giving up.</param>
  /// <param name="waitUntilFn">
  /// State checking function. It should return <c>true</c> once target state is reached. The very
  /// first execution happens immediately, <i>before</i> exiting from the method. If this
  /// this execution returned <c>true</c> then the successful callback is also called immediately. 
  /// </param>
  /// <param name="success">Callback to execute when state has been successfully reached.</param>
  /// <param name="failure">
  /// Callabck to execute when state has not been reached before frame update limit is exhausted.
  /// </param>
  /// <param name="update">
  /// Callback to execute every fixed frame update while waiting. This callabck will be called at
  /// least once, and teh first call happens immediately. The argument tells how many frames the
  /// method was waiting so far. For the very first call it's, obviously, zero.
  /// </param>
  /// <param name="traceUpdates">When <c>true</c> every wiating cycle will be logged.</param>
  /// <returns>Enumerator that can be used as coroutine target.</returns>
  /// <example>
  /// <code><![CDATA[
  /// class MyComponent : MonoBehaviour {
  ///   void Awake() {
  ///     var count = 5;
  ///     Debug.Log("Before start waiting");
  ///     AsyncCall.WaitForPhysics(
  ///         this, 10,
  ///         () => --count == 0,
  ///         success: () => Debug.Log("Success!"),
  ///         failure: () => Debug.Log("Failure!"),
  ///         update: x => Debug.LogFormat("...waiting: {0}", x));
  ///     Debug.Log("After start waiting");
  ///   }
  /// }
  /// ]]></code>
  /// <para>
  /// This example will print the following:
  /// </para>
  /// <code><![CDATA[
  /// // Before start waiting
  /// // ...waiting: 0
  /// // After start waiting
  /// // ...waiting: 1
  /// // ...waiting: 2
  /// // ...waiting: 3
  /// // ...waiting: 4
  /// // Success!
  /// ]]></code>
  /// <para>If you adjust <c>count</c> to <c>11</c> then the last message will be "Failure!".</para>
  /// </example>
  /// <seealso cref="AsyncWaitForPhysics"/>
  /// <seealso href="https://docs.unity3d.com/Manual/Coroutines.html">Unity 3D: Coroutines</seealso>
  /// <seealso href="https://docs.unity3d.com/ScriptReference/WaitForFixedUpdate.html">
  /// Unity 3D: WaitForFixedUpdate</seealso>
  public static Coroutine WaitForPhysics(
      MonoBehaviour mono,  int maxFrames,
      Func<bool> waitUntilFn,
      Action success = null,
      Action failure = null,
      Action<int> update = null,
      bool traceUpdates = false) {
    return mono.StartCoroutine(
        AsyncWaitForPhysics(maxFrames, waitUntilFn, success, failure, update, traceUpdates));
  }

  /// <summary>Async version of <see cref="WaitForPhysics"/>.</summary>
  /// <param name="maxFrames">Number of fixed frame updates to wait before giving up.</param>
  /// <param name="waitUntilFn">
  /// State checking function. It should return <c>true</c> once target state is reached. The very
  /// first execution happens immediately, <i>before</i> exiting from the method. If this
  /// this execution returned <c>true</c> then the successful callback is also called immediately. 
  /// </param>
  /// <param name="success">Callback to execute when state has been successfully reached.</param>
  /// <param name="failure">
  /// Callabck to execute when state has not been reached before frame update limit is exhausted.
  /// </param>
  /// <param name="update">
  /// Callback to execute every fixed frame update while waiting. This callabck will be called at
  /// least once, and teh first call happens immediately. The argument tells how many frames the
  /// method was waiting so far. For the very first call it's, obviously, zero.
  /// </param>
  /// <param name="traceUpdates">When <c>true</c> every wiating cycle will be logged.</param>
  /// <returns>Enumerator that can be used as a coroutine target.</returns>
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
                                                Action<int> update = null,
                                                bool traceUpdates = false) {
    bool res = false;
    for (var i = 0; i < maxFrames; i++) {
      if (update != null) {
        update(i);
      }
      res = waitUntilFn();
      if (traceUpdates) {
        Debug.LogFormat("Waiting for physics: frame={0}, condition={1}", i, res);
      }
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
  static IEnumerator WaitForEndOfFrameCoroutine(Action action) {
    yield return new WaitForEndOfFrame();
    action();
  }

  static IEnumerator WaitForSecondsCoroutine(float seconds, Action action) {
    yield return new WaitForSeconds(seconds);
    action();
  }

  static IEnumerator WaitForFixedUpdateCoroutine(Action action) {
    yield return new WaitForFixedUpdate();
    action();
  }
  #endregion
}

}  // namespace
