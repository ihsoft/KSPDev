// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com a.k.a. "ihsoft"
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Linq;
using Logger = KSPDev.LogUtils.Logger;

namespace KSPDev {

/// <summary>Keeps and controls filters to apply to the incoming logs.</summary>
[KSPAddon(KSPAddon.Startup.Instantly, true /*once*/)]
internal static class LogFilter {
  /// <summary>Sources that starts from any of the strings in the filter will be ingored.</summary>
  /// <remarks>Walking thru this filter requires full scan (in a worst case) so, it should be of a
  /// reasonable size.</remarks>
  public static List<string> prefixFilter = new List<string>();
  
  /// <summary>Sources that exactly matches the filter will be ignored.</summary>
  public static HashSet<string> exactFilter = new HashSet<string>();

  // Constants for filter settings persitence.
  private const string PrefixMatchFilterNodeName = "PrefixMatchFilter";
  private const string SourcePrefixKeyName = "sourcePrefix";
  private const string ExactMatchFilterNodeName = "ExactMatchFilter";
  private const string SourceKeyName = "source";
  private const string FiltersFilePath = "GameData/KSPDev/LogConsole-filters.cfg";

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
  
  /// <summary>Loads saved filters.</summary>
  public static void LoadFilters() {
    var filtersPath = KSPUtil.ApplicationRootPath + FiltersFilePath;
    Logger.logInfo("Loading filters from {0}...", filtersPath);
    ConfigNode node = ConfigNode.Load(filtersPath);
    if (node == null) {
      return;
    }

    var prefixMatchNode = node.GetNode(PrefixMatchFilterNodeName);
    if (prefixMatchNode != null) {
      var cfgPrefixFilter = prefixMatchNode.GetValues(SourcePrefixKeyName);
      Logger.logInfo("Read prefix matches: {0}", String.Join(", ", cfgPrefixFilter));
      prefixFilter = cfgPrefixFilter.ToList();
    }

    var exactMatchNode = node.GetNode(ExactMatchFilterNodeName);
    if (exactMatchNode != null) {
      var cfgExactFilter = exactMatchNode.GetValues(SourceKeyName);
      Logger.logInfo("Read exact matches: {0}", String.Join(", ", cfgExactFilter));
      exactFilter = new HashSet<string>(cfgExactFilter);
    }
  }

  /// <summary>Saves current filters.</summary>
  public static bool SaveFilters() {
    var node = new ConfigNode();

    ConfigNode prefixMatchNode = node.AddNode(PrefixMatchFilterNodeName);
    foreach (var prefix in prefixFilter.OrderBy(p => p)) {
      prefixMatchNode.AddValue(SourcePrefixKeyName, prefix);
    }
    
    ConfigNode exactMatchNode = node.AddNode(ExactMatchFilterNodeName);
    foreach (var source in exactFilter.OrderBy(s => s)) {
      exactMatchNode.AddValue(SourceKeyName, source);
    }

    try {
      node.Save(KSPUtil.ApplicationRootPath + FiltersFilePath);
    } catch (Exception ex) {
      Logger.logError("Cannot save filter: {0}", ex.Message);
      return false;
    }
    return true;
  }
  
  /// <summary>Verifies if <paramref name="log"/> macthes the filters.</summary>
  /// <param name="log">A log record to check.</param>
  /// <returns><c>true</c> if any of the filters matched.</returns>
  public static bool CheckLogForFilter(LogInterceptor.Log log) {
    return exactFilter.Contains(log.source) || prefixFilter.Any(log.source.StartsWith);
  }
}

} // namespace KSPDev
