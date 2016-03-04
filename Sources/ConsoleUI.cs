// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com a.k.a. "ihsoft"
// This software is distributed under Public domain license.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KSPDev {

/// <summary>A console to display Unity's debug logs in-game.</summary>
[KSPAddon(KSPAddon.Startup.EveryScene, false /*once*/)]
internal class ConsoleUI : MonoBehaviour {
  /// <summary>A hotkey to show and hide the console window.</summary>
  /// TODO: Read it from config.
  private static KeyCode toggleKey = KeyCode.BackQuote;

  // Display log level selection.
  // TODO: Read from config.  
  /// <summary>Specifies if INFO logs should be shown.</summary>
  private static bool showInfo = false;
  
  /// <summary>Specifies if WARNING logs should be shown.</summary>
  private static bool showWarning = true;
  
  /// <summary>Specifies if ERROR logs should be shown.</summary>
  private static bool showError = true;
  
  /// <summary>Specifies if EXCEPTION logs should be shown.</summary>
  private static bool showException = true;
  
  /// <summary>Log scrool box position.</summary>
  private static Vector2 scrollPosition;
  
  /// <summary>Specifies if debug console is visible.</summary>
  private static bool isConsoleVisible = false;

  // Display mode constants.
  // TODO: Use enum.  
  private const int ShowModeRaw = 0;
  private const int ShowModeCollapse = 1;
  private const int ShowModeSmart = 2;
  
  /// <summary>Current display mode.</summary>
  private static int logShowMode = ShowModeSmart;

  /// <summary>ID of the curently selected log record.</summary>
  /// <remarks>It shows expanded.</remarks>
  private static int selectedLogRecordId = -1;

  /// <summary>
  /// Indicates that visible log records should be queried from
  /// <seealso cref="snapshotLogAggregator"/>.
  /// </summary>
  private static bool logUpdateIsPaused = false;
  
  /// <summary>Idicates that logs from the current aggergator need to be requeried.</summary>
  private static bool logsViewChanged = false;
  
  /// <summary>A logger that keeps records on th disk.</summary>
  internal static PersistentLogAggregator diskLogAggregator = new PersistentLogAggregator();
  /// <summary>A logger to show when <seealso cref="ShowModeRaw"/> is selected.</summary>
  internal static PlainLogAggregator rawLogAggregator = new PlainLogAggregator();
  /// <summary>A logger to show when <seealso cref="ShowModeCollapse"/> is selected.</summary>
  internal static CollapseLogAggregator collapseLogAggregator = new CollapseLogAggregator();
  /// <summary>A logger to show when <seealso cref="ShowModeSmart"/> is selected.</summary>
  internal static SmartLogAggregator smartLogAggregator = new SmartLogAggregator();
  /// <summary>A logger to show a static snapshot.</summary>
  internal static SnapshotLogAggregator snapshotLogAggregator = new SnapshotLogAggregator();

  // TODO: Annotate
  internal static IEnumerable<LogRecord> logsToShow = new LogRecord[0];
  internal static int infoLogs = 0;
  internal static int warningLogs = 0;
  internal static int errorLogs = 0;
  internal static int exceptionLogs = 0;

  /// <summary>Console widnow margin on the screen.</summary>
  private const int Margin = 20;
  
  /// <summary>For every UI window Unity needs a unique ID. This is the one.</summary>
  private const int WindowId = 19450509;

  /// <summary>Actual screen position of the console window.</summary>
  private Rect windowRect =
      new Rect(Margin, Margin, Screen.width - (Margin * 2), Screen.height - (Margin * 2));

  // TODO: Document the others.
  private Rect titleBarRect = new Rect(0, 0, 10000, 20);
  private readonly GUIContent clearLabel =
      new GUIContent("Clear", "Clear the contents of the console.");
  private readonly GUIContent[] logShowingModes = {
      new GUIContent("Raw"),
      new GUIContent("Collapsed"),
      new GUIContent("Smart"),
  };
  
  /// <summary>Color settings for every type of the log.</summary>
  /// TODO: Read from config.
  private static readonly Dictionary<LogType, Color> logTypesColor =
      new Dictionary<LogType, Color>() {
          { LogType.Log, Color.white},
          { LogType.Warning, Color.yellow},
          { LogType.Error, Color.red},
          { LogType.Exception, Color.magenta},
          { LogType.Assert, Color.gray},
      };

  /// <summary>Only used to capture console toggle key.</summary>
  void Update() {
    if (Input.GetKeyDown(toggleKey)) {
      isConsoleVisible = !isConsoleVisible;
    }
    }
  }

  /// <summary>Actually renders the console window.</summary>
  void OnGUI() {
    if (!isConsoleVisible) {
      return;
    }
    windowRect = GUILayout.Window(WindowId, windowRect, MakeConsoleWindow, "Debug logs");
  }

  /// <summary>Verifies if level of the log record is needed by the UI.</summary>
  /// <param name="log">A log record to verify.</param>
  /// <returns><c>true</c> if this level is visible.</returns>
  private static bool LogLevelFilter(LogRecord log) {
    return log.srcLog.type == LogType.Exception && showException
        || log.srcLog.type == LogType.Error && showError
        || log.srcLog.type == LogType.Warning && showWarning
        || log.srcLog.type == LogType.Log && showInfo;
  }

  private readonly GUIUtils.GuiActionsList guiActions = new GUIUtils.GuiActionsList();
  
  /// <summary>Shows a window that displays the recorded logs.</summary>
  /// <param name="windowID">Window ID.</param>
  void MakeConsoleWindow(int windowID) {
    
    // Only update GUI state in a Layout pass which is the first pass in the frame. There may be
    // several different passes in OnGUI within the same frame, and in all the passes number/type of
    // GUI controls must match.
    if (guiActions.ExecutePendingGuiActions()) {
      UpdateLogsView(forceUpdate: logUpdateIsPaused);
    }

    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
    var logRecordStyle = new GUIStyle(GUI.skin.box) {
        alignment = TextAnchor.MiddleLeft,
    };
    var minSizeLayout = GUILayout.ExpandWidth(false);
    
    foreach (var log in logsToShow.Where(LogLevelFilter)) {
      var recordMsg = log.MakeTitle()
          + (selectedLogRecordId == log.srcLog.id ? ":\n" + log.srcLog.stackTrace : "");
      GUI.contentColor = logTypesColor[log.srcLog.type];
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

//        //TODO: parse frames (need stack) and suggest rewritings.
//        GUILayout.BeginHorizontal();
//        GUILayout.Label("Skip frames:", minSizeLayout);
//        GUILayout.Button("To KAS.Blah.Blah", GUILayout.ExpandWidth(false));
//        GUILayout.Button("By prefix: KIS.", GUILayout.ExpandWidth(false));
//        GUILayout.Button("By prefix: KIS.Some.", GUILayout.ExpandWidth(false));
//        GUILayout.EndHorizontal();
      }
    }
    GUILayout.EndScrollView();

    GUI.contentColor = Color.white;

    // Bottom menu.
    GUILayout.BeginHorizontal();
    
    // Clear logs in the current aggregator. 
    if (GUILayout.Button(clearLabel)) {
      guiActions.Add(GuiActionClearLogs);
    }
    
    // Log mode selection. 
    GUI.changed = false;
    var showMode = GUILayout.SelectionGrid(
        logShowMode, logShowingModes, logShowingModes.Length, GUILayout.ExpandWidth(false));
    logsViewChanged |= GUI.changed;
    if (GUI.changed) {
      guiActions.Add(() => GuiActionSetMode(mode: showMode));
    }

    //FIXME: make it a button
    GUI.changed = false;
    logUpdateIsPaused = GUILayout.Toggle(logUpdateIsPaused, "PAUSED", GUILayout.ExpandWidth(false));
    if (GUI.changed) {
      guiActions.Add(() => GuiActionSetPaused(isPaused: logUpdateIsPaused));
    }
    
    // Draw logs filter by level and refresh logs when filter changes.
    GUI.changed = false;
    showInfo = MakeFormattedToggle(
        showInfo, logTypesColor[LogType.Log], "INFO ({0})", infoLogs);
    showWarning = MakeFormattedToggle(
        showWarning, logTypesColor[LogType.Warning], "WARNING ({0})", warningLogs);
    showError = MakeFormattedToggle(
        showError, logTypesColor[LogType.Error], "ERROR ({0})", errorLogs);
    showException = MakeFormattedToggle(
        showException, logTypesColor[LogType.Exception], "EXCEPTION ({0})", exceptionLogs);
    logsViewChanged |= GUI.changed;
    GUILayout.EndHorizontal();

    // Allow the window to be dragged by its title bar.
    GUI.DragWindow(titleBarRect);
  }

  /// <summary>Makes a standard toggle GUI element.</summary>
  /// <param name="value">A toggle initial state.</param>
  /// <param name="color">A toggle color foreground.</param>
  /// <param name="fmt">A formatting string for the toggle caption</param>
  /// <param name="args">Arguments for the formatting string.</param>
  /// <returns></returns>
  private bool MakeFormattedToggle(bool value, Color color, string fmt, params object[] args) {
    GUI.contentColor = color;
    return GUILayout.Toggle(value, string.Format(fmt, args), GUILayout.ExpandWidth(false));
  }

  /// <summary>Populates <seealso cref="logsToShow"/> and stats numbers.</summary>
  /// <remarks>Current aggregator is determined from <seealso cref="logShowMode"/> and
  /// <seealso cref="logUpdateIsPaused"/></remarks>
  /// <param name="forceUpdate">If <c>false</c> then logs view will only be updated if there were
  /// newly aggregated records in teh current aggregator.</param>
  private void UpdateLogsView(bool forceUpdate = false) {
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
  private BaseLogAggregator GetCurrentAggregator(bool ignorePaused = false) {
    BaseLogAggregator currentAggregator = snapshotLogAggregator;
    if (ignorePaused || !logUpdateIsPaused) {
      if (logShowMode == ShowModeRaw) {
        currentAggregator = rawLogAggregator;
      } else if (logShowMode == ShowModeCollapse) {
        currentAggregator = collapseLogAggregator;
      } else {
        currentAggregator = smartLogAggregator;
      }
    }
    return currentAggregator;
  }
  
  private void GuiActionSetPaused(bool isPaused) {
    if (isPaused) {
      snapshotLogAggregator.LoadLogs(GetCurrentAggregator(ignorePaused: true));
    }
    logUpdateIsPaused = isPaused;
    logsViewChanged = true;
  }

  private void GuiActionClearLogs() {
    GuiActionSetPaused(isPaused: false);
    GetCurrentAggregator().ClearAllLogs();
    logsViewChanged = true;
  }
  
  private void GuiActionSelectLog(int newSelectedId) {
    selectedLogRecordId = newSelectedId;
  }
  
  private void GuiActionAddSilence(string pattern, bool isPrefix) {
    if (isPrefix) {
      LogFilter.AddSilenceByPrefix(pattern);
    } else {
      LogFilter.AddSilenceBySource(pattern);
    }
    LogFilter.SaveFilters();

    rawLogAggregator.UpdateFilter();
    collapseLogAggregator.UpdateFilter();
    smartLogAggregator.UpdateFilter();
    snapshotLogAggregator.UpdateFilter();
    logsViewChanged = true;
  }
  
  private void GuiActionSetMode(int mode) {
    logShowMode = mode;
    GuiActionSetPaused(isPaused: false);  // New mode invalidates snapshot.
    logsViewChanged = true;
  }
}

/// <summary>Only used to start logs aggregations earlier.</summary>
[KSPAddon(KSPAddon.Startup.Instantly, true /*once*/)]
internal class AggregationStarter : MonoBehaviour {
  void Awake() {
    // First, ensure log innterception is started.
    LogInterceptor.StartIntercepting();
    ConsoleUI.diskLogAggregator.StartCapture();
    LogFilter.LoadFilters();
    ConsoleUI.rawLogAggregator.StartCapture();
    ConsoleUI.collapseLogAggregator.StartCapture();
    ConsoleUI.smartLogAggregator.StartCapture();
  }
}

} // namespace KSPDev
