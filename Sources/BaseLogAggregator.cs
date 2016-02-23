// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com a.k.a. "ihsoft"
// This software is distributed under Public domain license.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace KSPDev {

/// <summary>Base class for any log aggregator.</summary>
public abstract class BaseLogAggregator {
  /// <summary>Defines how many records of each type to keep in memory.</summary>
  // TODO: read it from the config file.
  private const int MaxLogRecords = 1000;
  
  /// <summary>Maximum number of chached (and non-aggregated) records.</summary>
  /// <remarks>Once the limit is reached all the cached records get aggregated via
  /// <seealso cref="AggregateLogRecord"/> method.</remarks>
  /// TODO: Get it from the config.
  private const int RawBufferSize = 1000;

  /// <summary>A live list of the stored logs.</summary>
  /// <remarks>This list constantly updates so, *never* iterate over it! Make a copy and then do
  /// whatever readonly operations are needed. Write operations are only allowed from the specific
  /// methods.</remarks>
  protected LinkedList<LogRecord> logRecords = new LinkedList<LogRecord>();
  
  // Counters for every type of the logs. Descendants are responsible to keep them up to date.
  private int infoLogs = 0;
  private int warningLogs = 0;
  private int errorLogs = 0;
  private int exceptionLogs = 0;

  /// <summary>A buffer to keep unaggregated <seealso cref="LogInterceptor"/> log records.</summary>
  /// <remarks>Call <seealso cref="FlushBufferedLogs"/> before accessing aggregated logs to have up
  /// to date state.</remarks>
  private List<LogInterceptor.Log> rawLogsBuffer = new List<LogInterceptor.Log>();

  /// <summary>Returns aggregated logs.</summary>
  /// <remarks>Implementation decides how exactly <seealso cref="logRecords"/> are returned to the
  /// consumer. Main requirement: it must *NOT* change once returned. I.e. returning a list copy is
  /// highly encouraged.</remarks>
  /// <returns>A list of records.</returns>
  public abstract IEnumerable<LogRecord> GetLogRecords();
  
  /// <summary>Clears all currently aggregated logs.</summary>
  /// <remarks>Must at least clear <seealso cref="logRecords"/> and reset counters.</remarks>
  public abstract void ClearAllLogs();

  /// <summary>Drops an aggregated log in <seealso cref="logRecords"/>.</summary>
  /// <remarks>Called by the parent when it decides a log record must be dropped. Implementation
  /// must obey.</remarks>
  /// <param name="node">A list node to remove.</param>
  protected abstract void DropAggregatedLogRecord(LinkedListNode<LogRecord> node);
  
  /// <summary>Adds a new log record to the aggregation.</summary>
  /// <remarks>Parent calls this method when it wants a record to be counted. It's up to the
  /// implementation what to do with the record.</remarks>
  /// <param name="logRecord">A log from the <seealso cref="LogInterceptor"/>.</param>
  protected abstract void AggregateLogRecord(LogRecord logRecord);
  
  /// <summary>Initiates log capturing by this aggergator.</summary>
  /// <remarks>It's ok to call this method multiple times.</remarks>
  public void StartCapture() {
    LogInterceptor.RegisterPreviewCallback(LogPreview);
  }

  /// <summary>Stops log capturing by this aggergator.</summary>
  public void StopCapture() {
    LogInterceptor.UnregisterPreviewCallback(LogPreview);
    FlushBufferedLogs();
  }
  
  /// <summary>Re-scans aggregated logs applying the current filters.</summary>
  /// <remarks>Call it when settings in <seealso cref="LogFilter"/> has changed, and log records
  /// that matched the new filters need to be removed.</remarks>
  public void UpdateFilter() {
    FlushBufferedLogs();
    LinkedListNode<LogRecord> node = logRecords.First;
    while (node != null) {
      LinkedListNode<LogRecord> removeNode = node;
      node = node.Next;
      if (LogFilter.CheckIsFiltered(removeNode.Value.srcLog)) {
        DropAggregatedLogRecord(removeNode);
      }
    }
  }

  /// <summary>Flushes all unaggregated logs.</summary>
  public void FlushBufferedLogs() {
    if (rawLogsBuffer.Count > 0) {
      // Get a snapshot to not get affected by the updates.
      var rawLogsCopy = rawLogsBuffer.ToArray();
      rawLogsBuffer.Clear();
      foreach (var log in rawLogsCopy) {
        var logRecord = new LogRecord(log);
        AggregateLogRecord(logRecord);
      }
      DropExcessiveRecords();
    }
  }

  /// <summary>Resets all log counters to zero.</summary>
  /// <remarks>If implementation calls this method then all aggregated logs must be cleared as well.
  /// </remarks>
  protected void ResetLogCounters() {
    infoLogs = 0;
    warningLogs = 0;
    errorLogs = 0;
    exceptionLogs = 0;
  }
  
  /// <summary>Updates counters for the log record type.</summary>
  /// <remarks>Implementation must call this method every time when number of record in
  /// <seealso cref="logRecords"/> changes.</remarks>
  /// <param name="logRecord">A log record to get type from.</param>
  /// <param name="delta">Delta to add to the current counter.</param>
  protected void UpdateLogCounter(LogRecord logRecord, int delta) {
    switch (logRecord.srcLog.type) {
      case LogType.Log:
        infoLogs += delta;
        break;
      case LogType.Warning:
        warningLogs += delta;
        break;
      case LogType.Error: 
        errorLogs += delta; 
        break;
      case LogType.Exception: 
        exceptionLogs += delta; 
        break;
    }
  }

  /// <summary>Cleanups extra log records.</summary>
  /// <remarks>Limit of <seealso cref="MaxLogRecords"/> is applied per type.</remarks>
  private void DropExcessiveRecords() {
    if (logRecords.Count > 0) {
      LinkedListNode<LogRecord> node = logRecords.First;
      while (infoLogs > MaxLogRecords || warningLogs > MaxLogRecords
             || errorLogs > MaxLogRecords || exceptionLogs > MaxLogRecords) {
        LinkedListNode<LogRecord> removeNode = node;
        node = node.Next;
        var logType = removeNode.Value.srcLog.type;
        if (logType == LogType.Log && infoLogs > MaxLogRecords
            || logType == LogType.Warning && warningLogs > MaxLogRecords
            || logType == LogType.Error && errorLogs > MaxLogRecords
            || logType == LogType.Exception && exceptionLogs > MaxLogRecords) {
          DropAggregatedLogRecord(removeNode);
        }
      }
    }
  }

  /// <summary>A callback handler for incoming Unity log records.</summary>
  /// <remarks>
  /// <para>The record is only stored if it's not banned by <seealso cref="LogFilter"/>.</para>
  /// <para>The incoming records are buffered in a list, and get aggregated when the buffer is
  /// exhausted. Such apporach saves CPU when no log console UI is presented.</para>
  /// </remarks>
  /// <param name="log">Raw log record.</param>
  private void LogPreview(LogInterceptor.Log log) {
    if (LogFilter.CheckIsFiltered(log)) {
      return;
    }
    // Override unsupported log types.
    if (log.type != LogType.Log && log.type != LogType.Warning
        && log.type != LogType.Error && log.type != LogType.Exception) {
      log.type = LogType.Error;
    }
    rawLogsBuffer.Add(log);
    if (rawLogsBuffer.Count >= RawBufferSize) {
      FlushBufferedLogs();
    }
  }
}

} // namespace KSPDev
