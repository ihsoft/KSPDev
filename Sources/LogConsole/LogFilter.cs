// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Linq;
using KSPDev.LogUtils;
using KSPDev.ConfigUtils;

namespace KSPDev.LogcConsole {

/// <summary>Keeps and controls filters to apply to the incoming logs.</summary>
[PersistentFieldsFileAttribute("KSPDev/settings.cfg", "LogFilter")]
internal static class LogFilter {
  /// <summary>Sources that starts from any of the strings in the filter will be ingored.</summary>
  /// <remarks>Walking thru this filter requires full scan (in a worst case) so, it should be of a
  /// reasonable size.</remarks>
  [PersistentField("PrefixMatchFilter/sourcePrefix", isCollection = true)]
  public static List<string> prefixFilter = new List<string>();
  
  /// <summary>Sources that exactly matches the filter will be ignored.</summary>
  [PersistentField("ExactMatchFilter/source", isCollection = true)]
  public static HashSet<string> exactFilter = new HashSet<string>();

  /// <summary>Adds a new filter by exact match of the source.</summary>
  /// <param name="source"></param>
  public static void AddSilenceBySource(string source) {
    if (!exactFilter.Contains(source)) {
      exactFilter.Add(source);
      Logger.logWarning("Added exact match silence: {0}", source);
    }
  }

  /// <summary>Adds a new filter by preifx match of the source.</summary>
  /// <param name="prefix">A prefix to match for.</param>
  public static void AddSilenceByPrefix(string prefix) {
    if (!prefixFilter.Contains(prefix)) {
      prefixFilter.Add(prefix);
      Logger.logWarning("Added prefix match silence: {0}", prefix);
    }
  }
  
  /// <summary>Verifies if <paramref name="log"/> macthes the filters.</summary>
  /// <param name="log">A log record to check.</param>
  /// <returns><c>true</c> if any of the filters matched.</returns>
  public static bool CheckLogForFilter(LogInterceptor.Log log) {
    return exactFilter.Contains(log.source) || prefixFilter.Any(log.source.StartsWith);
  }
}

} // namespace KSPDev
