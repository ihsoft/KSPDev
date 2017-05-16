// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using KSPDev.Extensions;

namespace KSPDev.ProcessingUtils {

/// <summary>
/// Simple state machine that allows tracking of the states and checking the basic transition
/// conditions.
/// </summary>
/// <remarks>
/// If a module has more that two modes (which can be controlled by a simple boolean) it makes sense
/// to define each mode as a state, and introduce a definite state transition diagram. Once it's
/// done, a state machine can be setup by defining which transitions are allowed. At this point the
/// module will be able to just react on the state change events instead of checking multiple
/// conditions.
/// </remarks>
/// <typeparam name="T">The enum to use as the state constants.</typeparam>
/// <example>
/// Let's pretend there is a module with three states:
/// <list type="bullet">
/// <item>The state <c>One</c> can be transitioned into both <c>Two</c> and <c>Three</c>.</item>
/// <item>The states <c>Two</c> and <c>Three</c> can only return back to <c>One</c>.</item>
/// <item>In states <c>Two</c> and <c>Three</c> different menu options are available.</item>
/// <item>In state <c>One</c> no menu options are available.</item>
/// </list>
/// <code><![CDATA[
/// class MyModule : PartModule {
///   enum MyState {
///     One, Two, Three
///   }
///
///   [KSPField(isPersistant = true)]
///   public MyState persistedState = MyState.One;  // ALWAYS provide a default value!
///
///   SimpleStateMachine<MyState> linkStateMachine;
///
///   [KSPEvent(guiName = "State: TWO")]
///   public void StateTwoMenuAction() {
///     Debug.LogInfo("StateTwoMenuAction()");
///   }
///
///   [KSPEvent(guiName = "State: THREE")]
///   public void StateThreeMenuAction() {
///     Debug.LogInfo("StateThreeMenuAction()");
///   }
///
///   public override OnAwake() {
///     linkStateMachine = new SimpleStateMachine<MyState>(true /* strict */);
///     linkStateMachine.SetTransitionConstraint(
///         MyState.One,
///         new[] {MyState.Two, MyState.Three});
///     linkStateMachine.SetTransitionConstraint(
///         MyState.Two,
///         new[] {MyState.One});
///     linkStateMachine.SetTransitionConstraint(
///         MyState.Three,
///         new[] {MyState.One});
///     linkStateMachine.AddStateHandlers(
///         MyState.One,
///         enterHandler: x => {
///           Events["StateTwoMenuAction"].active = false;
///           Events["StateThreeMenuAction"].active = false;
///         });
///     linkStateMachine.AddStateHandlers(
///         MyState.Two,
///         enterHandler: x => {
///           Events["StateTwoMenuAction"].active = true;
///           Events["StateThreeMenuAction"].active = false;
///         });
///     linkStateMachine.AddStateHandlers(
///         MyState.Three,
///         enterHandler: x => {
///           Events["StateTwoMenuAction"].active = false;
///           Events["StateThreeMenuAction"].active = true;
///         });
///   }
///
///   public override void OnStart(PartModule.StartState state) {
///     linkStateMachine.Start(persistedState);  // Restore state from the save file.
///   }
///
///   void OnDestory() {
///     // Usually, this isn't needed. But if code needs to do a cleanup job it makes sense to wrap
///     // it into a handler, and stop the machine in Unity destructor.
///     linkStateMachine.Stop();
///   }
///
///   public override OnUpdate() {
///     if (Input.GetKeyDown("1")) {
///       // This transition will always succceed. 
///       stateMachine.currentState = MyState.One;
///     }
///     if (Input.GetKeyDown("2")) {
///       // This transition will only succceed if current state is MyState.One. 
///       stateMachine.currentState = MyState.Two;
///     }
///     if (Input.GetKeyDown("3")) {
///       // This transition will only succceed if current state is MyState.One. 
///       stateMachine.currentState = MyState.Three;
///     }
///   }
/// }
/// ]]></code>
/// <para>
/// The same logic could be achivied in a different way. Instead of enabling/disabling all the menu
/// items in every "enter" handler the code could define "leave" handlers that would disable the
/// related menu item. This way every state handler would control its own menu item without
/// interacting with any existing or the future items.
/// </para>
/// </example>
public sealed class SimpleStateMachine<T> where T : struct, IConvertible {
  /// <summary>Current state of the machine.</summary>
  /// <remarks>
  /// Setting the same state as the current one is a NO-OP. Setting of a new state may throw an
  /// exception in the strict mode.
  /// </remarks>
  /// <value>The current state.</value>
  /// <seealso cref="isStrict"/>
  /// <seealso cref="ForceSetState"/>
  public T currentState {
    get { return _currentState; }
    set { SetState(value); }
  }
  T _currentState;

  /// <summary>Tells if the state machine is started.</summary>
  /// <value>The started state.</value>
  /// <seealso cref="Start"/>
  public bool isStarted { get; private set; }

  /// <summary>Tells if all the transitions must be excplicitly declared.</summary>
  /// <remarks>
  /// The state transitions are defined via <see cref="SetTransitionConstraint"/>.
  /// </remarks>
  /// <value>The strict mode state.</value>
  /// <seealso cref="SetTransitionConstraint"/>
  /// <seealso cref="ResetTransitionConstraint"/>
  public bool isStrict { get; private set; }

  /// <summary>Delegate for a callback which notifies about a state change.</summary>
  /// <param name="state">The current state of the machine.</param>
  public delegate void OnChange(T state);

  /// <summary>Special debug delegate to track the state changes.</summary>
  /// <remarks>This callback is called before the actual state change.</remarks>
  /// <param name="fromState">The state before the change.</param>
  /// <param name="toState">The state after the change.</param>
  public delegate void OnDebugChange(T fromState, T toState);

  /// <summary>
  /// Debug handler to track the state changes. Avoid using it in the normal code logic.
  /// </summary>
  public OnDebugChange OnDebugStateChange;

  readonly Dictionary<T, HashSet<OnChange>> enterHandlers = new Dictionary<T, HashSet<OnChange>>();
  readonly Dictionary<T, HashSet<OnChange>> leaveHandlers = new Dictionary<T, HashSet<OnChange>>();
  readonly Dictionary<T, T[]> transitionContstraints = new Dictionary<T, T[]>();

  /// <summary>Constructs a new unstarted state machine.</summary>
  /// <param name="strict">The strict mode.</param>
  /// <seealso cref="isStrict"/>
  public SimpleStateMachine(bool strict) {
    isStrict = strict;
  }

  /// <summary>Starts the state machine and makes it available for the state transitions.</summary>
  /// <remarks>
  /// <para>
  /// Until the machine is started, the state transitions are not possible. An attempt to move the
  /// machine into any state will result in a <see cref="InvalidOperationException"/> exception.
  /// </para>
  /// <para>Starting of the machine will trigger an enter state event.</para>
  /// </remarks>
  /// <param name="startState">The initial state of the machine.</param>
  /// <seealso cref="isStarted"/>
  /// <seealso cref="AddStateHandlers"/>
  public void Start(T startState) {
    CheckIsNotStarted();
    isStarted = true;
    _currentState = startState;
    if (OnDebugStateChange != null) {
      OnDebugStateChange(_currentState, _currentState);
    }
    FireEnterState();
  }

  /// <summary>Stops the state machine making it unavailable for any state transition.</summary>
  /// <remarks>
  /// If the machine is not started then this call is a NO-OP. Stoping of the started machine will
  /// trigger a leave state event.
  /// </remarks>
  /// <seealso cref="isStarted"/>
  /// <seealso cref="AddStateHandlers"/>
  public void Stop() {
    if (isStarted) {
      if (OnDebugStateChange != null) {
        OnDebugStateChange(_currentState, _currentState);
      }
      FireLeaveState();
      isStarted = false;
    }
  }

  /// <summary>Defines a state and the allowed target states for it.</summary>
  /// <remarks>
  /// In the strict mode it's required that every transition is declared excplicitly.
  /// </remarks>
  /// <param name="fromState">The source state.</param>
  /// <param name="toStates">The list of the states that are allowed as the targets.</param>
  /// <seealso cref="isStrict"/>
  public void SetTransitionConstraint(T fromState, T[] toStates) {
    CheckIsNotStarted();
    transitionContstraints.Remove(fromState);
    transitionContstraints.Add(fromState, toStates);
  }

  /// <summary>Clears the transitions for the source state if any.</summary>
  /// <param name="fromState">The source state to clear the tarnsitions for.</param>
  /// <seealso cref="isStrict"/>
  public void ResetTransitionConstraint(T fromState) {
    CheckIsNotStarted();
    transitionContstraints.Remove(fromState);
  }

  /// <summary>Changes the current state bypassing all the transition checks.</summary>
  /// <remarks>
  /// It's discouraged to use this method in the normal flow. However, it may be handy when
  /// recovering a module from an unknown state (e.g. an unexpected exception was thrown in the
  /// middle of the process).
  /// </remarks>
  /// <param name="newState">
  /// The new state. It can be a state that is not mentioned in any state transition.
  /// </param>
  public void ForceSetState(T newState) {
    CheckIsStarted();
    if (OnDebugStateChange != null) {
      OnDebugStateChange(_currentState, newState);
    }
    if (isStarted) {
      FireLeaveState();
    }
    _currentState = newState;
    FireEnterState();
  }

  /// <summary>Adds a state change event.</summary>
  /// <remarks>
  /// Note, that the code must not expect that the handlers will be called in the same order as they
  /// were added. Each handler must be independent from the others.
  /// </remarks>
  /// <param name="state">The state to call a callback on.</param>
  /// <param name="enterHandler">
  /// The callback to call when the state machine has switched to a new state. The callback is
  /// triggered <i>after</i> the state has actually changed.
  /// </param>
  /// <param name="leaveHandler">
  /// The callback to call when the state machine is going to leave the current state. The callback
  /// is triggered <i>before</i> the state has actually changed. 
  /// </param>
  public void AddStateHandlers(
      T state, OnChange enterHandler = null, OnChange leaveHandler = null) {
    CheckIsNotStarted();
    if (enterHandler != null) {
      enterHandlers.SetDefault(state).Add(enterHandler);
    }
    if (leaveHandler != null) {
      leaveHandlers.SetDefault(state).Add(leaveHandler);
    }
  }

  /// <summary>Removes a state change event handler.</summary>
  /// <remarks>It's safe to call it for a non-existing handler.</remarks>
  /// <param name="state">The state to delete a handler for.</param>
  /// <param name="enterHandler">The enter state handler to delete.</param>
  /// <param name="leaveHandler">The leave state handler to delete.</param>
  public void RemoveHandlers(T state, OnChange enterHandler = null, OnChange leaveHandler = null) {
    CheckIsNotStarted();
    if (enterHandler != null && enterHandlers.ContainsKey(state)) {
      enterHandlers[state].Remove(enterHandler);
    }
    if (leaveHandler != null && leaveHandlers.ContainsKey(state)) {
      leaveHandlers[state].Remove(leaveHandler);
    }
  }

  /// <summary>Verifies if the machine can move into the desired state.</summary>
  /// <param name="newState">The state to check the transition for.</param>
  /// <returns><c>true</c> if the transition is allowed.</returns>
  /// <seealso cref="isStrict"/>
  /// <seealso cref="SetTransitionConstraint"/>
  public bool CheckCanSwitchTo(T newState) {
    return !isStrict
        || transitionContstraints.ContainsKey(_currentState)
           && transitionContstraints[_currentState].IndexOf(newState) != -1;
  }

  #region Local utility methods
  /// <summary>Verifies that the state machine is started.</summary>
  /// <exception cref="InvalidOperationException">If state machine is not yet started.</exception>
  void CheckIsStarted() {
    if (!isStarted) {
      throw new InvalidOperationException("Not allowed in STOPPED state");
    }
  }

  /// <summary>Verifies that the state machine is <i>not</i> started.</summary>
  /// <exception cref="InvalidOperationException">If state machine is already started.</exception>
  void CheckIsNotStarted() {
    if (isStarted) {
      throw new InvalidOperationException("Not allowed in STARTED state");
    }
  }

  /// <summary>
  /// Changes the machine's state if the current and the new states are different. Checks if the
  /// transition is allowed before actually changing the state.
  /// </summary>
  /// <param name="newState">The state to change to.</param>
  /// <seealso cref="CheckCanSwitchTo"/>
  /// <exception cref="InvalidOperationException">If the transition is not allowed.</exception>
  void SetState(T newState) {
    CheckIsStarted();
    if (!_currentState.Equals(newState)) {
      if (!CheckCanSwitchTo(newState)) {
        throw new InvalidOperationException(string.Format(
            "Transition {0}=>{1} is not allowed", _currentState, newState));
      }
      ForceSetState(newState);
    }
  }

  /// <summary>Notifies all the handlers about leaving the current state.</summary>
  void FireLeaveState() {
    HashSet<OnChange> handlers;
    if (leaveHandlers.TryGetValue(_currentState, out handlers)) {
      foreach (var @event in handlers) {
        @event.Invoke(_currentState);
      }
    }
  }

  /// <summary>Notifies all the handlers about entering a new state.</summary>
  void FireEnterState() {
    HashSet<OnChange> handlers;
    if (enterHandlers.TryGetValue(_currentState, out handlers)) {
      foreach (var @event in handlers) {
        @event.Invoke(_currentState);
      }
    }
  }
  #endregion
}

}  // namespace
