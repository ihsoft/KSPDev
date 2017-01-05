// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public Domain license.

using KSPDev.ConfigUtils;
using System.Collections.Generic;
using System.Linq;

namespace KSPDev.LogConsole {

/// <summary>A log capturer that just accumulates all logs in a plain list.</summary>
[PersistentFieldsFileAttribute("KSPDev/KSPDev.settings", "PlainLogAggregator")]
sealed class PlainLogAggregator : BaseLogAggregator {
  /// <inheritdoc/>
  public override IEnumerable<LogRecord> GetLogRecords() {
    return logRecords.ToArray().Reverse();
  }
  
  /// <inheritdoc/>
  public override void ClearAllLogs() {
    logRecords.Clear();
    ResetLogCounters();
  }
  
  /// <inheritdoc/>
  protected override void DropAggregatedLogRecord(LinkedListNode<LogRecord> node) {
    logRecords.Remove(node);
    UpdateLogCounter(node.Value, -1);
  }

  /// <inheritdoc/>
  protected override void AggregateLogRecord(LogRecord logRecord) {
    logRecords.AddLast(new LogRecord(logRecord));
    UpdateLogCounter(logRecord, 1);
  }
}

} // namespace KSPDev
