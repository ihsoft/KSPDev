// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using System.Collections.Generic;
using System.Linq;

namespace KSPDev.LogcConsole {

/// <summary>A log capturer that collapses last repeated records into one.</summary>
[PersistentFieldsFileAttribute("KSPDev/KSPDev.settings", "CollapseLogAggregator")]
internal sealed class CollapseLogAggregator : BaseLogAggregator {
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
    if (logRecords.Any()
        && logRecords.Last().GetSimilarityHash() == logRecord.GetSimilarityHash()) {
      logRecords.Last().MergeRepeated(logRecord);
    } else {
      logRecords.AddLast(new LogRecord(logRecord));
      UpdateLogCounter(logRecord, 1);
    }
  }
}

} // namespace KSPDev
