// KSP Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.LogUtils;
using StackTrace = System.Diagnostics.StackTrace;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace KSPDev {
  
/// <summary>An alternative log processor that allows better logs handling.</summary>
/// <remarks>Keep it static!</remarks>
public static class LogInterceptor {

  /// <summary>A basic container for the incoming logs. Immutable.</summary>
  public class Log {
    public readonly int id;
    public readonly DateTime timestamp;
    public readonly string message;
    public readonly string stackTrace;
    public readonly string source;
    public readonly LogType type;
    
    internal Log(int id, DateTime timestamp, string message,
                 string stackTrace, string source, LogType type) {
      this.id = id;
      this.timestamp = timestamp;
      this.message = message;
      this.stackTrace = stackTrace;
      this.source = source;
      this.type = type;
    }
  }

  /// <summary>Maximum number of lines to keep in memory.</summary>
  /// <remarks>Setting to a lower setting doesn't have immediate effect. It's undefined when
  /// excessive log records get cleaned up from <see cref="logs"/>.</remarks>
  /// TODO: read from config.
  public static int maxLogLines = 1000;

  /// <summary>Intercepting mode. When disabled all logs go to the system.</summary>
  public static bool isStarted {
    get { return _isStarted; }
  }
  private static bool _isStarted = false;

  /// <summary>Shifts stack trace forward by the exact source match.</summary>
  /// <remarks>Use this filter to skip well-known methods that wrap logging. Due to hash-match this
  /// set can be reasonable big without significant impact to the application performance.</remarks>
  /// TODO: Read from config. 
  public static readonly HashSet<string> exactMatchOverride = new HashSet<string>() {
      "UnityEngine.Application.CallLogCallback",  // Unity debug log handler.
      "UnityEngine.MonoBehaviour.print",  // Unity std I/O method.
      // KAC logging core.
      "KSPPluginFramework.MonoBehaviourExtended.LogFormatted",
      "TWP_KACWrapper.KACWrapper.LogFormatted",
      "KAC_KERWrapper.KERWrapper.LogFormatted",
      "KAC_VOIDWrapper.VOIDWrapper.LogFormatted",
      // SCANsat logging core.
      "SCANsat.SCANUtil.SCANlog",
      "SCANsat.SCANmainMenuLoader.debugWriter",
      // KAS logging core.
      "KAS.KAS_Shared.DebugLog",
      "KAS.KAS_Shared.DebugError",
      // Infernal robotics logging core.
      "InfernalRobotics.Logger.Log",
      // KER logging core.
      "KerbalEngineer.Logger.Flush",
      // AVC logging core.
      "MiniAVC.Logger.Flush",
  };

  /// <summary>
  /// Skips all the matched prefixes up in the stack trace until a non-macthing one is found.
  /// </summary>
  /// <remarks>Use this filter when logging is wrapped by a distinct module that may emit logging
  /// from different methods. This filter is handled via "full scan" approach so, having it too big
  /// may result in a degraded application performance.</remarks>
  public static readonly List<string> prefixMatchOverride = new List<string>() {
      "UnityEngine.Debug.",  // Unity debug logs wrapper.
      "KSPDev.LogUtils.Logger.",  // Own KSPDev logging methods.
      // TODO: Deprecate once KIS is migrated into KSPDev package.
      "KSPDev.DevLogger.",  // Legacy version: Own KSPDev logging methods.
  };

  /// <summary>Latest log records.</summary>
  /// <remarks>List contains at maximum <see cref="maxLogLines"/> records.</remarks>
  private static readonly Queue<Log> logs = new Queue<Log>(maxLogLines);

  /// <summary>A callback type for the log listeners.</summary>
  /// <param name="log">An immutable KSPDev log record.</param>
  public delegate void PreviewCallback(Log log);
  private static HashSet<PreviewCallback> previewCallbacks = new HashSet<PreviewCallback>();

  /// <summary>A  collection to accumulate callbacks that throw errors.</summary>
  /// <remarks>A preview callback that throws an exception is unregistered immideately to minimize
  /// the impact. This collection is used locally only in <see cref="HandleLog"/> but to save
  /// performance it's created statically with a reasonable pre-allocated size.</remarks>
  private static List<PreviewCallback> badCallbacks = new List<PreviewCallback>(10);

  private static int lastLogId = 1;

  /// <summary>Installs interceptor callback and disables system debug log.</summary>
  public static void StartIntercepting() {
    if (_isStarted) {
      return;  // NOOP if already started.
    }
    Logger.logWarning("Debug output intercepted by KSPDev. Open its UI to see the logs"
                      + " (it usually opens with a 'backquote' hotkey)");
    _isStarted = true;
    Application.RegisterLogCallback(HandleLog);
    Logger.logWarning("Debug log transferred from system to the KSPDev");
  }

  /// <summary>Removes log interceptor and allows logs flowing into the system.</summary>
  /// <remarks>Doesn't work properly for KSP since the game won't pickup the logs back.</remarks>
  public static void StopIntercepting() {
    if (!_isStarted) {
      return;  // NOOP if already stopped.
    }
    Logger.logWarning(
        "Debug output returned back to the system. Use system's console to see the logs");
    Application.RegisterLogCallback(null);
    _isStarted = false;
    Logger.logWarning("Debug output returned back from KSPDev to the system");
  }
  
  /// <summary>Registers a callaback that is called on every log record intercepted.</summary>
  /// <remarks>If there are multiple callbacks registered then they are called in an undetermined
  /// order.</remarks>
  /// <param name="previewCallback">A callback to register.</param>
  public static void RegisterPreviewCallback(PreviewCallback previewCallback) {
    previewCallbacks.Add(previewCallback);
  }

  /// <summary>Unregisters log preview callaback.</summary>
  /// <param name="previewCallback">A callback to unregister.</param>
  public static void UnregisterPreviewCallback(PreviewCallback previewCallback) {
    previewCallbacks.Remove(previewCallback);
  }

  /// <summary>Records a log from the log callback.</summary>
  /// <param name="message">Message.</param>
  /// <param name="stackTrace">Trace of where the message came from.</param>
  /// <param name="type">Type of message (error, exception, warning, assert).</param>
  private static void HandleLog(string message, string stackTrace, LogType type) {
    var source = "";

    // Detect source and stack trace for logs other than exceptions. Exceptions are logged from
    // the Unity engine, and the provided stack trace should be used. 
    if (type != LogType.Exception) {
      source = GetSourceAndStackTrace(ref stackTrace);
    }
    var log = new Log(lastLogId++, DateTime.Now, message, stackTrace, source, type);

    // Notify preview handlers. Do an exception check and exclude preview callbacks that cause
    // errors.
    foreach (var callback in previewCallbacks) {
      try {
        callback(log);
      } catch (Exception ex) {
        InternalLog("Preview callback thrown an error and will been unregistered",
                    stackTrace:ex.StackTrace, type:LogType.Exception);
        badCallbacks.Add(callback);
      }
    }
    if (badCallbacks.Count > 0) {
      previewCallbacks.RemoveWhere(badCallbacks.Contains);
      badCallbacks.Clear();
    }
    
    logs.Enqueue(log);
    while (logs.Count > maxLogLines) {
      logs.Dequeue();
    }
  }

  /// <summary>Calculates log source and the related stack trace.</summary>
  /// <remarks>The stack trace grabbed from the current calling point can be really big because it
  /// usually comes from a generic Unity methods, KSP libraries, or an addon debug wrapper modules.
  /// While it's just inconvinient when investigating the logs it's a huge problem when calculating
  /// the "source", a meaningful piece of code that actually did the logging. In normal case it's a
  /// full method name but when logging is wrapped in several helper methods deducting it may become
  /// a problem. This method does checks for exact (<see cref="exactMatchOverride"/>) and prefix
  /// (<see cref="prefixMatchOverride"/>) matches of the source to exclude sources that don't
  /// make sense. Finetuning of the matches is required to have perfectly clear logs.</remarks>
  /// <para>This method assumes it's two levels down in the calling stack from the last Unity's
  /// method (which is <c>UnityEngine.Application.CallLogCallback</c> for now).</para>
  /// <param name="stackTrace">[ref] A string representation of the applicable stack strace.</param>
  /// <returns>A string that identifies a meaningful piece of code that triggered the log.</returns>
  private static string GetSourceAndStackTrace(ref string stackTrace) {
    StackTrace st = null;
    string source = "";

    int skipFrames = 2;  // +1 for calling from HandleLogs(), +1 for Unity last method.
    while (true) {
      st = new StackTrace(skipFrames, true);
      if (st.FrameCount == 0) {
        if (skipFrames == 2) {
          // If filters haven't affected frame count and still it's zero then it's a rare situation
          // of stack overflow error. Just give the best we can as stack trace but set source to
          // UNKNOWN.
          stackTrace = new StackTrace(true).ToString();
          return "UNKNOWN";
        }
        // Fallback in a case of weird filters endining up in filtering everything out.
        st = new StackTrace(true);
        stackTrace = st.ToString();
        InternalLog("Stack trace is exhausted during filters processing. Use original.");
        return MakeSourceFromFrame(st.GetFrame(0));
      }
      source = MakeSourceFromFrame(st.GetFrame(0));

      // Check if exactly this source is blacklisted.
      if (exactMatchOverride.Contains(source)) {
        ++skipFrames;
        continue;  // Re-run overrides for the new source.
      }

      // Check if the whole namespace prefix needs to be skipped in the trace.
      var prefixFound = false;
      foreach (var prefix in prefixMatchOverride) {
        if (source.StartsWith(prefix)) {
          prefixFound = true;
          ++skipFrames;
          for (var frameNum = 1; frameNum < st.FrameCount; ++frameNum) {
            if (!MakeSourceFromFrame(st.GetFrame(frameNum)).StartsWith(prefix)) {
              break;
            }
            ++skipFrames;
          }
          break;
        }
      }
      if (prefixFound) {
        continue;  // There is a prefix match, re-try all the filters.
      }
      
      // No overrides.
      break;
    }
    
    stackTrace = st.ToString();  // Unity only gives stacktrace for the exceptions.
    return source;
  }
  
  /// <summary>Makes source string from the frame.</summary>
  /// <param name="frame">A stack frame to make string from.</param>
  /// <returns>A source string.</returns>
  private static string MakeSourceFromFrame(System.Diagnostics.StackFrame frame) {
    var chkMethod = frame.GetMethod();
    return chkMethod.DeclaringType + "." + chkMethod.Name;
  }

  /// <summary>A helper method to do logging from the interceptor class.</summary>
  /// <param name="message">A message to show.</param>
  /// <param name="type">Optional type of the log.</param>
  /// <param name="stackTrace">Optional stacktrace. When not specified the current stack is used.
  /// </param>
  private static void InternalLog(string message,
                                  LogType type = LogType.Log, string stackTrace = null) {
    var log = new Log(lastLogId++, DateTime.Now,
                      message, stackTrace ?? new StackTrace(true).ToString(), "KSPDev-Internal",
                      type);
    logs.Enqueue(log);
  }
}

} // namespace KSPDev
