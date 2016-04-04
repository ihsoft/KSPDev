// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using System.Collections.Generic;
using System.Linq;

namespace KSPDev.LogcConsole {

/// <summary>A log capturer that just accumulates all logs in a plain list.</summary>
[PersistentFieldsFileAttribute("KSPDev/settings.cfg", "PlainLogAggregator")]
internal sealed class PlainLogAggregator : BaseLogAggregator {
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
    logRecords.AddLast(new LogRecord(logRecord));
    UpdateLogCounter(logRecord, 1);
  }
}

} // namespace KSPDev
