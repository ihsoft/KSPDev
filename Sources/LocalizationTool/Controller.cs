// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSP.Localization;
using KSPDev.FSUtils;
using KSPDev.ConfigUtils;
using KSPDev.GUIUtils;
using KSPDev.InputUtils;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSPDev.LocalizationTool {

[PersistentFieldsFileAttribute("KSPDev/LocalizationTool/PluginData/settings.cfg", "UI")]
[PersistentFieldsFileAttribute("KSPDev/LocalizationTool/PluginData/session.cfg", "UI",
                               Controller.SessionGroup)]
class Controller : MonoBehaviour {
  #region GUI scrollbox records
  /// <summary>Base class for the records that represent the extractor entities.</summary>
  abstract class ScannedRecord {
    public bool selected;
    public abstract void GUIAddItem();
  }

  /// <summary>Simple item to display info in the scroll box. It cannot be selected.</summary>
  class StubRecord : ScannedRecord {
    public string stubText;
    public override void GUIAddItem() {
      GUILayout.Label(stubText);
    }
  }

  /// <summary>Item that represents mod's parts record.</summary>
  class PartsRecord : ScannedRecord {
    public List<AvailablePart> parts = new List<AvailablePart>();
    public string urlPrefix = "";

    /// <inheritdoc/>
    public override void GUIAddItem() {
      selected = GUILayout.Toggle(selected, ToString());
    }

    /// <inheritdoc/>
    public override string ToString() {
      return string.Format("{0} ({1} parts)", urlPrefix, parts.Count);
    }
  }

  /// <summary>Item that represents an assembly record.</summary>
  class AssemblyRecord : ScannedRecord {
    public Assembly assembly;
    public List<Type> types;

    /// <inheritdoc/>
    public override void GUIAddItem() {
      selected = GUILayout.Toggle(selected, ToString());
    }

    /// <inheritdoc/>
    public override string ToString() {
      return string.Format("{0}, v{1} ({2} modules)",
                           KspPaths.MakeRelativePathToGameData(assembly.Location),
                           assembly.GetName().Version, types.Count);
    }
  }
  #endregion

  /// <summary>Name of the persistent group to keep session settings in.</summary>
  /// <remarks>
  /// Session keeps current UI and layout settings. They get changed frequently and saved/loaded on
  /// every scene.
  /// </remarks>
  const string SessionGroup = "session";

  #region Mod's settings
  [PersistentField("KeySwitch")]
  static KeyboardInputSwitch switchKey = new KeyboardInputSwitch(KeyCode.LeftAlt);

  [PersistentField("keyToggle")]
  static KeyCode toggleKey = KeyCode.F9;

  [PersistentField("scrollHeight")]
  static int scrollHeight = 150;
  #endregion

  #region Session settings
  [PersistentField("windowPos", group = SessionGroup)]
  static Vector2 windowPos = new Vector2(0, 0);

  /// <summary>Specifies if debug console is visible.</summary>
  [PersistentField("isOpen", group = SessionGroup)]
  static bool isUIVisible;

  [PersistentField("lookupPrefix", group = SessionGroup)]
  string lookupPrefix = "";
  #endregion

  /// <summary>A list of actions to apply at the end of the GUI frame.</summary>
  static readonly GuiActionsList guiActions = new GuiActionsList();

  #region Window intermediate properties
  const int WindowId = 19410622;
  static Vector2 windowSize = new Vector2(430, 0);
  static Rect titleBarRect = new Rect(0, 0, 10000, 20);
  static Rect windowRect;
  #endregion

  readonly List<ScannedRecord> targets = new List<ScannedRecord>();
  Vector2 partsScrollPos;
  string lastCachedLookupPrefix;

  #region MonoBehaviour overrides 
  /// <summary>Only loads session settings.</summary>
  void Awake() {
    ConfigAccessor.ReadFieldsInType(typeof(Controller), null /* instance */);
    ConfigAccessor.ReadFieldsInType(typeof(Controller), this, group: SessionGroup);
    windowRect = new Rect(windowPos, windowSize);
  }

  /// <summary>Only stores session settings.</summary>
  void OnDestroy() {
    windowPos = windowRect.position;
    ConfigAccessor.WriteFieldsFromType(typeof(Controller), this, group: SessionGroup);
  }

  /// <summary>Only used to capture console toggle key.</summary>
  void Update() {
    if (switchKey.Update() && Input.GetKeyDown(toggleKey)) {
      isUIVisible = !isUIVisible;
    }
  }

  /// <summary>Actually renders the console window.</summary>
  void OnGUI() {
    if (isUIVisible) {
      windowRect = GUILayout.Window(
          WindowId, windowRect, MakeConsoleWindow,
          "KSPDev LocalizationTool v" + this.GetType().Assembly.GetName().Version);
    }
  }
  #endregion

  /// <summary>Shows a UI dialog.</summary>
  /// <param name="windowID">The window ID. Unused.</param>
  void MakeConsoleWindow(int windowID) {
    guiActions.ExecutePendingGuiActions();
    using (new GUILayout.VerticalScope(GUI.skin.box)) {
      // Search prefix controls.
      using (new GUILayout.HorizontalScope(GUI.skin.box)) {
        GUILayout.Label("URL prefix:", GUILayout.ExpandWidth(false));
        lookupPrefix = GUILayout.TextField(lookupPrefix, GUILayout.ExpandWidth(true)).TrimStart();
        if (lookupPrefix != lastCachedLookupPrefix) {
          lastCachedLookupPrefix = lookupPrefix;
          guiActions.Add(() => GuiActionUpdateTargets(lookupPrefix));
        }
      }

      // Found items scroll view.
      using (new GUILayout.HorizontalScope(GUI.skin.box)) {
        using (var scrollScope =
               new GUILayout.ScrollViewScope(partsScrollPos, GUILayout.Height(scrollHeight))) {
          partsScrollPos = scrollScope.scrollPosition;
          foreach (var target in targets) {
            target.GUIAddItem();
          }
        }
      }

      // Action buttons.
      if (GUILayout.Button("Export strings into a new file")) {
        GuiActionExportStrings();
      }
      if (GUILayout.Button("Refresh strings from the configs")) {
        GuiActionRefreshStrings();
      }
    }
    GUI.DragWindow(titleBarRect);
  }

  /// <summary>Saves the strings for the selected entities into a new file.</summary>
  void GuiActionExportStrings() {
    //FIXME
    Debug.LogWarningFormat("*** GuiActionExportStrings");
  }
  
  /// <summary>Saves the strings for the selected entities into a new file.</summary>
  void GuiActionRefreshStrings() {
    //FIXME
    Debug.LogWarningFormat("*** GuiActionRefreshStrings");
  }
  
  /// <summary>Finds all the entities for the prefix, and populates the list.</summary>
  /// <param name="prefix">The prefix to find URL by.</param>
  //FIXME: Create localization targets.
  void GuiActionUpdateTargets(string prefix) {
    targets.Clear();
    if (prefix.Length < 3) {
      targets.Add(new StubRecord() {
          stubText = "<i>...type 3 or more prefix characters...</i>",
      });
      return;
    }

    // Find part configs for the prefix.
    var parts = PartLoader.LoadedPartsList
        .Where(x => x.partUrl.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
        .GroupBy(x => {
          var pos = x.partUrl.LastIndexOf("/Parts", StringComparison.OrdinalIgnoreCase);
          return pos != -1 ? x.partUrl.Substring(0, pos + 6) : x.partUrl;
        });
    foreach (var group in parts) {
      targets.Add(new PartsRecord() {
          urlPrefix = group.Key,
          parts = group.ToList(),
      });
    }

    // Find assemblies for the prefix.
    var assemblies = AssemblyLoader.loadedAssemblies
      .Where(x => x.url.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)
                  && x.types.Count > 0);
    foreach (var assembly in assemblies) {
      targets.Add(new AssemblyRecord() {
          assembly = assembly.assembly,
          types = assembly.types.SelectMany(x => x.Value).ToList(),
      });
    }

    if (targets.Count == 0) {
      targets.Add(new StubRecord() {
          stubText = "<i>...nothing found for the prefix...</i>",
      });
    }
  }
}

[KSPAddon(KSPAddon.Startup.MainMenu, false /*once*/)]
class ControllerLauncher1 : Controller {
}

[KSPAddon(KSPAddon.Startup.FlightAndEditor, false /*once*/)]
class ControllerLauncher2 : Controller {
}

}  // namesapce
