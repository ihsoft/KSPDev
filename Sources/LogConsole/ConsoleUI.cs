// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public Domain license.

using KSPDev.ConfigUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSPDev.LogConsole {

/// <summary>A console to display Unity's debug logs in-game.</summary>
[KSPAddon(KSPAddon.Startup.EveryScene, false /*once*/)]
[PersistentFieldsFileAttribute("KSPDev/LogConsole/Plugins/PluginData/settings.cfg", "UI")]
[PersistentFieldsFileAttribute("KSPDev/LogConsole/Plugins/PluginData/session.cfg", "UI",
                               ConsoleUI.SessionGroup)]
sealed class ConsoleUI : MonoBehaviour {
  /// <summary>Name of the persistent group to keep session settings in.</summary>
  /// <remarks>
  /// Session keeps current UI and layout settings. They get changed frequently and saved/loaded on
  /// every scene.
  /// </remarks>
  const string SessionGroup = "session";

  #region Session settings
  [PersistentField("showInfo", group = SessionGroup)]
  static bool showInfo = false;

  [PersistentField("showWarning", group = SessionGroup)]
  static bool showWarning = true;

  [PersistentField("showErrors", group = SessionGroup)]
  static bool showError = true;

  [PersistentField("showExceptions", group = SessionGroup)]
  static bool showException = true;

  [PersistentField("logMode", group = SessionGroup)]
  static ShowMode logShowMode = ShowMode.Smart;
  #endregion  

  #region Mod's settings
  [PersistentField("consoleToggleKey")]
  static KeyCode toggleKey = KeyCode.BackQuote;

  [PersistentField("ColorSchema/infoLog")]
  static Color infoLogColor = Color.white;
  
  [PersistentField("ColorSchema/warningLog")]
  static Color warningLogColor = Color.yellow;

  [PersistentField("ColorSchema/errorLog")]
  static Color errorLogColor = Color.red;

  [PersistentField("ColorSchema/exceptionLog")]
  static Color exceptionLogColor = Color.magenta;
  #endregion

  /// <summary>Console window margin on the screen.</summary>
  const int Margin = 20;

  /// <summary>For every UI window Unity needs a unique ID. This is the one.</summary>
  const int WindowId = 19450509;

  /// <summary>Actual screen position of the console window.</summary>
  static Rect windowRect =
      new Rect(Margin, Margin, Screen.width - (Margin * 2), Screen.height - (Margin * 2));

  /// <summary>A title bar location.</summary>
  static Rect titleBarRect = new Rect(0, 0, 10000, 20);

  /// <summary>Mode names.</summary>
  static readonly GUIContent[] logShowingModes = {
      new GUIContent("Raw"),
      new GUIContent("Collapsed"),
      new GUIContent("Smart"),
  };

  /// <summary>Display mode constants. Must match <see cref="logShowingModes"/>.</summary>
  enum ShowMode {
    /// <summary>Simple list of log records.</summary>
    Raw = 0,
    /// <summary>List where identical consecutive records are grouped.</summary>
    Collapsed = 1,
    /// <summary>
    /// List where identical records are grouped globally. If group get updated with a new log
    /// record then its timestamp is updated.
    /// </summary>
    Smart = 2
  }
  
  /// <summary>Log scrool box position.</summary>
  static Vector2 scrollPosition;

  /// <summary>Specifies if debug console is visible.</summary>
  static bool isConsoleVisible = false;

  /// <summary>ID of the curently selected log record.</summary>
  /// <remarks>It shows expanded.</remarks>
  static int selectedLogRecordId = -1;

  /// <summary>Indicates that visible log records should be queried from
  /// <see cref="snapshotLogAggregator"/>.</summary>
  static bool logUpdateIsPaused = false;
  
  /// <summary>Idicates that logs from the current aggergator need to be requeryied.</summary>
  static bool logsViewChanged = false;
  
  /// <summary>A logger that keeps records on th disk.</summary>
  internal static PersistentLogAggregator diskLogAggregator = new PersistentLogAggregator();
  /// <summary>A logger to show when <see cref="ShowMode.Raw"/> is selected.</summary>
  internal static PlainLogAggregator rawLogAggregator = new PlainLogAggregator();
  /// <summary>A logger to show when <see cref="ShowMode.Collapsed"/> is selected.</summary>
  internal static CollapseLogAggregator collapseLogAggregator = new CollapseLogAggregator();
  /// <summary>A logger to show when <see cref="ShowMode.Smart"/> is selected.</summary>
  internal static SmartLogAggregator smartLogAggregator = new SmartLogAggregator();
  /// <summary>A logger to show a static snapshot.</summary>
  static SnapshotLogAggregator snapshotLogAggregator = new SnapshotLogAggregator();

  /// <summary>A snapshot of logs for the current view.</summary>
  static IEnumerable<LogRecord> logsToShow = new LogRecord[0];
  
  /// <summary>Number of INFO records in <see cref="logsToShow"/>.</summary>
  static int infoLogs = 0;
  /// <summary>Number of WARNING records in <see cref="logsToShow"/>.</summary>
  static int warningLogs = 0;
  /// <summary>Number of ERROR records in <see cref="logsToShow"/>.</summary>
  static int errorLogs = 0;
  /// <summary>Number of EXCEPTION records in <see cref="logsToShow"/>.</summary>
  static int exceptionLogs = 0;

  /// <summary>A list of actions to apply at the end of the GUI frame.</summary>
  static readonly GUIUtils.GuiActionsList guiActions = new GUIUtils.GuiActionsList();

  /// <summary>Only loads session settings.</summary>
  void Awake() {
    ConfigAccessor.ReadFieldsInType(typeof(ConsoleUI), this, group: SessionGroup);
  }

  /// <summary>Only stores session settings.</summary>
  void OnDestroy() {
    ConfigAccessor.WriteFieldsFromType(typeof(ConsoleUI), this, group: SessionGroup);
  }

  /// <summary>Only used to capture console toggle key.</summary>
  void Update() {
    if (Input.GetKeyDown(toggleKey)) {
      isConsoleVisible = !isConsoleVisible;
    }
  }

  /// <summary>Actually renders the console window.</summary>
  void OnGUI() {
    if (isConsoleVisible) {
      windowRect = GUILayout.Window(
          WindowId, windowRect, MakeConsoleWindow, "KSPDev Logs Console");
    }
  }

  /// <summary>Shows a window that displays the recorded logs.</summary>
  /// <param name="windowId">Window ID.</param>
  void MakeConsoleWindow(int windowId) {
    // Only logs snapshot when it's safe to change GUI leayout.
    if (guiActions.ExecutePendingGuiActions()) {
      UpdateLogsView(forceUpdate: logUpdateIsPaused);
    }

    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
    var logRecordStyle = new GUIStyle(GUI.skin.box) {
        alignment = TextAnchor.MiddleLeft,
    };
    var minSizeLayout = GUILayout.ExpandWidth(false);

    // Report if log interceptor is not handling logs.
    if (!LogInterceptor.isStarted) {
      GUI.contentColor = GetLogTypeColor(LogType.Error);
      GUILayout.Box("KSPDev is not handling system logs. Open standard in-game debug console to see"
                    + " the current logs", logRecordStyle);
    }

    foreach (var log in logsToShow.Where(LogLevelFilter)) {
      var recordMsg = log.MakeTitle()
          + (selectedLogRecordId == log.srcLog.id ? ":\n" + log.srcLog.stackTrace : "");
      GUI.contentColor = GetLogTypeColor(log.srcLog.type);
      GUILayout.Box(recordMsg, logRecordStyle);

      // Check if log record is selected.
      if (Event.current.type == EventType.MouseDown) {
        Rect logBoxRect = GUILayoutUtility.GetLastRect();
        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
          // Toggle selection.
          var newSelectedId = selectedLogRecordId == log.srcLog.id ? -1 : log.srcLog.id;
          guiActions.Add(() => GuiActionSelectLog(newSelectedId));
        }
      }
      
      // Add source and filter controls when expanded.
      if (selectedLogRecordId == log.srcLog.id && log.srcLog.source.Any()) {
        GUI.contentColor = Color.white;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Silence: source", minSizeLayout);
        if (GUILayout.Button(log.srcLog.source, minSizeLayout)) {
          guiActions.Add(() => GuiActionAddSilence(log.srcLog.source, isPrefix: false));
        }
        var sourceParts = log.srcLog.source.Split('.');
        if (sourceParts.Length > 1) {
          GUILayout.Label("or by prefix", minSizeLayout);
          for (var i = sourceParts.Length - 1; i > 0; --i) {
            var prefix = String.Join(".", sourceParts.Take(i).ToArray()) + '.';
            if (GUILayout.Button(prefix, minSizeLayout)) {
              guiActions.Add(() => GuiActionAddSilence(prefix, isPrefix: true));
            }
          }
        }
        GUILayout.EndHorizontal();
      }
    }
    GUILayout.EndScrollView();

    GUI.contentColor = Color.white;

    // Bottom menu.
    GUILayout.BeginHorizontal();
    
    // Window size/snap.
    if (GUILayout.Button(new GUIContent("\u21d5"), minSizeLayout)) {
      windowRect = new Rect(Margin, Margin,
                            Screen.width - Margin * 2, Screen.height - Margin * 2);
    }
    if (GUILayout.Button(new GUIContent("\u21d1"), minSizeLayout)) {
      windowRect = new Rect(Margin, Margin,
                            Screen.width - Margin * 2, (Screen.height - Margin * 2) / 3);
    }
    if (GUILayout.Button(new GUIContent("\u21d3"), minSizeLayout)) {
      var clientHeight = (Screen.height - 2 * Margin) / 3;
      windowRect = new Rect(Margin, Screen.height - Margin - clientHeight,
                            Screen.width - Margin * 2, clientHeight);
    }

    // Clear logs in the current aggregator.
    if (GUILayout.Button(new GUIContent("Clear"))) {
      guiActions.Add(GuiActionClearLogs);
    }
    
    // Log mode selection. 
    GUI.changed = false;
    var showMode = GUILayout.SelectionGrid(
        (int) logShowMode, logShowingModes, logShowingModes.Length, GUILayout.ExpandWidth(false));
    logsViewChanged |= GUI.changed;
    if (GUI.changed) {
      guiActions.Add(() => GuiActionSetMode(mode: (ShowMode) showMode));
    }

    GUI.changed = false;
    logUpdateIsPaused = GUILayout.Toggle(logUpdateIsPaused, "PAUSED", GUILayout.ExpandWidth(false));
    if (GUI.changed) {
      guiActions.Add(() => GuiActionSetPaused(isPaused: logUpdateIsPaused));
    }
    
    // Draw logs filter by level and refresh logs when filter changes.
    GUI.changed = false;
    showInfo = MakeFormattedToggle(showInfo, infoLogColor, "INFO ({0})", infoLogs);
    showWarning = MakeFormattedToggle(showWarning, warningLogColor, "WARNING ({0})", warningLogs);
    showError = MakeFormattedToggle(showError, errorLogColor, "ERROR ({0})", errorLogs);
    showException =
        MakeFormattedToggle(showException, exceptionLogColor, "EXCEPTION ({0})", exceptionLogs);
    logsViewChanged |= GUI.changed;
    GUILayout.EndHorizontal();

    // Allow the window to be dragged by its title bar.
    GUI.DragWindow(titleBarRect);
  }

  /// <summary>Verifies if level of the log record is needed by the UI.</summary>
  /// <param name="log">A log record to verify.</param>
  /// <returns><c>true</c> if this level is visible.</returns>
  static bool LogLevelFilter(LogRecord log) {
    return log.srcLog.type == LogType.Exception && showException
        || log.srcLog.type == LogType.Error && showError
        || log.srcLog.type == LogType.Warning && showWarning
        || log.srcLog.type == LogType.Log && showInfo;
  }

  /// <summary>Gives a color for the requested log type.</summary>
  /// <param name="type">A log type to get color for.</param>
  /// <returns>A color for the type.</returns>
  static Color GetLogTypeColor(LogType type) {
    switch (type) {
    case LogType.Log: return infoLogColor;
    case LogType.Warning: return warningLogColor;
    case LogType.Error: return errorLogColor;
    case LogType.Exception: return exceptionLogColor;
    }
    return Color.gray;
  }

  /// <summary>Makes a standard toggle GUI element.</summary>
  /// <param name="value">A toggle initial state.</param>
  /// <param name="color">A toggle color foreground.</param>
  /// <param name="fmt">A formatting string for the toggle caption</param>
  /// <param name="args">Arguments for the formatting string.</param>
  /// <returns></returns>
  bool MakeFormattedToggle(bool value, Color color, string fmt, params object[] args) {
    GUI.contentColor = color;
    return GUILayout.Toggle(value, string.Format(fmt, args), GUILayout.ExpandWidth(false));
  }

  /// <summary>Populates <see cref="logsToShow"/> and stats numbers.</summary>
  /// <remarks>Current aggregator is determined from <see cref="logShowMode"/> and
  /// <see cref="logUpdateIsPaused"/></remarks>
  /// <param name="forceUpdate">If <c>false</c> then logs view will only be updated if there were
  /// newly aggregated records in teh current aggregator.</param>
  void UpdateLogsView(bool forceUpdate = false) {
    BaseLogAggregator currentAggregator = GetCurrentAggregator();
    if (currentAggregator.FlushBufferedLogs() || logsViewChanged || forceUpdate) {
      logsToShow = currentAggregator.GetLogRecords();
      infoLogs = currentAggregator.infoLogsCount;
      warningLogs = currentAggregator.warningLogsCount;
      errorLogs = currentAggregator.errorLogsCount;
      exceptionLogs = currentAggregator.exceptionLogsCount;
    }
    logsViewChanged = false;
  }
  
  /// <summary>Returns an aggregator for teh currentkly selected mode.</summary>
  /// <param name="ignorePaused">If <c>true</c> then snapshot aggregator is not considered.</param>
  /// <returns>An aggregator.</returns>
  BaseLogAggregator GetCurrentAggregator(bool ignorePaused = false) {
    BaseLogAggregator currentAggregator = snapshotLogAggregator;
    if (ignorePaused || !logUpdateIsPaused) {
      if (logShowMode == ShowMode.Raw) {
        currentAggregator = rawLogAggregator;
      } else if (logShowMode == ShowMode.Collapsed) {
        currentAggregator = collapseLogAggregator;
      } else {
        currentAggregator = smartLogAggregator;
      }
    }
    return currentAggregator;
  }

  #region GUI action handlers
  void GuiActionSetPaused(bool isPaused) {
    if (isPaused) {
      snapshotLogAggregator.LoadLogs(GetCurrentAggregator(ignorePaused: true));
    }
    logUpdateIsPaused = isPaused;
    logsViewChanged = true;
  }

  void GuiActionClearLogs() {
    GuiActionSetPaused(isPaused: false);
    GetCurrentAggregator().ClearAllLogs();
    logsViewChanged = true;
  }
  
  void GuiActionSelectLog(int newSelectedId) {
    selectedLogRecordId = newSelectedId;
  }
  
  void GuiActionAddSilence(string pattern, bool isPrefix) {
    if (isPrefix) {
      LogFilter.AddSilenceByPrefix(pattern);
    } else {
      LogFilter.AddSilenceBySource(pattern);
    }
    ConfigAccessor.WriteFieldsFromType(typeof(LogFilter), null /* instance */);

    rawLogAggregator.UpdateFilter();
    collapseLogAggregator.UpdateFilter();
    smartLogAggregator.UpdateFilter();
    snapshotLogAggregator.UpdateFilter();
    logsViewChanged = true;
  }
  
  void GuiActionSetMode(ShowMode mode) {
    logShowMode = mode;
    GuiActionSetPaused(isPaused: false);  // New mode invalidates snapshot.
    logsViewChanged = true;
  }
  #endregion
}

/// <summary>Only used to start logs aggregation and load configs.</summary>
[KSPAddon(KSPAddon.Startup.Instantly, true /*once*/)]
sealed class AggregationStarter : MonoBehaviour {
  void Awake() {
    // Read all configs.
    ConfigAccessor.ReadFieldsInType(typeof(ConsoleUI), null /* instance */);
    ConfigAccessor.ReadFieldsInType(typeof(LogInterceptor), null /* instance */);
    ConfigAccessor.ReadFieldsInType(typeof(LogFilter), null /* instance */);
    ConfigAccessor.ReadFieldsInType(
        ConsoleUI.diskLogAggregator.GetType(), ConsoleUI.diskLogAggregator);
    ConfigAccessor.ReadFieldsInType(
        ConsoleUI.rawLogAggregator.GetType(), ConsoleUI.rawLogAggregator);
    ConfigAccessor.ReadFieldsInType(
        ConsoleUI.collapseLogAggregator.GetType(), ConsoleUI.collapseLogAggregator);
    ConfigAccessor.ReadFieldsInType(
        ConsoleUI.smartLogAggregator.GetType(), ConsoleUI.smartLogAggregator);

    // Start all aggregators.
    ConsoleUI.rawLogAggregator.StartCapture();
    ConsoleUI.collapseLogAggregator.StartCapture();
    ConsoleUI.smartLogAggregator.StartCapture();
    ConsoleUI.diskLogAggregator.StartCapture();
    LogInterceptor.StartIntercepting();
  }
}

} // namespace KSPDev
