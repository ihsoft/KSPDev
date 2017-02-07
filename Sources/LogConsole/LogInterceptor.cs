// KSP Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public Domain license.

using KSPDev.ConfigUtils;
using StackTrace = System.Diagnostics.StackTrace;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace KSPDev.LogConsole {
  
/// <summary>An alternative log processor that allows better logs handling.</summary>
/// <remarks>Keep it static!</remarks>
[PersistentFieldsFileAttribute("KSPDev/LogConsole/Plugins/PluginData/settings.cfg",
                               "LogInterceptor")]
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

  /// <summary>Specifies if logs interception is allowed.</summary>
  /// <remarks>
  /// If <c>false</c> then calls to <see cref="StartIntercepting"/> will be ignored.
  /// </remarks>
  /// <seealso cref="StartIntercepting"/>
  /// <seealso cref="StopIntercepting"/>
  [PersistentField("enableInterception")]
  static bool enableInterception = true;

  /// <summary>Intercepting mode. When disabled all logs go to the system.</summary>
  public static bool isStarted {
    get { return _isStarted; }
  }
  static bool _isStarted = false;

  /// <summary>Shifts stack trace forward by the exact source match.</summary>
  /// <remarks>
  /// Use this filter to skip well-known methods that wrap logging. Due to hash-match this set can
  /// be reasonable big without significant impact to the application performance.
  /// </remarks>
  [PersistentField("ExactMatchOverride/source", isCollection = true)]
  static HashSet<string> exactMatchOverride = new HashSet<string>();

  /// <summary>
  /// Skips all the matched prefixes up in the stack trace until a non-matching source is found.
  /// </summary>
  /// <remarks>
  /// Use this filter when logging is wrapped by a distinct module that may emit logging from
  /// different methods. This filter is handled via "full scan" approach so, having it too big may
  /// result in a degraded application performance.
  /// </remarks>
  [PersistentField("PrefixMatchOverride/sourcePrefix", isCollection = true)]
  static List<string> prefixMatchOverride = new List<string>();

  /// <summary>Callback type for the log listeners.</summary>
  /// <param name="log">An immutable KSPDev log record.</param>
  public delegate void PreviewCallback(Log log);
  static HashSet<PreviewCallback> previewCallbacks = new HashSet<PreviewCallback>();

  /// <summary>Collection to accumulate callbacks that throw errors.</summary>
  /// <remarks>
  /// A preview callback that throws an exception is unregistered immideately to minimize
  /// the impact. This collection is used locally only in <see cref="HandleLog"/> but to save
  /// performance it's created statically with a reasonable pre-allocated size.</remarks>
  static List<PreviewCallback> badCallbacks = new List<PreviewCallback>(10);

  /// <summary>Unique indentifier of the log record.</summary>
  static int lastLogId = 1;

  /// <summary>Installs interceptor callback and disables system debug log.</summary>
  public static void StartIntercepting() {
    if (!enableInterception || _isStarted) {
      return;  // NOOP if already started or disabled.
    }
    Debug.LogWarning("Debug output intercepted by KSPDev. Open its UI to see the logs"
                     + " (it usually opens with a 'backquote' hotkey)");
    _isStarted = true;
    Application.logMessageReceived += HandleLog;
    Debug.LogWarning("Debug log transferred from system to the KSPDev");
  }

  /// <summary>Removes log interceptor and allows logs flowing into the system.</summary>
  /// <remarks>
  /// Doesn't work properly for KSP 1.0.5 since the game won't pickup the log handler back.
  /// </remarks>
  public static void StopIntercepting() {
    if (!_isStarted) {
      return;  // NOOP if already stopped.
    }
    Debug.LogWarning(
        "Debug output returned back to the system. Use system's console to see the logs");
    Application.logMessageReceived += HandleLog;
    _isStarted = false;
    Debug.LogWarning("Debug output returned back from KSPDev to the system");
  }
  
  /// <summary>Registers a callaback that is called on every log record intercepted.</summary>
  /// <remarks>
  /// If there are multiple callbacks registered then they are called in an undetermined order.
  /// </remarks>
  /// <param name="previewCallback">Callback to register.</param>
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
  static void HandleLog(string message, string stackTrace, LogType type) {
    // Detect source and stack trace for logs other than exceptions. Exceptions are logged from
    // the Unity engine, and the provided stack trace should be used. 
    string source;
    source = type != LogType.Exception
        ? GetSourceAndStackTrace(ref stackTrace)
        : GetSourceAndReshapeStackTrace(ref stackTrace);
    var log = new Log(lastLogId++, DateTime.Now, message, stackTrace, source, type);

    // Notify preview handlers. Do an exception check and exclude preview callbacks that cause
    // errors.
    foreach (var callback in previewCallbacks) {
      try {
        callback(log);
      } catch (Exception) {
        badCallbacks.Add(callback);
      }
    }
    if (badCallbacks.Count > 0) {
      previewCallbacks.RemoveWhere(badCallbacks.Contains);
      Debug.LogErrorFormat("Dropped {0} bad log preview callbacks", badCallbacks.Count);
      badCallbacks.Clear();
    }
  }

  /// <summary>Calculates log source and the related stack trace.</summary>
  /// <remarks>
  /// The stack trace grabbed from the current calling point can be really big because it
  /// usually comes from a generic Unity methods, KSP libraries, or an addon debug wrapper modules.
  /// While it's just inconvinient when investigating the logs it's a huge problem when calculating
  /// the "source", a meaningful piece of code that actually did the logging. In normal case it's a
  /// full method name but when logging is wrapped in several helper methods deducting it may become
  /// a problem. This method does checks for exact (<see cref="exactMatchOverride"/>) and prefix
  /// (<see cref="prefixMatchOverride"/>) matches of the source to exclude sources that don't
  /// make sense. Finetuning of the matches is required to have perfectly clear logs.
  /// </remarks>
  /// <para>
  /// This method assumes it's two levels down in the calling stack from the last Unity's method
  /// (which is <c>UnityEngine.Application.CallLogCallback</c> for now).
  /// </para>
  /// <param name="stackTrace">[ref] A string representation of the applicable stack strace.</param>
  /// <returns>A string that identifies a meaningful piece of code that triggered the log.</returns>
  static string GetSourceAndStackTrace(ref string stackTrace) {
    StackTrace st = null;
    string source = "";

    int skipFrames = 2;  // +1 for calling from HandleLogs(), +1 for Unity last method.
    while (true) {
      st = new StackTrace(skipFrames, true);
      if (st.FrameCount == 0) {
        // Internal errors (like UnityEngine or core C#) can be logged directly thru the callback.
        // In which case stack trace doesn't have any useful information. Return empty stack and
        // "UNKNOWN" source when it happens.
        stackTrace = "<System call>";
        return "UNKNOWN";
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

  /// <summary>Calculates source from the Unity exception stack trace.</summary>
  /// <remarks>Also cutifies stack trace to make it looking more C#ish.</remarks>
  /// <param name="stackTrace">Unity provided stack trace.</param>
  /// <returns>Source extraced from the first line of the trace.</returns>
  static string GetSourceAndReshapeStackTrace(ref string stackTrace) {
    var lines = stackTrace.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
    stackTrace = string.Join("\n", (from line in lines select "   at " + line).ToArray());
    if (lines.Length > 0 && !string.IsNullOrEmpty(lines[0])) {
      var line = lines[0];
      return line.Substring(0, line.IndexOfAny(new[] {' ', '('})).Trim();
    }
    return "";
  }

  /// <summary>Makes source string from the frame.</summary>
  /// <param name="frame">A stack frame to make string from.</param>
  /// <returns>A source string.</returns>
  static string MakeSourceFromFrame(System.Diagnostics.StackFrame frame) {
    var chkMethod = frame.GetMethod();
    return chkMethod.DeclaringType + "." + chkMethod.Name;
  }
}

} // namespace KSPDev
