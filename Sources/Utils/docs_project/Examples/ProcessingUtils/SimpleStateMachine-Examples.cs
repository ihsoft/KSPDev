// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ProcessingUtils;
using KSPDev.LogUtils;
using System;
using UnityEngine;

namespace Examples {

#region SimpleStateMachine1
/// <summary>A class that demonstrates a simple state module with three states.</summary>
/// <remarks>
/// There are the following rules for the state changes:
/// <list type="bullet">
/// <item>The state <c>One</c> can be transitioned into both <c>Two</c> and <c>Three</c>.</item>
/// <item>The states <c>Two</c> and <c>Three</c> can only return back to <c>One</c>.</item>
/// <item>In states <c>Two</c> and <c>Three</c> different menu options are available.</item>
/// <item>In state <c>One</c> no menu options are available.</item>
/// </list>
/// </remarks>
class SimpleStateMachine1 : PartModule {
  public enum State {
    One,
    Two,
    Three
  }

  [KSPField(isPersistant = true)]
  public State persistedState = State.One;  // ALWAYS provide a default value!

  SimpleStateMachine<State> stateMachine;

  [KSPEvent(guiName = "State: TWO")]
  public void StateTwoMenuAction() {
    Debug.Log("StateTwoMenuAction()");
  }

  [KSPEvent(guiName = "State: THREE")]
  public void StateThreeMenuAction() {
    Debug.Log("StateThreeMenuAction()");
  }

  public override void OnAwake() {
    stateMachine = new SimpleStateMachine<State>(strict: true);
    // State ONE can be transitioned into both TWO and THREE.
    stateMachine.SetTransitionConstraint(
        State.One,
        new[] {State.Two, State.Three});
    // State TWO can only get back to ONE.
    stateMachine.SetTransitionConstraint(
        State.Two,
        new[] {State.One});
    // State THREE can only get back to ONE.
    stateMachine.SetTransitionConstraint(
        State.Three,
        new[] {State.One});
    // No menus available in state ONE.
    stateMachine.AddStateHandlers(
        State.One,
        enterHandler: oldState => {
          Events["StateTwoMenuAction"].active = false;
          Events["StateThreeMenuAction"].active = false;
        },
        leaveHandler: newState => Debug.LogFormat("Move from ONE to {0}", newState));
    // Only TWO-menu is available in the state TWO.
    stateMachine.AddStateHandlers(
        State.Two,
        enterHandler: oldState => {
          Events["StateTwoMenuAction"].active = true;
          Events["StateThreeMenuAction"].active = false;
        });
    // Only THREE-menu is available in the state THREE.
    stateMachine.AddStateHandlers(
        State.Three,
        enterHandler: oldState => {
          Events["StateTwoMenuAction"].active = false;
          Events["StateThreeMenuAction"].active = true;
        });
  }

  public override void OnStart(PartModule.StartState state) {
    stateMachine.currentState = persistedState;  // Restore state from the save file.
  }

  void OnDestory() {
    // Usually, this isn't needed. But if code needs to do a cleanup job it makes sense to wrap
    // it into a handler, and stop the machine in the Unity destructor.
    stateMachine.currentState = null;
  }

  public override void OnUpdate() {
    if (Input.GetKeyDown("1")) {
      // This transition will always succceed. 
      stateMachine.currentState = State.One;
    }
    if (Input.GetKeyDown("2")) {
      // This transition will only succceed if current state is MyState.One. 
      stateMachine.currentState = State.Two;
    }
    if (Input.GetKeyDown("3")) {
      // This transition will only succceed if current state is MyState.One. 
      stateMachine.currentState = State.Three;
    }
  }
}
#endregion

#region SimpleStateMachineStrict
class SimpleStateMachineStrict {
  public enum State {
    One,
    Two,
    Three
  };

  public void TestMachine() {
    var sm = new SimpleStateMachine<State>(strict: true);
    sm.SetTransitionConstraint(State.One, new State[] { State.Two });
    sm.AddStateHandlers(
        State.One,
        enterHandler: newState => LogFromTo(sm.currentState, newState),
        leaveHandler: newState => LogFromTo(sm.currentState, newState));

    sm.currentState = State.One;  // Start the machine.
    // Logs: Move from NULL to One

    if (sm.CheckCanSwitchTo(State.Two)) {
      sm.currentState = State.Two;  // An allowed tranistion.
      // Logs: Move from One to Two
    }

    if (sm.CheckCanSwitchTo(State.Three)) {
      // This will never happen.
      sm.currentState = State.Three;
    }

    sm.currentState = null;  // Stop the machine.
    // Logs: Move Two to NULL
  }

  void LogFromTo(State? from, State? to) {
    Debug.LogFormat(
        "Move from {0} to {1}", DbgFormatter.Nullable(from), DbgFormatter.Nullable(to));
  }
}
#endregion

#region SimpleStateMachineFree
class SimpleStateMachineFree {
  public enum State {
    One,
    Two,
    Three
  };

  public void TestMachine() {
    var sm = new SimpleStateMachine<State>(strict: false);
    sm.AddStateHandlers(
        State.One,
        enterHandler: oldState => Debug.Log("Now in state ONE"),
        leaveHandler: newState => Debug.LogFormat("Going into state: {0}", newState));
    sm.onAfterTransition += (from, to) => Debug.LogFormat(
        "Move from {0} to {1}", DbgFormatter.Nullable(from), DbgFormatter.Nullable(to));

    sm.currentState = State.One;  // Start the machine.
    // Logs:
    // Now in state ONE
    // Move from NULL to One

    sm.currentState = State.Two;
    // Logs:
    // Going into state: Two
    // Move from One to Two

    sm.currentState = State.Three;
    // Logs: Move from Two to Three

    sm.currentState = null;  // Stop the machine.
    // Logs: Move Three to NULL
  }
}
#endregion

}  // namespace

