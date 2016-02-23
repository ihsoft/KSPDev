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

  /// <summary>Log scrool box position.</summary>
  private static Vector2 scrollPosition;
  
  /// <summary>Specifies if debug console is visible.</summary>
  private static bool showConsole = false;

  // Display log level selection.
  // TODO: Read from config.  
  private static bool showInfo = false;
  private static bool showWarning = true;
  private static bool showError = true;
  private static bool showException = true;
  private const bool showAsserts = true;
  
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
  /// If <c>true</c> the UI updates are frozen and only a napshot of logs is presented.
  /// </summary>
  private static bool logUpdateIsPaused = false;
  
  /// <summary>A logger to show when <seealso cref="ShowModeSmart"/> is selected.</summary>
  internal static SmartLogAggregator smartLogAggregator = new SmartLogAggregator();

  /// <summary>Console widnow margin on the screen.</summary>
  private const int Margin = 20;
  
  /// <summary>For every UI window Unity needs a unique ID. This is the one.</summary>
  private const int WindowId = 1945;

  /// <summary>Actual screen position of the console window.</summary>
  private Rect windowRect =
      new Rect(Margin, Margin, Screen.width - (Margin * 2), Screen.height - (Margin * 2));

  // TODO: Document the others.
  private Rect titleBarRect = new Rect(0, 0, 10000, 20);
  private readonly GUIContent clearLabel =
      new GUIContent("Clear", "Clear the contents of the console.");
  private readonly GUIContent showInfoLabel = new GUIContent("INFO", "Show records of type Log.");
  private readonly GUIContent showWarningLabel =
      new GUIContent("WARNING", "Show records of type Warning.");
  private readonly GUIContent showErrorLabel =
      new GUIContent("ERROR", "Show records of type Error.");
  private readonly GUIContent showExceptionLabel =
      new GUIContent("EXCEPTION", "Show records of type Exception.");
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
      showConsole = !showConsole;
    }
  }

  /// <summary>Actually renders the console window.</summary>
  void OnGUI() {
    if (!showConsole) {
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
        || log.srcLog.type == LogType.Log && showInfo
        || log.srcLog.type == LogType.Assert && showAsserts;
  }

  private delegate void GUIAction();
  private readonly List<GUIAction> guiActions = new List<GUIAction>();
  
  /// <summary>Shows a window that displays the recorded logs.</summary>
  /// <param name="windowID">Window ID.</param>
  void MakeConsoleWindow(int windowID) {
    // Only update GUI state in a Layout pass which is the first pass in the frame. There may be
    // several different passes in OnGUI within the same frame, and in all the passes number/type of
    // GUI controls must match.
    if (Event.current.type == EventType.Layout) {
      // Apply GUI actions from the previous frame.
      if (guiActions.Any()) {
        foreach (var action in guiActions) {
          action();
        }
        guiActions.Clear();
      }
      smartLogAggregator.FlushBufferedLogs();

//      if (!logUpdateIsPaused) {
//        // Update log records.
//        if (logShowMode == ShowModeRaw) {
//          UpdateRawView();
//        } else if (logShowMode == ShowModeCollapse) {
//          UpdateCollapsedView();
//        } else if (logShowMode == ShowModeSmart) {
//          UpdateSmartView();
//        }
//      }
    }

    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
    var logRecordStyle = new GUIStyle(GUI.skin.box) {
        alignment = TextAnchor.MiddleLeft,
    };
    var minSizeLayout = GUILayout.ExpandWidth(false);
    
    // TODO: make in configurable via UI
//    const bool reverseOrder = true;
//    var logsToRender = logRecords.Where(LogLevelFilter);
    var logs = smartLogAggregator.GetLogRecords().Where(LogLevelFilter);
//    IEnumerable<LogRecord> logs = reverseOrder ? logsToRender.Reverse() : logsToRender;

    // Iterate through the recorded logs and add item into the scrolling control.
    //var newSelectedLogRecord = selectedLogRecord;
    foreach (var log in logs) {
      var recordMsg = log.MakeTitle()
          + (selectedLogRecordId == log.srcLog.id ? ":\n" + log.srcLog.stackTrace : "");
      GUI.contentColor = logTypesColor[log.srcLog.type];
      GUILayout.Box(recordMsg, logRecordStyle);

      // Check if log record is selected.
      if (Event.current.type == EventType.MouseDown) {
        //TODO: using Input.MouseDown may be better idea.
        //Pos.y = Screen.height - Pos.y;
        Rect logBoxRect = GUILayoutUtility.GetLastRect();
        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
          // Toggle selection.
          var newSelectedId = selectedLogRecordId == log.srcLog.id ? -1 : log.srcLog.id;
          guiActions.Add(() => selectedLogRecordId = newSelectedId);
        }
      }
      
      // Add source and filter controls when expanded.
      if (selectedLogRecordId == log.srcLog.id && log.srcLog.source.Any()) {
        GUI.contentColor = Color.white;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Silence: source", minSizeLayout);
        if (GUILayout.Button(log.srcLog.source, minSizeLayout)) {
          guiActions.Add(() => {
              LogFilter.AddSilenceBySource(log.srcLog.source);
              LogFilter.SaveFilters();
          });
          //UNDONE: temp solution. make a new buttion.
//          Type T = typeof(GUIUtility);
//          var systemCopyBufferProperty = T.GetProperty("systemCopyBuffer");
//          systemCopyBufferProperty.SetValue(null, "bla bla", null);
          //System.Windows.Clipboard.SetText(log.MakeTitle() + "\n" + log.srcLog.stackTrace);
        }
        var sourceParts = log.srcLog.source.Split('.');
        if (sourceParts.Length > 1) {
          GUILayout.Label("or by prefix", minSizeLayout);
          for (var i = sourceParts.Length - 1; i > 0; --i) {
            var prefix = String.Join(".", sourceParts.Take(i).ToArray()) + '.';
            if (GUILayout.Button(prefix, minSizeLayout)) {
              guiActions.Add(() => {
                  LogFilter.AddSilenceByPrefix(prefix);
                  smartLogAggregator.UpdateFilter();
                  LogFilter.SaveFilters();
              });
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
    if (GUILayout.Button(clearLabel)) {
      guiActions.Add(smartLogAggregator.ClearAllLogs);
    }

    // Draw logs mode controls and refresh logs on mode change.
    var oldShowMode = logShowMode;
    logShowMode = GUILayout.SelectionGrid(
        logShowMode, logShowingModes, logShowingModes.Length, GUILayout.ExpandWidth(false));
    if (oldShowMode != logShowMode) {
      //UNDONE: debug
      Debug.LogWarning(String.Format("Refresh due to mode change: {0}=>{1}", oldShowMode, logShowMode));
    }

    //FIXME: make it a button
    logUpdateIsPaused = GUILayout.Toggle(logUpdateIsPaused, "PAUSED", GUILayout.ExpandWidth(false));
    
    // Draw logs filter by level and refresh logs when filter changes.
    GUI.changed = false;
    showInfo = GUILayout.Toggle(showInfo, showInfoLabel, GUILayout.ExpandWidth(false));
    showWarning = GUILayout.Toggle(showWarning, showWarningLabel, GUILayout.ExpandWidth(false));
    showError = GUILayout.Toggle(showError, showErrorLabel, GUILayout.ExpandWidth(false));
    showException = GUILayout.Toggle(showException, showExceptionLabel, GUILayout.ExpandWidth(false));
    if (GUI.changed) {
      //UNDONE: debug
      Debug.LogWarning(String.Format("Refresh due to filter change: {0}", logShowMode));
    }

    GUILayout.EndHorizontal();

    // Allow the window to be dragged by its title bar.
    GUI.DragWindow(titleBarRect);
  }
}

/// <summary>Only used to start logs aggregations earlier.</summary>
[KSPAddon(KSPAddon.Startup.Instantly, true /*once*/)]
internal class AggregationStarter : MonoBehaviour {
  void Awake() {
    ConsoleUI.smartLogAggregator.StartCapture();
  }
}

} // namespace KSPDev
