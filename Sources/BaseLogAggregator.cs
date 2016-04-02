// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSPDev {

/// <summary>Base class for any log aggregator.</summary>
public abstract class BaseLogAggregator {
  /// <summary>Defines how many records of each type to keep in <see cref="logRecords"/>.</summary>
  // TODO: read it from the config file.
  private const int MaxLogRecords = 300;
  
  /// <summary>Maximum number of cached (and non-aggregated) records.</summary>
  /// <remarks>Once the limit is reached all the cached records get aggregated via
  /// <see cref="AggregateLogRecord"/> method.</remarks>
  /// TODO: Get it from the config.
  private const int RawBufferSize = 1000;

  /// <summary>A live list of the stored logs.</summary>
  /// <remarks>This list constantly updates so, *never* iterate over it! Make a copy and then do
  /// whatever readonly operations are needed. Write operations are only allowed from the specific
  /// methods.</remarks>
  protected LinkedList<LogRecord> logRecords = new LinkedList<LogRecord>();
  
  /// <summary>A number of INFO logs that this aggregator currently holds.</summary>
  /// <remarks>Also counts anything that is not ERROR, WARNING or EXCEPTION.</remarks>
  public int infoLogsCount {
    get { return _infoLogsCount; }
  }
  private int _infoLogsCount = 0;
  
  /// <summary>A number of WARNING logs that this aggregator currently holds.</summary>
  public int warningLogsCount {
    get { return _warningLogsCount; }
  }
  private int _warningLogsCount = 0;

  /// <summary>A number of ERROR logs that this aggregator currently holds.</summary>
  public int errorLogsCount {
    get { return _errorLogsCount; }
  }
  private int _errorLogsCount = 0;
  
  /// <summary>A number of EXCEPTION logs that this aggregator currently holds.</summary>
  public int exceptionLogsCount {
    get { return _exceptionLogsCount; }
  }
  private int _exceptionLogsCount = 0;

  /// <summary>A buffer to keep unaggregated <see cref="LogInterceptor"/> log records.</summary>
  /// <remarks>Call <see cref="FlushBufferedLogs"/> before accessing aggregated logs to have up
  /// to date state.</remarks>
  private readonly List<LogInterceptor.Log> rawLogsBuffer = new List<LogInterceptor.Log>();

  /// <summary>Returns aggregated logs.</summary>
  /// <remarks>Implementation decides how exactly <see cref="logRecords"/> are returned to the
  /// consumer. Main requirement: the collection must *NOT* change once returned. Returning a
  /// collection copy is highly encouraged.
  /// <para>Note: changing of the items in the collection is acceptable. Deep copy is not required.
  /// </para>
  /// </remarks>
  /// <returns>A list of records.</returns>
  public abstract IEnumerable<LogRecord> GetLogRecords();
  
  /// <summary>Clears all currently aggregated logs.</summary>
  /// <remarks>Must at least clear <see cref="logRecords"/> and reset counters.</remarks>
  public abstract void ClearAllLogs();

  /// <summary>Drops an aggregated log in <see cref="logRecords"/>.</summary>
  /// <remarks>Called by the parent when it decides a log record must be dropped. Implementation
  /// must obey.</remarks>
  /// <param name="node">A list node to remove.</param>
  protected abstract void DropAggregatedLogRecord(LinkedListNode<LogRecord> node);
  
  /// <summary>Adds a new log record to the aggregation.</summary>
  /// <remarks>Parent calls this method when it wants a record to be counted. It's up to the
  /// implementation what to do with the record.</remarks>
  /// <param name="logRecord">A log from the <see cref="LogInterceptor"/>. Do NOT store this
  /// instance! If tjhis log record needs to be stored make a copy via <see cref="LogRecord"/>
  /// constructor.</param>
  protected abstract void AggregateLogRecord(LogRecord logRecord);
  
  /// <summary>Initiates log capturing by this aggergator.</summary>
  /// <remarks>It's ok to call this method multiple times.</remarks>
  public virtual void StartCapture() {
    LogInterceptor.RegisterPreviewCallback(LogPreview);
  }

  /// <summary>Stops log capturing by this aggergator.</summary>
  public virtual void StopCapture() {
    LogInterceptor.UnregisterPreviewCallback(LogPreview);
    FlushBufferedLogs();
  }
  
  /// <summary>Re-scans aggregated logs applying the current filters.</summary>
  /// <remarks>Call it when settings in <see cref="LogFilter"/> has changed, and log records
  /// that matched the new filters need to be removed.</remarks>
  public virtual void UpdateFilter() {
    FlushBufferedLogs();
    LinkedListNode<LogRecord> node = logRecords.First;
    while (node != null) {
      LinkedListNode<LogRecord> removeNode = node;
      node = node.Next;
      if (CheckIsFiltered(removeNode.Value.srcLog)) {
        DropAggregatedLogRecord(removeNode);
      }
    }
  }

  /// <summary>Flushes all unaggregated logs.</summary>
  /// <returns><c>true</c> if there were pending changes.</returns>
  public virtual bool FlushBufferedLogs() {
    bool res = rawLogsBuffer.Count > 0;
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
    return res;
  }

  /// <summary>Verifies if <paramref name="log"/> matches the filters.</summary>
  /// <param name="log">A log record to check.</param>
  /// <returns><c>true</c> if any of the filters matched.</returns>
  protected virtual bool CheckIsFiltered(LogInterceptor.Log log) {
    return LogFilter.CheckLogForFilter(log);
  }

  /// <summary>Resets all log counters to zero.</summary>
  /// <remarks>If implementation calls this method then all aggregated logs must be cleared as well.
  /// </remarks>
  protected void ResetLogCounters() {
    _infoLogsCount = 0;
    _warningLogsCount = 0;
    _errorLogsCount = 0;
    _exceptionLogsCount = 0;
  }
  
  /// <summary>Updates counters for the log record type.</summary>
  /// <remarks>Implementation must call this method every time when number of record in
  /// <see cref="logRecords"/> changes.</remarks>
  /// <param name="logRecord">A log record to get type from.</param>
  /// <param name="delta">Delta to add to the current counter.</param>
  protected void UpdateLogCounter(LogRecord logRecord, int delta) {
    switch (logRecord.srcLog.type) {
      case LogType.Log:
        _infoLogsCount += delta;
        break;
      case LogType.Warning:
        _warningLogsCount += delta;
        break;
      case LogType.Error: 
        _errorLogsCount += delta; 
        break;
      case LogType.Exception: 
        _exceptionLogsCount += delta; 
        break;
    }
  }

  /// <summary>Cleanups extra log records.</summary>
  /// <remarks>Limit of <see cref="MaxLogRecords"/> is applied per type.</remarks>
  private void DropExcessiveRecords() {
    if (logRecords.Count > 0) {
      LinkedListNode<LogRecord> node = logRecords.First;
      while (_infoLogsCount > MaxLogRecords || _warningLogsCount > MaxLogRecords
             || _errorLogsCount > MaxLogRecords || _exceptionLogsCount > MaxLogRecords) {
        LinkedListNode<LogRecord> removeNode = node;
        node = node.Next;
        var logType = removeNode.Value.srcLog.type;
        if (logType == LogType.Log && _infoLogsCount > MaxLogRecords
            || logType == LogType.Warning && _warningLogsCount > MaxLogRecords
            || logType == LogType.Error && _errorLogsCount > MaxLogRecords
            || logType == LogType.Exception && _exceptionLogsCount > MaxLogRecords) {
          DropAggregatedLogRecord(removeNode);
        }
      }
    }
  }

  /// <summary>A callback handler for incoming Unity log records.</summary>
  /// <remarks>
  /// <para>The record is only stored if it's not banned by <see cref="CheckIsFiltered"/>.
  /// </para>
  /// <para>The incoming records are buffered in a list, and get aggregated when the buffer is
  /// exhausted. Such apporach saves CPU when no log console UI is presented.</para>
  /// </remarks>
  /// <param name="log">Raw log record.</param>
  private void LogPreview(LogInterceptor.Log log) {
    if (CheckIsFiltered(log)) {
      return;
    }
    // Override unsupported log types into INFO.
    if (log.type != LogType.Log && log.type != LogType.Warning
        && log.type != LogType.Error && log.type != LogType.Exception) {
      log = new LogInterceptor.Log(
          log.id, log.timestamp, log.message, log.stackTrace, log.source, LogType.Log);
    }
    rawLogsBuffer.Add(log);
    if (rawLogsBuffer.Count >= RawBufferSize) {
      FlushBufferedLogs();
    }
  }
}

} // namespace KSPDev
