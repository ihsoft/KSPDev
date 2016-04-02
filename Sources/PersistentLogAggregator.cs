// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.LogUtils;
using KSPDev.ConfigUtils;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System;
using UnityEngine;

namespace KSPDev {

/// <summary>A log capturer that writes logs on disk.</summary>
/// <remarks>
/// <para>Three files are created: <list type="bullet">
/// <item><c>INFO</c> that includes all logs;</item>
/// <item><c>WARNING</c> which captures warnings and errors;</item>
/// <item><c>ERROR</c> for the errors (including exceptions).</item>
/// </list></para>
/// <para>Persistent logging must be explicitly enabled via <c>PersistentLogs-settings.cfg</c>
/// </para>
/// </remarks>
[PersistentFieldsFileAttribute("KSPDev/settings.cfg", "PersistentLog")]
internal sealed class PersistentLogAggregator : BaseLogAggregator {
  [PersistentField("enableLogger")]
  private static bool enableLogger = true;
  
  [PersistentField("logFilesPath")]
  private static string logFilePath = "GameData/KSPDev/logs";
  
  /// <summary>Prefix for every log file name.</summary>
  [PersistentField("logFilePrefix")]
  private static string logFilePrefix = "KSPDev-LOG";
  
  /// <summary>Format of the timestamp in the file.</summary>
  [PersistentField("logTsFormat")]
  private static string logTsFormat = "yyMMdd\\THHmmss";

  /// <summary>Specifies if INFO file should be written.</summary>
  [PersistentField("writeInfoFile")]
  private static bool writeInfoFile = true;

  /// <summary>Specifies if WARNING file should be written.</summary>
  [PersistentField("writeWarningFile")]
  private static bool writeWarningFile = true;

  /// <summary>Specifies if ERROR file should be written.</summary>
  [PersistentField("writeErrorFile")]
  private static bool writeErrorFile = true;

  /// <summary>Specifies if new record should be aggregated and persisted.</summary>
  private bool writeLogsToDisk = false;

  /// <summary>Config file location relative to the KSP folder.</summary>
  private const string ConfigFilePath = "GameData/KSPDev/PersistentLog-settings.cfg";
  
  /// <summary>A writer that gets all the logs.</summary>
  private StreamWriter infoLogWriter;
  
  /// <summary>A writer for <c>WARNING</c>, <c>ERROR</c> and <c>EXCEPTION</c> logs.</summary>
  private StreamWriter warningLogWriter;

  /// <summary>Writer for <c>ERROR</c> and <c>EXCEPTION</c> logs.</summary>
  private StreamWriter errorLogWriter;

  public override IEnumerable<LogRecord> GetLogRecords() {
    return logRecords;  // It's always empty.
  }
  
  public override void ClearAllLogs() {
    // Cannot clear persistent log so, restart the files instead.
    StartLogFiles();
  }
  
  protected override void DropAggregatedLogRecord(LinkedListNode<LogRecord> node) {
    // Do nothing since there is no memory state in the aggregator.
  }

  protected override void AggregateLogRecord(LogRecord logRecord) {
    if (!writeLogsToDisk) {
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
      writeLogsToDisk = false;
      Logger.logException(ex);
      Logger.logError("Persistent log agregator failed to write a record. Logging disabled");
    }
  }

  public override void StartCapture() {
    base.StartCapture();
    StartLogFiles();
    PersistentLogAggregatorFlusher.activeAggregators.Add(this);
    if (writeLogsToDisk) {
      Logger.logInfo("Persistent aggregator started");
    } else {
      Logger.logWarning("Persistent aggregator disabled");
    }
  }

  public override void StopCapture() {
    Logger.logInfo("Stopping a persistent aggregator...");
    base.StopCapture();
    StopLogFiles();
    PersistentLogAggregatorFlusher.activeAggregators.Remove(this);
  }

  public override bool FlushBufferedLogs() {
    // Flushes accumulated logs to disk. In case of disk error the logging is disabled.
    var res = base.FlushBufferedLogs();
    if (res && writeLogsToDisk) {
      try {
        if (infoLogWriter != null) {
          infoLogWriter.Flush();
        }
        if (warningLogWriter != null) {
          warningLogWriter.Flush();
        }
        if (errorLogWriter != null) {
          errorLogWriter.Flush();
        }
      } catch (Exception ex) {
        writeLogsToDisk = false;  // Must be the first statement in the catch section!
        Logger.logException(ex);
        Logger.logError("Something went wrong when flushing data to disk. Disabling logging.");
      }
    }
    return res;
  }

  protected override bool CheckIsFiltered(LogInterceptor.Log log) {
    return false;  // Persist any log!
  }

  /// <summary>Creates new logs files and redirects logs to there.</summary>
  private void StartLogFiles() {
    StopLogFiles();  // In case something was opened.
    try {
      if (enableLogger) {
        if (logFilePath.Length > 0) {
          Directory.CreateDirectory(logFilePath);
        }
        var tsSuffix = DateTime.Now.ToString(logTsFormat);
        if (writeInfoFile) {
          infoLogWriter = new StreamWriter(Path.Combine(
              logFilePath, String.Format("{0}.{1}.INFO.txt", logFilePrefix, tsSuffix)));
        }
        if (writeWarningFile) {
          warningLogWriter = new StreamWriter(Path.Combine(
              logFilePath, String.Format("{0}.{1}.WARNING.txt", logFilePrefix, tsSuffix)));
        }
        if (writeErrorFile) {
          errorLogWriter = new StreamWriter(Path.Combine(
              logFilePath, String.Format("{0}.{1}.ERROR.txt", logFilePrefix, tsSuffix)));
        }
      }
      writeLogsToDisk = infoLogWriter != null || warningLogWriter != null || errorLogWriter != null;
    } catch (Exception ex) {
      writeLogsToDisk = false;  // Must be the first statement in the catch section!
      Logger.logException(ex);
      Logger.logError("Not enabling logging to disk due to errors");
    }
  }

  /// <summary>Flushes and closes all opened log files.</summary>
  private void StopLogFiles() {
    try {
      if (infoLogWriter != null) {
        infoLogWriter.Close();
      }
      if (warningLogWriter != null) {
        warningLogWriter.Close();
      }
      if (errorLogWriter != null) {
        errorLogWriter.Close();
      }
    } catch (Exception ex) {
      Logger.logException(ex);
    }
    infoLogWriter = null;
    warningLogWriter = null;
    errorLogWriter = null;
    writeLogsToDisk = false;
  }
}

/// <summary>A helper class to periodically flush logs to disk.</summary>
/// <remarks>Also, does flush on scene change or game exit.</remarks>
[KSPAddon(KSPAddon.Startup.EveryScene, false /*once*/)]
internal sealed class PersistentLogAggregatorFlusher : MonoBehaviour {
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
  /// <remarks>This method never returns.</remarks>
  /// <returns>Delay till next flush.</returns>
  private IEnumerator FlushLogsCoroutine() {
    while (true) {
      yield return new WaitForSeconds(persistentLogsFlushPeriod);
      FlushAllAggregators();
    }
  }
}

} // namespace KSPDev
