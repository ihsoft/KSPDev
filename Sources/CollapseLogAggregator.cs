// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com a.k.a. "ihsoft"
// This software is distributed under Public domain license.

using System.Collections.Generic;
using System.Linq;

namespace KSPDev {

/// <summary>A log capturer that collapses last repeated records into one.</summary>
internal class CollapseLogAggregator : BaseLogAggregator {

  public override IEnumerable<LogRecord> GetLogRecords() {
    return logRecords.ToArray().Reverse();
  }
  
  public override void ClearAllLogs() {
    logRecords.Clear();
    ResetLogCounters();
  }
  
  protected override void DropAggregatedLogRecord(LinkedListNode<LogRecord> node) {
    logRecords.Remove(node);
    UpdateLogCounter(node.Value, -1);
  }

  protected override void AggregateLogRecord(LogRecord logRecord) {
    if (logRecords.Any() && logRecords.Last().GetHashCode() == logRecord.GetHashCode()) {
      logRecords.Last().MergeRepeated(logRecord);
    } else {
      logRecords.AddLast(new LogRecord(logRecord));
      UpdateLogCounter(logRecord, 1);
    }
  }
}

} // namespace KSPDev
