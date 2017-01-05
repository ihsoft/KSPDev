// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public Domain license.

using System.Collections.Generic;

namespace KSPDev.LogConsole {

/// <summary>
/// A simple wrapper to hold static logs copy originated from any other aggreator.
/// </summary>
sealed class SnapshotLogAggregator : BaseLogAggregator {
  /// <summary>Makes copies of the log records from <paramref name="srcAggregator"/>.</summary>
  /// <remarks>Does a deep copy of every record.</remarks>
  /// <param name="srcAggregator">An aggregator to get log records from.</param>
  public void LoadLogs(BaseLogAggregator srcAggregator) {
    ClearAllLogs();
    foreach (var log in srcAggregator.GetLogRecords()) {
      AggregateLogRecord(log);
    }
  }

  /// <inheritdoc/>
  public override void StartCapture() {
    // Do nothing since no capturing is needed.
  }

  /// <inheritdoc/>
  public override void StopCapture() {
    // Nothing to stop.
  }

  /// <inheritdoc/>
  public override IEnumerable<LogRecord> GetLogRecords() {
    // Return logs exactly as they were copied from the source. No need to make a copy since this
    // collection is static anyways.
    return logRecords;
  }
  
  /// <inheritdoc/>
  public override void ClearAllLogs() {
    logRecords.Clear();
    ResetLogCounters();
  }
  
  protected override void DropAggregatedLogRecord(LinkedListNode<LogRecord> node) {
    logRecords.Remove(node);
    UpdateLogCounter(node.Value, -1);
  }

  protected override void AggregateLogRecord(LogRecord logRecord) {
    logRecords.AddLast(new LogRecord(logRecord));
    UpdateLogCounter(logRecord, 1);
  }
}

} // namespace KSPDev
