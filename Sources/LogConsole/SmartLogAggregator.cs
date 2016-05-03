// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using System.Collections.Generic;
using System.Linq;

namespace KSPDev.LogcConsole {

/// <summary>A log capturer that aggregates logs globally by the content.</summary>
[PersistentFieldsFileAttribute("KSPDev/KSPDev.settings", "SmartLogAggregator")]
internal sealed class SmartLogAggregator : BaseLogAggregator {
  /// <summary>Log index used by smart logging.</summary>
  private readonly Dictionary<int, LinkedListNode<LogRecord>> logRecordsIndex =
      new Dictionary<int, LinkedListNode<LogRecord>>();
 
  public override IEnumerable<LogRecord> GetLogRecords() {
    return logRecords.ToArray().Reverse();
  }
  
  public override void ClearAllLogs() {
    logRecords.Clear();
    logRecordsIndex.Clear();
    ResetLogCounters();
  }
  
  protected override void DropAggregatedLogRecord(LinkedListNode<LogRecord> node) {
    logRecords.Remove(node);
    logRecordsIndex.Remove(node.Value.GetSimilarityHash());
    UpdateLogCounter(node.Value, -1);
  }

  protected override void AggregateLogRecord(LogRecord logRecord) {
    LinkedListNode<LogRecord> existingNode;
    if (logRecordsIndex.TryGetValue(logRecord.GetSimilarityHash(), out existingNode)) {
      logRecords.Remove(existingNode);
      existingNode.Value.MergeRepeated(logRecord);
      logRecords.AddLast(existingNode);
    } else {
      var node = logRecords.AddLast(new LogRecord(logRecord));
      logRecordsIndex.Add(logRecord.GetSimilarityHash(), node);
      UpdateLogCounter(logRecord, 1);
    }
  }
}

} // namespace KSPDev
