// KSP Development tools.
// Author: igor.zavoychinskiy@gmail.com a.k.a. "ihsoft"
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
  // TODO: Make them all readonly. Would require a constructor. 
  public struct Log {
    public int id;
    public DateTime timestamp;
    public string message;
    public string stackTrace;
    public string source;
    public LogType type;
  }

  /// <summary>Maximum number of lines to keep in memory.</summary>
  /// <remarks>Setting to a lower setting doesn't have immediate effect. It's undefined when
  /// excessive log records get cleaned up from <seealso cref="logs"/>.</remarks>
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
  /// 
  //FIXME: Drop number of frames. It's always 1.
  public static readonly Dictionary<string, int> exactMatchOverride =
      new Dictionary<string, int>() {
          {"UnityEngine.MonoBehaviour.print", 1},  // Unity std I/O method.
          // KAC logging core.
          {"KSPPluginFramework.MonoBehaviourExtended.LogFormatted", 1},
          {"TWP_KACWrapper.KACWrapper.LogFormatted", 1},
          {"KAC_KERWrapper.KERWrapper.LogFormatted", 1},
          {"KAC_VOIDWrapper.VOIDWrapper.LogFormatted", 1},
          // SCANsat logging core.
          {"SCANsat.SCANUtil.SCANlog", 1},
          {"SCANsat.SCANmainMenuLoader.debugWriter", 1},
          // KAS logging core.
          {"KAS.KAS_Shared.DebugLog", 1},
          {"KAS.KAS_Shared.DebugError", 1},
          // Infernal robotics logging core.
          {"InfernalRobotics.Logger.Log", 1},
          // KER logging core.
          {"KerbalEngineer.Logger.Flush", 1},
          // AVC logging core.
          {"MiniAVC.Logger.Flush", 1},
      };

  /// <summary>
  /// Skips all the matched prefixes up in the stack trace until a non-macthing one is found.
  /// </summary>
  /// <remarks>Use this filter when logging is wrapped by a distinct module that may emit logging
  /// from different methods. This filter is handled via "full scan" approach so, having it too big
  /// may result in a degraded application performance.</remarks>
  public static readonly List<string> prefixMatchOverride = new List<string>() {
      "KSPDev.LogUtils.Logger.",  // Own KSPDev logging methods.
      // FIXME: Deprecate once KIS is migrated into KSPDev package.
      "KSPDev.DevLogger.",  // Legacy version: Own KSPDev logging methods.
  };

  /// <summary>Latest log records.</summary>
  /// <remarks>List contains at maximum <seealso cref="maxLogLines"/> records.</remarks>
  public static readonly Queue<Log> logs = new Queue<Log>(maxLogLines);

  public delegate void PreviewCallback(Log log);
  private static HashSet<PreviewCallback> previewCallbacks = new HashSet<PreviewCallback>();

  /// <summary>
  /// A utility collection to accumulate callbacks that throw errors. 
  /// </summary>
  /// <remarks>A preview callback that throws an exception is unregistered immideately to minimize
  /// the impact. To save performance this collection is made static, and it's pre-allocated size
  /// reasonable.</remarks>
  private static List<PreviewCallback> badCallbacks = new List<PreviewCallback>(10);

  /// <summary>
  /// Number of stack frames between <seealso cref="HandleLog"/> and the logging source code.
  /// </summary>
  /// <remarks>
  /// Autodetected on the first run and then used to properly calculate log recordsource.
  /// </remarks>
  private static int skipStackFrames = -1;

  /// <summary>A pattern string used to detect skipping frames.</summary>
  /// <remarks>Expected that nobody will log this string alone in a normal flow.</remarks>
  private const string SkipStackFramesDetectionStr = "##-KSPDev-##";

  /// <summary>A name of the method that sends frame detection signature.</summary>
  private const string SkipStackFramesDetectionSource = "StartIntercepting";

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
    if (skipStackFrames == -1) {
      // Write a detection signature to figure out stack frame depth.
      Logger.logInfo(SkipStackFramesDetectionStr);
    }
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

    if (skipStackFrames == -1 && message == SkipStackFramesDetectionStr) {
      // Detect stack frame depth to properly skip internal Unity and addons stuff.
      DetectSkipFrames(new StackTrace(true));
      if (skipStackFrames >= 0) {
        return;  // Counter detected sucessfully.
      }
    } else if (type != LogType.Exception) {
      // Detect source and stack trace for logs other than exceptions. Exceptions are logged from
      // the Unity engine, and the provided stack trace should be used. 
      source = GetSourceAndStackTrace(ref stackTrace);
    }

    var log = new Log() {
        id = lastLogId++,
        timestamp = DateTime.Now,
        message = message,
        stackTrace = stackTrace,
        type = type,
        source = source,
    };

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

  /// <summary>Detects a calling depth of <seealso cref="HandleLog"/>.</summary>
  /// <remarks>Knowing this depth is needed to omit unneeded stack trace records.</remarks>
  /// <param name="st">A stack trace to analyze.</param>
  private static void DetectSkipFrames(StackTrace st) {
    for (int i = 0; i < st.FrameCount; ++i) {
      if (st.GetFrame(i).GetMethod().Name == SkipStackFramesDetectionSource) {
        skipStackFrames = i;
        break;
      }
    }
  }

  /// <summary>Calculates log source and the related stack trace.</summary>
  /// <remarks>The stack trace grabbed from the current calling point can be really big because it
  /// usually comes from a generic Unity methods, KSP libraries, or an addon debug wrapper modules.
  /// While it's just inconvinient when investigating the logs it's a huge problem when calculating
  /// the "source", a meaningful piece of code that actually did the logging. In normal case it's a
  /// full method name but when logging is wrapped in several helper methods deducting it may become
  /// a problem. This method does checks for exact (<seealso cref="exactMatchOverride"/>) and prefix
  /// (<seealso cref="prefixMatchOverride"/>) matches of the source to exclude sources that don't
  /// make sense. Finetuning of the matches is required to have perfectly clear logs.</remarks>
  /// <param name="stackTrace">[ref] A string representation of the applicable stack strace.</param>
  /// <returns>A string that identifies a meaningful piece of code that triggered the log.</returns>
  private static string GetSourceAndStackTrace(ref string stackTrace) {
    StackTrace st = null;
    string source = "";

    int skipFramesExtra = 1;  // +1 for calling from HandleLogs().
    while (true) {
      st = new StackTrace(skipStackFrames + skipFramesExtra, true);
      if (st.FrameCount == 0) {
        if (skipFramesExtra == 1) {
          // If filters haven't affected frame count then it's a rare situation of stack overflow
          // error. In such cases stack trace is either not available or not relevant. Report this
          // situation separately.
          stackTrace = "<Unknown>";
          return "SystemError";
        }
        // Fallback in a case of weird filters endining up in filtering everything out.
        st = new StackTrace(skipStackFrames, true);
        stackTrace = st.ToString();
        InternalLog("Stack trace is exhausted during filters processing. Use original.");
        return MakeSourceFromFrame(st.GetFrame(0));
      }
      source = MakeSourceFromFrame(st.GetFrame(0));

      // Check if this source needs a different frame for the source caclculation.
      int skipMoreFrames;
      if (exactMatchOverride.TryGetValue(source, out skipMoreFrames) && skipMoreFrames > 0) {
        skipFramesExtra += skipMoreFrames;
        continue;  // There is an exact match, re-try other exact match filters.
      }

      // Check if the whole namespace prefix needs to be skipped in the trace.
      var prefixFound = false;
      foreach (var prefix in prefixMatchOverride) {
        if (source.StartsWith(prefix)) {
          prefixFound = true;
          ++skipFramesExtra;
          for (var frameNum = 1; frameNum < st.FrameCount; ++frameNum) {
            if (!MakeSourceFromFrame(st.GetFrame(frameNum)).StartsWith(prefix)) {
              break;
            }
            ++skipFramesExtra;
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
    var log = new Log() {
        id = lastLogId++,
        timestamp = DateTime.Now,
        message = message,
        stackTrace = stackTrace ?? new StackTrace(true).ToString(),
        type = type,
        source = "KSPDev-Internal",
    };
    logs.Enqueue(log);
  }
}

/// <summary>Main loader class.</summary>
/// <remarks>The class will be loaded only once and die immediately. Though, all we need is
/// initalization once the game is loaded. After that functionality will be served via static
/// methods.</remarks>
[KSPAddon(KSPAddon.Startup.Instantly, true /*once*/)]
internal class KSPDevLogLoader : MonoBehaviour {
  void Awake() {
    LogInterceptor.StartIntercepting();
  }
}

} // namespace KSPDev
