// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public Domain license.

using KSPDev.ConfigUtils;
using KSPDev.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSPDev.LogConsole {

/// <summary>A console to display Unity's debug logs in-game.</summary>
[KSPAddon(KSPAddon.Startup.Instantly, true /*once*/)]
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

  [PersistentField("quickFilter", group = SessionGroup)]
  static string quickFilterStr = "";
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

  /// <summary>Style to draw a control of the minimum size.</summary>
  static readonly GUILayoutOption MinSizeLayout = GUILayout.ExpandWidth(false);

  /// <summary>Mode names.</summary>
  static readonly string[] logShowingModes = { "Raw", "Collapsed", "Smart" };

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
  
  /// <summary>Log scroll box position.</summary>
  static Vector2 scrollPosition;

  /// <summary>Specifies if the debug console is visible.</summary>
  static bool isConsoleVisible;

  /// <summary>ID of the curently selected log record.</summary>
  /// <remarks>It shows expanded.</remarks>
  static int selectedLogRecordId = -1;

  /// <summary>Indicates that the visible log records should be queried from a
  /// <see cref="snapshotLogAggregator"/>.</summary>
  static bool logUpdateIsPaused;

  /// <summary>Idicates that the logs from the current aggergator need to be re-queried.</summary>
  static bool logsViewChanged;

  #region Log aggregators
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
  #endregion

  /// <summary>A snapshot of the logs for the current view.</summary>
  static IEnumerable<LogRecord> logsToShow = new LogRecord[0];
  
  /// <summary>Number of the INFO records in the <see cref="logsToShow"/> collection.</summary>
  static int infoLogs = 0;
  /// <summary>Number of the WARNING records in the <see cref="logsToShow"/> collection.</summary>
  static int warningLogs = 0;
  /// <summary>Number of the ERROR records in the <see cref="logsToShow"/> collection.</summary>
  static int errorLogs = 0;
  /// <summary>Number of the EXCEPTION records in the <see cref="logsToShow"/> collection.</summary>
  static int exceptionLogs = 0;

  /// <summary>A list of actions to apply at the end of the GUI frame.</summary>
  static readonly GuiActionsList guiActions = new GuiActionsList();

  /// <summary>Tells if the controls should be shown at the bottom of the dialog.</summary>
  bool isToolbarAtTheBottom = true;

  #region Quick filter fields
  /// <summary>Tells if the quick filter editing is active.</summary>
  /// <remarks>Log console update is freezed until the mode is ended.</remarks>
  static bool quickFilterInputEnabled;

  /// <summary>Tells the last known qick filter status.</summary>
  /// <remarks>It's updated in every <c>OnGUI</c> call. Used to detect the mode change.</remarks>
  static bool oldQuickFilterInputEnabled;

  /// <summary>The old value of the quick feilter before the edit mode has started.</summary>
  static string oldQuickFilterStr;

  /// <summary>The size for the quick filter input field.</summary>
  static readonly GUILayoutOption QuickFilterSizeLayout = GUILayout.Width(100);
  #endregion

  #region Session persistence
  /// <summary>Only loads the session settings.</summary>
  void Awake() {
    UnityEngine.Object.DontDestroyOnLoad(gameObject);

    // Read the configs for all the aggregators.
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

    // Load UI configs.
    ConfigAccessor.ReadFieldsInType(typeof(ConsoleUI), null /* instance */);
    ConfigAccessor.ReadFieldsInType(typeof(ConsoleUI), this, group: SessionGroup);
  }
  
  /// <summary>Only stores the session settings.</summary>
  void OnDestroy() {
    ConfigAccessor.WriteFieldsFromType(typeof(ConsoleUI), this, group: SessionGroup);
  }
  #endregion

  /// <summary>Actually renders the console window.</summary>
  void OnGUI() {
    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == toggleKey) {
      isConsoleVisible = !isConsoleVisible;
      Event.current.Use();
    }
    if (isConsoleVisible) {
      var title = "KSPDev Logs Console";
      if (logUpdateIsPaused) {
        title += " <i>(PAUSED)</i>";
      }
      windowRect = GUILayout.Window(WindowId, windowRect, MakeConsoleWindow, title);
    }
  }

  /// <summary>Shows a window that displays the recorded logs.</summary>
  /// <param name="windowId">Window ID.</param>
  void MakeConsoleWindow(int windowId) {
    // Only show the logs snapshot when it's safe to change the GUI layout.
    if (guiActions.ExecutePendingGuiActions()) {
      UpdateLogsView();
      // Check if the toolbar goes out of the screen.
      isToolbarAtTheBottom = windowRect.yMax < Screen.height;
    }

    if (!isToolbarAtTheBottom) {
      CreateGUIToolbar();
    }

    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
    var logRecordStyle = new GUIStyle(GUI.skin.box) {
        alignment = TextAnchor.MiddleLeft,
    };

    // Report if log interceptor is not handling logs.
    if (!LogInterceptor.isStarted) {
      GUI.contentColor = GetLogTypeColor(LogType.Error);
      GUILayout.Box("KSPDev is not handling system logs. Open standard in-game debug console to see"
                    + " the current logs", logRecordStyle);
    }

    // Report a quick filter state.
    if (quickFilterInputEnabled) {
      ShowStatusText("Logs update is PAUSED due to the quick filter editing is active."
                      + " Hit ENTER to accept the filter, or ESC to discard.");
    }

    // Show the records. Report if there is none.
    var capturedRecords = logsToShow.Where(LogLevelFilter);
    var showRecords = capturedRecords.Where(LogQuickFilter);
    if (!quickFilterInputEnabled && !showRecords.Any()) {
      var msg = "No records available for the selected levels";
      if (capturedRecords.Any()) {
        msg += " and quick filter \"" + quickFilterStr + "\"";
      }
      ShowStatusText(msg);
    }
    foreach (var log in showRecords) {
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
        GUILayout.Label("Silence: source", MinSizeLayout);
        if (GUILayout.Button(log.srcLog.source, MinSizeLayout)) {
          guiActions.Add(() => GuiActionAddSilence(log.srcLog.source, isPrefix: false));
        }
        var sourceParts = log.srcLog.source.Split('.');
        if (sourceParts.Length > 1) {
          GUILayout.Label("or by prefix", MinSizeLayout);
          for (var i = sourceParts.Length - 1; i > 0; --i) {
            var prefix = String.Join(".", sourceParts.Take(i).ToArray()) + '.';
            if (GUILayout.Button(prefix, MinSizeLayout)) {
              guiActions.Add(() => GuiActionAddSilence(prefix, isPrefix: true));
            }
          }
        }
        GUILayout.EndHorizontal();
      }
    }
    GUILayout.EndScrollView();

    if (isToolbarAtTheBottom) {
      CreateGUIToolbar();
    }

    // Allow the window to be dragged by its title bar.
    GuiWindow.DragWindow(ref windowRect, titleBarRect);
  }

  /// <summary>Creates controls for the console.</summary>
  void CreateGUIToolbar() {
    GUI.contentColor = Color.white;
    using (new GUILayout.HorizontalScope()) {
      // Window size/snap.
      if (GUILayout.Button("\u21d5", MinSizeLayout)) {
        windowRect = new Rect(Margin, Margin,
                              Screen.width - Margin * 2, Screen.height - Margin * 2);
      }
      if (GUILayout.Button("\u21d1", MinSizeLayout)) {
        windowRect = new Rect(Margin, Margin,
                              Screen.width - Margin * 2, (Screen.height - Margin * 2) / 3);
      }
      if (GUILayout.Button("\u21d3", MinSizeLayout)) {
        var clientHeight = (Screen.height - 2 * Margin) / 3;
        windowRect = new Rect(Margin, Screen.height - Margin - clientHeight,
                              Screen.width - Margin * 2, clientHeight);
      }

      // Quick filter.
      // Due to Unity GUI behavior, any change to the layout resets the text field focus. We do some
      // tricks here to set initial focus to the field but not locking it permanently.
      GUILayout.Label("Quick filter:", MinSizeLayout);
      if (quickFilterInputEnabled) {
        GUI.SetNextControlName("quickFilter");
        quickFilterStr = GUILayout.TextField(quickFilterStr, QuickFilterSizeLayout);
        if (Event.current.type == EventType.KeyUp) {
          if (Event.current.keyCode == KeyCode.Return) {
            guiActions.Add(GuiActionAcceptQuickFilter);
          } else if (Event.current.keyCode == KeyCode.Escape) {
            guiActions.Add(GuiActionCancelQuickFilter);
          }
        } else if (Event.current.type == EventType.Layout
                   && GUI.GetNameOfFocusedControl() != "quickFilter") {
          if (oldQuickFilterInputEnabled != quickFilterInputEnabled
              && !oldQuickFilterInputEnabled) {
            GUI.FocusControl("quickFilter");  // Initial set of the focus.
          } else {
            guiActions.Add(GuiActionCancelQuickFilter);  // The field has lost the focus.
          }
        }  
      } else {
        var title = quickFilterStr == "" ? "<i>NONE</i>" : quickFilterStr;
        if (GUILayout.Button(title, QuickFilterSizeLayout)) {
          guiActions.Add(GuiActionStartQuickFilter);
        }
      }
      oldQuickFilterInputEnabled = quickFilterInputEnabled;

      using (new GuiEnabledState(!quickFilterInputEnabled)) {
        // Clear logs in the current aggregator.
        if (GUILayout.Button("Clear")) {
          guiActions.Add(GuiActionClearLogs);
        }

        // Log mode selection. 
        GUI.changed = false;
        var showMode = GUILayout.SelectionGrid(
            (int) logShowMode, logShowingModes, logShowingModes.Length, MinSizeLayout);
        logsViewChanged |= GUI.changed;
        if (GUI.changed) {
          guiActions.Add(() => GuiActionSetMode((ShowMode) showMode));
        }

        GUI.changed = false;
        var isPaused = GUILayout.Toggle(logUpdateIsPaused, "PAUSED", MinSizeLayout);
        if (GUI.changed) {
          guiActions.Add(() => GuiActionSetPaused(isPaused));
        }

        // Draw logs filter by level and refresh logs when filter changes.
        GUI.changed = false;
        showInfo = MakeFormattedToggle(showInfo, infoLogColor, "INFO ({0})", infoLogs);
        showWarning = MakeFormattedToggle(showWarning, warningLogColor, "WARNING ({0})", warningLogs);
        showError = MakeFormattedToggle(showError, errorLogColor, "ERROR ({0})", errorLogs);
        showException =
            MakeFormattedToggle(showException, exceptionLogColor, "EXCEPTION ({0})", exceptionLogs);
        logsViewChanged |= GUI.changed;
      }
    }
  }

  /// <summary>Verifies if level of the log record is needed by the UI.</summary>
  /// <param name="log">The log record to verify.</param>
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

  /// <summary>Verifies if the log record matches the quick filter criteria.</summary>
  /// <remarks>The quick filter string is a case-insensitive prefix of the log's source.</remarks>
  /// <param name="log">The log record to verify.</param>
  /// <returns><c>true</c> if this log passes the quick filter check.</returns>
  bool LogQuickFilter(LogRecord log) {
    var filter = quickFilterInputEnabled ? oldQuickFilterStr : quickFilterStr;
    return log.srcLog.source.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
  }

  /// <summary>Makes a standard toggle GUI element.</summary>
  /// <param name="value">A toggle initial state.</param>
  /// <param name="color">A toggle color foreground.</param>
  /// <param name="fmt">A formatting string for the toggle caption</param>
  /// <param name="args">Arguments for the formatting string.</param>
  /// <returns></returns>
  bool MakeFormattedToggle(bool value, Color color, string fmt, params object[] args) {
    var oldColor = GUI.contentColor;
    GUI.contentColor = color;
    var res = GUILayout.Toggle(value, string.Format(fmt, args), MinSizeLayout);
    GUI.contentColor = oldColor;
    return res;
  }

  /// <summary>Shows a simple string in the scrolling area.</summary>
  /// <param name="text">The text to present.</param>
  void ShowStatusText(string text) {
    GUI.contentColor = Color.gray;
    GUILayout.Label("<i>" + text + "</i>");
    GUI.contentColor = Color.white;
  }

  /// <summary>Populates <see cref="logsToShow"/> and the stats numbers.</summary>
  /// <remarks>
  /// The current aggregator is determined from <see cref="logShowMode"/> and
  /// <see cref="logUpdateIsPaused"/> state.
  /// </remarks>
  void UpdateLogsView() {
    var currentAggregator =
        logUpdateIsPaused ? snapshotLogAggregator : GetCurrentAggregator();
    if (currentAggregator.FlushBufferedLogs() || logsViewChanged) {
      logsToShow = currentAggregator.GetLogRecords();
      infoLogs = currentAggregator.infoLogsCount;
      warningLogs = currentAggregator.warningLogsCount;
      errorLogs = currentAggregator.errorLogsCount;
      exceptionLogs = currentAggregator.exceptionLogsCount;
    }
    logsViewChanged = false;
  }
  
  /// <summary>Returns an aggregator for the currently selected mode.</summary>
  /// <returns>An aggregator.</returns>
  BaseLogAggregator GetCurrentAggregator() {
    BaseLogAggregator currentAggregator;
    if (logShowMode == ShowMode.Raw) {
      currentAggregator = rawLogAggregator;
    } else if (logShowMode == ShowMode.Collapsed) {
      currentAggregator = collapseLogAggregator;
    } else {
      currentAggregator = smartLogAggregator;
    }
    return currentAggregator;
  }

  #region GUI action handlers
  void GuiActionSetPaused(bool isPaused) {
    if (isPaused == logUpdateIsPaused) {
      return;  // Prevent refreshing of the snapshot if the mode hasn't changed.
    }
    if (isPaused) {
      snapshotLogAggregator.LoadLogs(GetCurrentAggregator());
    }
    logUpdateIsPaused = isPaused;
    logsViewChanged = true;
  }

  void GuiActionCancelQuickFilter() {
    if (quickFilterInputEnabled) {
      quickFilterInputEnabled = false;
      quickFilterStr = oldQuickFilterStr;
      oldQuickFilterStr = null;
      GuiActionSetPaused(false);
    }
  }

  void GuiActionAcceptQuickFilter() {
    quickFilterInputEnabled = false;
    oldQuickFilterStr = null;
    GuiActionSetPaused(false);
  }

  void GuiActionStartQuickFilter() {
    quickFilterInputEnabled = true;
    oldQuickFilterStr = quickFilterStr;
    GuiActionSetPaused(true);
  }

  void GuiActionClearLogs() {
    GuiActionSetPaused(false);
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
    GuiActionSetPaused(false);  // New mode invalidates the snapshot.
    logsViewChanged = true;
  }
  #endregion
}

} // namespace KSPDev
