// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com a.k.a. "ihsoft"
// This software is distributed under Public domain license.

using System.Collections.Generic;
using System.Linq;

namespace KSPDev {

/// <summary>A log capturer that aggregates logs globally by the content.</summary>
internal class SmartLogAggregator : BaseLogAggregator {
  /// <summary>Log index used by smart logging.</summary>
  private readonly Dictionary<int, LinkedListNode<LogRecord>> logRecordsIndex =
      new Dictionary<int, LinkedListNode<LogRecord>>();
 
  public override IEnumerable<LogRecord> GetLogRecords() {
    return logRecords.Reverse();
  }
  
  public override void ClearAllLogs() {
    logRecords.Clear();
    logRecordsIndex.Clear();
    ResetLogCounters();
  }
  
  protected override void DropAggregatedLogRecord(LinkedListNode<LogRecord> node) {
    logRecords.Remove(node);
    logRecordsIndex.Remove(node.Value.GetHashCode());
    UpdateLogCounter(node.Value, -1);
  }

  protected override void AggregateLogRecord(LogRecord logRecord) {
    LinkedListNode<LogRecord> existingNode;
    if (logRecordsIndex.TryGetValue(logRecord.GetHashCode(), out existingNode)) {
      logRecords.Remove(existingNode);
      existingNode.Value.MergeRepeated(logRecord);
      logRecords.AddLast(existingNode);
    } else {
      var node = logRecords.AddLast(logRecord);
      logRecordsIndex.Add(logRecord.GetHashCode(), node);
      UpdateLogCounter(logRecord, 1);
    }
  }
}

} // namespace KSPDev
