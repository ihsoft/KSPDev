// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using System;
using UnityEngine;

namespace KSPDev.InputUtils {

/// <summary>
/// Wrapper around a keyboard key code that incapsulates the persiting and the handling logic into
/// a single class.
/// </summary>
/// <remarks>
/// <para>
/// The fields of this type are correctly handled by the stock game persisting functionality
/// (<see cref="KSPField"/>). It's also compatible with the KSPDev persisting logic
/// (<see cref="PersistentFieldAttribute"/>).
/// </para>
/// <para>
/// <i>Important!</i> This type will be correctly loaded or saved by the KSP core but it will
/// <i>not</i> be correctly copied in the game's editor. It's not an issue when the field is static
/// but in case of it's an instance member, the code must not be accessing it in the editor mode.
/// </para>
/// </remarks>
/// <example>
/// <para>
/// To define a key binding just create a class with the key code as a parameter, and notify the
/// switch about the frame updates so that it could update its state:
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
/// In case of the switch state needs to be checked from the other methods use <see cref="isHold"/>
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
/// When the code needs to react to a switch state event, it can register a listener:
/// </para>
/// <code><![CDATA[
/// class MyClass : MonoBehaviour {
///   KeyboardInputSwitch mySwitch = new KeyboardInputSwitch(KeyCode.Alpha1);
///
///   void Awake() {
///     mySwitch.OnStateChanged += OnSwitchStateChange;
///     mySwitch.OnPress += delegate{
///       Debug.Log("Key pressed");
///     };
///     mySwitch.OnRelease += delegate{
///       Debug.Log("Key is released");
///     };
///     mySwitch.OnClick += delegate{
///       Debug.Log("Key click registered");
///     };
///   }
///
///   void OnDestroy() {
///     // Do nothing since in this example the switch is an instance field, and it will be
///     // destroyed together with the owning class (and, hence, all the listeners).
///     // However, if it was a static field we would do something like this:
///     mySwitch.OnStateChanged -= OnSwitchStateChange;
///     // Anonymous functions cannot be unregistered, so don't use them on the static fields.
///   }
///
///   void OnSwitchStateChange() {
///     Debug.LogFormat("Switch state changed to: {0}", mySwitch.isHold);
///   }
///
///   void Update() {
///     mySwitch.Update();
///   }
/// }
/// ]]></code>
/// <para>
/// In many cases you may want to load a key bining from a config file. It can be achieved by
/// adding an attribute to the field of this type:
/// </para>
/// <code><![CDATA[
/// [PersistentFieldsFile("my/mod/settings.cfg", "")]
/// class MyClass : PartModule {
///   // Note that KSPField attributed fields *must* be public.
///   [KSPField]
///   public KeyboardInputSwitch switchFromPart = new KeyboardInputSwitch(KeyCode.Alpha1);
///
///   // Note that for a PersistentField attribute the field doesn't need to be public.
///   // However, the private fields are handled a bit differently (read the docs!).
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
/// </example>
/// <seealso cref="PersistentFieldAttribute"/>
/// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
public class KeyboardInputSwitch : IConfigNode {
  /// <summary>Tells if any switch is being hold.</summary>
  /// <remarks>
  /// This value is a version specific. I.e. the multiple versions of the utils DLL will not see
  /// each other. So, if this property returns <c>true</c> then the only safe assumption is that any
  /// switch <i>within the running</i> mod is being hold. Ideally, when all the mods in the game run
  /// the same version of the utils DLL, this property will truly say if any in the <i>game</i> is
  /// in the hold state.
  /// </remarks>
  /// <value><c>true</c> if any <i>switch</i> key is being hold.</value>
  public static bool isAnyKeyHold { get { return keysHold > 0; } }
  static int keysHold;

  /// <summary>Maximum delay to record a click event.</summary>
  /// <remarks>
  /// If the key is released later than this delay, then the click event will not be triggered. By
  /// default the switch uses the same value as defined in the core KSP <see cref="KeyBinding"/>.
  /// </remarks>
  /// <seealso cref="OnClick"/>
  /// <seealso href="https://kerbalspaceprogram.com/api/class_key_binding.html">KSP: KeyBinding</seealso>
  public const float ClickDelay = 0.2f;

  /// <summary>Defines the current hold state of the switch.</summary>
  /// <remarks>
  /// This property may not represent the actual keyboard key hold state since it can be assigned
  /// from the code.
  /// </remarks>
  /// <value><c>true</c> if this switch is being hold.</value>
  /// <see cref="SetHoldState"/>
  public bool isHold {
    get { return _isHold; }
    set { SetHoldState(value); }
  }
  bool _isHold;

  /// <summary>Key code for the switch.</summary>
  /// <remarks>
  /// It can be changed in runtime but if the hold state was <c>true</c> then it must be reset by
  /// the caller.
  /// </remarks>
  public KeyCode keyCode;

  /// <summary>
  /// Determines if the switch should react on the keyboard events from the <see cref="Update"/>
  /// method.
  /// </summary>
  /// <remarks>
  /// If switch is disabled while the key was pressed then the hold state will <i>not</i> be reset.
  /// If the state needs to be reset then caller must do it explicitly.
  /// </remarks>
  public bool keyboardEnabled = true;

  #region Events
  /// <summary>
  /// Event that notifies about the hold state change. The event is only called when the state has
  /// actually changed.
  /// </summary>
  /// <remarks>
  /// Remember to remove the listeners when their owner class is destroyed by the game. If it's not
  /// done then no NRE will happen, but the "ghost" listeners will continue to react on the events.  
  /// </remarks>
  public event Callback OnStateChanged;
  
  /// <summary>
  /// Event that notifies that the switch key has been pressed.
  /// </summary>
  /// <remarks>
  /// Remember to remove the listeners when their owner class is destroyed by the game. If it's not
  /// done then no NRE will happen, but the "ghost" listeners will continue to react on the events.  
  /// </remarks>
  public event Callback OnPress;

  /// <summary>
  /// Event that notifies that the switch key has been released.
  /// </summary>
  /// <remarks>
  /// Remember to remove the listeners when their owner class is destroyed by the game. If it's not
  /// done then no NRE will happen, but the "ghost" listeners will continue to react on the events.  
  /// </remarks>
  public event Callback OnRelease;

  /// <summary>
  /// Event that notifies about the click event.
  /// </summary>
  /// <remarks>
  /// <para>
  /// In order for the click event to trigger the key release event must happen within the
  /// <see cref="ClickDelay"/> delay after the preceding press event.
  /// </para>
  /// <para>
  /// Remember to remove the listeners when their owner class is destroyed by the game. If it's not
  /// done then no NRE will happen, but the "ghost" listeners will continue to react on the events.  
  /// </para>  
  /// </remarks>
  /// <seealso cref="ClickDelay"/>
  public event Callback OnClick;
  #endregion

  /// <summary>Last press event timestamp.</summary>
  float lastPressTime;

  #region IConfigNode implementation
  /// <summary>Loads a persisted switch binding.</summary>
  /// <param name="node">The node to get values from.</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  public virtual void Load(ConfigNode node) {
    ConfigAccessor.GetValueByPath(node, "keyCode", ref keyCode);
  }

  /// <summary>Saves the switch binding.</summary>
  /// <param name="node">The node to store the values into.</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  public virtual void Save(ConfigNode node) {
    node.SetValue("keyCode", keyCode.ToString(), createIfNotFound: true);
  }
  #endregion

  /// <summary>
  /// Creates a switch with a <see cref="KeyCode.None"/> key binding. It's a default constructor
  /// needed for the <see cref="PersistentFieldAttribute"/> functionality to work.
  /// </summary>
  public KeyboardInputSwitch() : this(KeyCode.None) {
  }

  /// <summary>Main constructor to create a switch for the provided key code.</summary>
  /// <param name="code">
  /// The key code to activate the switch. Can be <see cref="KeyCode.None"/> in which case this
  /// switch can only be changed via the code.
  /// </param>
  public KeyboardInputSwitch(KeyCode code) {
    this.keyCode = code;
  }

  /// <summary>Checks the keyboard status and updates the switch accordingly.</summary>
  /// <remarks>
  /// This method handles the game's pause and time warp modes, and disables the key handling in
  /// these modes. It also respects the UI locking mode set by the game.
  /// </remarks>
  /// <returns>The current hold state.</returns>
  /// <seealso cref="keyboardEnabled"/>
  public virtual bool Update() {
    if (!keyboardEnabled) {
      return false;
    }
    if (!Input.GetKey(keyCode)
        || Mathf.Approximately(Time.timeScale, 0f)
        || Time.timeScale > 1f
        || InputLockManager.IsLocked(ControlTypes.UI)) {
      SetHoldState(false);
    } else if (Input.GetKey(keyCode) && !isAnyKeyHold) {
      SetHoldState(true);
    }
    return isHold;
  }

  /// <summary>Updates the hold state and triggers the event(s) if any.</summary>
  /// <remarks>This method can be used to simulate a click event.</remarks>
  /// <param name="newState">The new hold state.</param>
  /// <seealso cref="OnStateChanged"/>
  /// <seealso cref="OnClick"/>
  protected virtual void SetHoldState(bool newState) {
    if (_isHold != newState) {
      _isHold = newState;
      if (newState) {
        keysHold++;
      } else {
        keysHold--;
      }

      // Fire events.
      if (OnStateChanged != null) {
        OnStateChanged();
      }
      if (isHold) {
        lastPressTime = Time.realtimeSinceStartup;
        if (OnPress != null) {
          OnPress();
        }
      } else {
        if (OnRelease != null) {
          OnRelease();
        }
        if (lastPressTime + ClickDelay > Time.realtimeSinceStartup && OnClick != null) {
          OnClick();
        }
      }
    }
  }
}

}  // namespace
