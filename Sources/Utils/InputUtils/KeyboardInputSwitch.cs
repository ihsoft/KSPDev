// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using System;
using UnityEngine;

namespace KSPDev.InputUtils {

/// <summary>
/// Wrapper around keyboard key code that incapsulates persiting and handling into a single class.  
/// </summary>
/// <remarks>
/// Fields of this type are correctly handled by <see cref="KSPField"/> and
/// <see cref="PersistentFieldAttribute"/> attributes.
/// </remarks>
/// <example>
/// <para>
/// To define a key binding just create a class with the key code as a parameter and notify the
/// switch about frame updates so what it could update its state:
/// </para>
/// <code><![CDATA[
/// class MyClass : MonoBehaviour {
///   KeyboardInputSwitch mySwitch = new KeyboardInputSwitch(KeyCode.Alpha1);
///
///   void Update() {
///     if (mySwitch.Update()) {
///       Debug.Log("The key is being hold");
///     }
///   }
/// }
/// ]]></code>
/// <para>
/// In case of switch state needs to be checked from other methods use <see cref="isHold"/>
/// property:
/// </para>
/// <code><![CDATA[
/// class MyClass : MonoBehaviour {
///   KeyboardInputSwitch mySwitch = new KeyboardInputSwitch(KeyCode.Alpha1);
///
///   void Update() {
///     mySwitch.Update();
///   }
///
///   void FixedUpdate() {
///     if (mySwitch.isHold) {
///       Debug.Log("The key is being hold");
///     }
///   }
/// }
/// ]]></code>
/// <para>
/// When code needs to react to the event of changing switch state it can register for the state
/// change event:
/// </para>
/// <code><![CDATA[
/// class MyClass : MonoBehaviour {
///   KeyboardInputSwitch mySwitch = new KeyboardInputSwitch(KeyCode.Alpha1);
///
///   void Awake() {
///     mySwitch.OnStateChanged += OnSwitchStateChange;
///     // Also add a debug handler via simple lambda...
///     mySwitch.OnStateChanged += x => Debug.Log("State changed!");
///     // ...and add one more lambda...
///     mySwitch.OnStateChanged += x => {
///       if (x) {
///         Debug.Log("Key is released");
///       } else {
///         Debug.Log("Key is pressed");
///       }
///     };
///   }
///
///   void OnDestroy() {
///     // Do nothing since switch is an instance field, and it will be
///     // destroyed together with all the listeners.
///     // Though, if it was static we would do something like this:
///     mySwitch.OnStateChanged -= OnSwitchStateChange;
///   }
///
///   void OnSwitchStateChange(bool oldState) {
///     Debug.LogFormat("Switch state changed: {0}=>{1}", oldState, mySwitch.isHold);
///   }
///
///   void Update() {
///     mySwitch.Update();
///   }
/// }
/// ]]></code>
/// <para>
/// In many cases you may want to load key bining from a config file. Due to switch supports
/// KSP/KSPDev persistence it can easily be achieved by just adding an attribute:
/// </para>
/// <code><![CDATA[
/// [PersistentFieldsFile("my/mod/settings.cfg", "")]
/// class MyClass : PartModule {
///   // Note that KSPField attributed fields *must* be public.
///   [KSPField]
///   public KeyboardInputSwitch switchFromPart = new KeyboardInputSwitch(KeyCode.Alpha1);
///
///   // Note that for PersistentField attribute field doesn't need to be public.
///   // Though, private fields are handled a bit differently (read the docs!).
///   [PersistentField("Keyboard/Bindings")]
///   KeyboardInputSwitch switchFromSettings = new KeyboardInputSwitch(KeyCode.Alpha2);
///
///   public override void OnLoad(ConfigNode node) {
///     // At this point `switchFromPart` is already loaded from the part's config.
///     base.OnLoad(node);
///     // Load `switchFromSettings` via KSPDev from "my/mod/settings.cfg".
///     KSPDev.ConfigUtils.ConfigAccessor.ReadFieldsInType(typeof(MyClass), this);
///   }
/// }
/// ]]></code>
/// <para>Storing of the key binding works in a similar way.</para>
/// </example>
/// <seealso cref="PersistentFieldsFileAttribute"/>
/// <seealso cref="PersistentFieldAttribute"/>
/// <seealso href="https://kerbalspaceprogram.com/api/class_k_s_p_field.html">KSP: KSPField</seealso>
public class KeyboardInputSwitch : IConfigNode {
  /// <summary>Says if any switch is being hold.</summary>
  /// <remarks>
  /// This value is version specific. I.e. multiple versions of the utils DLL will not see each
  /// other. So if this property returns <c>true</c> then the only safe assumption is that any
  /// switch <i>within the running</i> mod is being hold. Ideally, when all mods in the game run the
  /// same version of the utils DLL this property will truly say if <i>any</i> switch across all
  /// mods is hold.
  /// </remarks>
  public static bool isAnyKeyHold { get { return keysHold > 0; } }
  static int keysHold;

  /// <summary>Defines current hold state of the switch.</summary>
  /// <remarks>
  /// Note that when reading this property it may not represent actual keyboard key hold state since
  /// switch state can be assigned from the code.
  /// </remarks>
  public bool isHold {
    get { return _isHold; }
    set { SetHoldState(value); }
  }
  bool _isHold;

  /// <summary>Key code for the switch.</summary>
  /// <remarks>
  /// It can be changed in runtime but if hold state was <c>true</c> it must be reset by the caller.
  /// </remarks>
  public KeyCode keyCode;

  /// <summary>
  /// Events that notify about hold state change. Callback's argument is the <i>old</i> value of the
  /// state.
  /// </summary>
  /// <remarks>
  /// Remember to remove listeners when their owner class is destroyed by the game. If it's not done
  /// no NRE will happen but "ghost" listeners will continue to react on the events.  
  /// </remarks>
  /// <remarks>The callback is only called when state has actually changed.</remarks>
  public event Callback<bool> OnStateChanged;

  /// <summary>
  /// Determines if switch should react on keyboard events from <see cref="Update"/> method. 
  /// </summary>
  /// <remarks>
  /// Note that if switch is disabled while the key was hold the hold state will <i>not</i> be
  /// reset. If state needs to be reset then caller must do it explicitly.
  /// </remarks>
  public bool keyboardEnabled = true;

  #region IConfigNode implementation
  /// <summary>
  /// Loads switch binding when field is attributed with KSP or KSPDev persisting attributes. 
  /// </summary>
  /// <param name="node">Node to get values from.</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  /// <seealso href="https://kerbalspaceprogram.com/api/class_k_s_p_field.html">KSP: KSPField</seealso>
  public virtual void Load(ConfigNode node) {
    ConfigAccessor.GetValueByPath(node, "keyCode", ref keyCode);
  }

  /// <summary>
  /// Saves switch binding when field is attributed with KSP or KSPDev persisting attributes. 
  /// </summary>
  /// <param name="node">Node to get values from.</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  /// <seealso href="https://kerbalspaceprogram.com/api/class_k_s_p_field.html">KSP: KSPField</seealso>
  public virtual void Save(ConfigNode node) {
    node.SetValue("keyCode", keyCode.ToString(), createIfNotFound: true);
  }
  #endregion

  /// <summary>
  /// Creates a switch with a <see cref="KeyCode.None"/> key binding. It's a default constructor
  /// needed for config utils to work.
  /// </summary>
  public KeyboardInputSwitch() : this(KeyCode.None) {
  }

  /// <summary>Main constructor to create a switch for the provided key code.</summary>
  /// <param name="code">
  /// Key code to activate the switch. Can be <see cref="KeyCode.None"/> in which case this switch
  /// can only be changed via code.
  /// </param>
  public KeyboardInputSwitch(KeyCode code) {
    this.keyCode = code;
  }

  /// <summary>Checks keyboard status and updates the switch accordingly.</summary>
  /// <remarks>
  /// This method handles game's pause and time warp modes, and disables key handling in these
  /// modes. It also respects UI locking mode set by the game.
  /// </remarks>
  /// <returns>Current hold state.</returns>
  /// <seealso cref="keyboardEnabled"/>
  public virtual bool Update() {
    if (!keyboardEnabled) {
      return false;
    }
    if (Input.GetKeyUp(keyCode)
        || Mathf.Approximately(Time.timeScale, 0f)
        || Time.timeScale > 1f
        || InputLockManager.IsLocked(ControlTypes.UI)) {
      SetHoldState(false);
    } else if (Input.GetKeyDown(keyCode) && !isAnyKeyHold) {
      SetHoldState(true);
    }
    return isHold;
  }

  /// <summary>Updates hold state and triggers event(s) if any.</summary>
  /// <param name="newState">New hold state.</param>
  /// <seealso cref="OnStateChanged"/>
  protected virtual void SetHoldState(bool newState) {
    if (_isHold != newState) {
      var oldState = _isHold;
      _isHold = newState;
      if (newState) {
        keysHold++;
      } else {
        keysHold--;
      }
      if (OnStateChanged != null) {
        OnStateChanged(oldState);
      }
    }
  }
}

}  // namespace
