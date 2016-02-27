// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com a.k.a. "ihsoft"
// This software is distributed under Public domain license.

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using StackTrace = System.Diagnostics.StackTrace;
using Logger = KSPDev.LogUtils.Logger;

namespace KSPDev {

/// <summary>A log capturer that writes logs on disk.</summary>
/// <remarks>
/// <para>Three files are created: <c>INFO</c> that includes all logs, <c>WARNING</c> which captures
/// warnings and errors, and <c>ERROR</c> for the errors (including exceptions).</para>
/// <para>Persistent logging must be explicitly enabled via <c>PersistentLogs-settings.cfg</c>
/// </para>
/// </remarks>
internal class PersistentLogAggregator : BaseLogAggregator {
  private bool logEnabled = false;
  
  // FIXME: Rename, cutify, etc.
  private const string logfilePath = "GameData/KSPDev/logs";
  private const string logfilePrefix = "KSPDev-LOG-PERS";
  private const string LogTsFormat = "yyMMdd\\THHmmss";
  
  private const string ConfigFilePath = "GameData/KSPDev/PersistentLog-settings.cfg";
  private const string ConfigGeneralNode = "GeneralSettings";
  private const string ConfigGeneralEnabled = "enabled";
  private const string ConfigGeneralPath = "logRelativePath";
  
  private StreamWriter infoLogWriter;
  private StreamWriter warningLogWriter;
  private StreamWriter errorLogWriter;

  public override IEnumerable<LogRecord> GetLogRecords() {
    return logRecords;  // It's always empty.
  }
  
  public override void ClearAllLogs() {
    // Cannot clear persistent log so, restart log files instead.
    StartLogFiles();
  }
  
  protected override void DropAggregatedLogRecord(LinkedListNode<LogRecord> node) {
    // No memory state so, do nothing.
  }

  protected override void AggregateLogRecord(LogRecord logRecord) {
    if (!logEnabled) {
      return;
    }
    var message = logRecord.MakeTitle();
    var type = logRecord.srcLog.type;
    if (type == LogType.Exception && logRecord.srcLog.stackTrace.Length > 0) {
      message += "\n" + logRecord.srcLog.stackTrace;
    }
    try {
      if (infoLogWriter != null) {
        infoLogWriter.WriteLine(message);
      }
      if (warningLogWriter != null
          && (type == LogType.Warning || type == LogType.Error || type == LogType.Exception)) {
        warningLogWriter.WriteLine(message);
      }
      if (errorLogWriter != null && (type == LogType.Error || type == LogType.Exception)) {
        errorLogWriter.WriteLine(message);
      }
    } catch (Exception ex) {
      logEnabled = false;
      Logger.logException(ex);
      Logger.logError("Persistent log agregator failed to write a record. Logging disabled");
    }
  }

  public override void StartCapture() {
    FlushBufferedLogs();
    LoadDefaultConfig();
    if (logEnabled) {
      base.StartCapture();
      StartLogFiles();
      PersistentLogAggregatorFlusher.activeAggregators.Add(this);
      Logger.logInfo("Persistent aggregator started");
    }
  }

  public override void StopCapture() {
    Logger.logInfo("Stopping a persistent aggregator...");
    base.StopCapture();
    PersistentLogAggregatorFlusher.activeAggregators.Remove(this);
  }

  public override bool FlushBufferedLogs() {
    // Flushes accumulated logs to disk. In case of disk error the logging is disabled.
    var res = base.FlushBufferedLogs();
    if (res && logEnabled) {
      try {
        infoLogWriter.Flush();
        warningLogWriter.Flush();
        errorLogWriter.Flush();
      } catch (Exception ex) {
        logEnabled = false;  // Must be the first statement in the catch section!
        Logger.logException(ex);
        Logger.logError("Something went wrong when flushing data to disk. Disabling logging.");
      }
    }
    return res;
  }

  protected override bool CheckIsFiltered(LogInterceptor.Log log) {
    return false;  // Persist any log!
  }

  /// <summary>Loads settings from a default file located in KSPDev folder.</summary>
  /// <remarks>Descendands can override the behavior to get settinsg from another place.</remarks>
  public virtual void LoadDefaultConfig() {
    var filtersPath = KSPUtil.ApplicationRootPath + ConfigFilePath;
    Logger.logInfo("Loading persistent log state from {0}...", filtersPath);
    ConfigNode node = ConfigNode.Load(filtersPath);
    if (node == null) {
      Logger.logWarning("Nothing found. No logging enabled.");
      return;
    }
    LoadConfigFromNode(node);
  }

  /// <summary>Loads settings from the provided node.</summary>
  /// <remarks>Descendants may override the method to read extra settings but they must call the
  /// base implementation.</remarks>
  /// <param name="node">A node to get settings from.</param>
  protected virtual void LoadConfigFromNode(ConfigNode node) {
    //FIXME: load path and state
    Logger.logWarning("**** loading persistent logs settings");
    logEnabled = true;
  }
  
  /// <summary>Creates new logs files and redirects logs to there.</summary>
  private void StartLogFiles() {
    try {
      if (logfilePath.Length > 0) {
        Directory.CreateDirectory(logfilePath);
      }
      var tsSuffix = DateTime.Now.ToString(LogTsFormat);
      // FIXME: Use constants for formats.
      infoLogWriter = new StreamWriter(
          Path.Combine(logfilePath, String.Format("{0}.{1}.INFO.txt", logfilePrefix, tsSuffix)));
      warningLogWriter = new StreamWriter(
          Path.Combine(logfilePath, String.Format("{0}.{1}.WARNING.txt", logfilePrefix, tsSuffix)));
      errorLogWriter = new StreamWriter(
          Path.Combine(logfilePath, String.Format("{0}.{1}.ERROR.txt", logfilePrefix, tsSuffix)));
      logEnabled = true;
    } catch (Exception ex) {
      logEnabled = false;  // Must be the first statement in the catch section!
      Logger.logException(ex);
      Logger.logError("Not enabling logging to disk due to errors");
    }
  }
}

/// <summary>A helper class to periodically flush logs to disk.</summary>
/// <remarks>Also, does flush on scene change or game exit.</remarks>
[KSPAddon(KSPAddon.Startup.EveryScene, false /*once*/)]
internal class PersistentLogAggregatorFlusher : MonoBehaviour {
  /// <summary>A list of persistent aggergators that need state flushing.</summary>
  public static HashSet<PersistentLogAggregator> activeAggregators =
      new HashSet<PersistentLogAggregator>();

  /// <summary>A delay between flushes.</summary>
  public static float persistentLogsFlushPeriod = 0.2f;  // Seconds.

  void Awake() {
    StartCoroutine(FlushLogsCoroutine());
  }

  void OnDestroy() {
    FlushAllAggregators();
  }

  /// <summary>Flushes all registered persistent aggregators.</summary>
  private static void FlushAllAggregators() {
    var aggregators = activeAggregators.ToArray();
    foreach (var aggregator in aggregators) {
      aggregator.FlushBufferedLogs();
    }
  }
  
  /// <summary>Flushes logs to disk periodically.</summary>
  /// <returns>Delay till next flush.</returns>
  private IEnumerator FlushLogsCoroutine() {
    while (true) {
      yield return new WaitForSeconds(persistentLogsFlushPeriod);
      FlushAllAggregators();
    }
  }
}

} // namespace KSPDev
