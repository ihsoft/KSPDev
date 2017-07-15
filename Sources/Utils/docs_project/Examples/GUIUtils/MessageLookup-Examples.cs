// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Examples {

#region MessageLookupDemo_Simple
public class MessageLookupDemo1 : PartModule {
  enum MyEnum {
    One,
    Two,
    Three
  }

  // Create the messages separately from the lookup to allow the LocalizationTool to pick them up.
  static readonly Message msg_1 = new Message("#msg1", defaultTemplate: "-ONE-");
  static readonly Message msg_2 = new Message("#msg2", defaultTemplate: "-TWO-");
  static readonly Message msg_3 = new Message("#msg3", defaultTemplate: "-THREE-");

  static readonly MessageLookup<MyEnum> msg = new MessageLookup<MyEnum>(
      new Dictionary<MyEnum, Message>() {
          {MyEnum.One, msg_1},
          {MyEnum.Two, msg_2},
          {MyEnum.Three, msg_3},
      });

  // Depending on the current language in the system, this method will present the values in
  // different languages.
  void Show() {
    Debug.Log(msg.Lookup(MyEnum.One));
    // Prints: "-ONE-"
    Debug.Log(msg.Lookup(MyEnum.Two));
    // Prints: "-TWO-"
    Debug.Log(msg.Lookup(MyEnum.Three));
    // Prints: "-THREE-"
  }
}
#endregion

#region MessageLookupDemo_WithDefault
public class MessageLookupDemo2 : PartModule {
  enum MyEnum {
    One,
    Two,
    Three,
  }

  // Create the messages separately from the lookup to allow the LocalizationTool to pick them up.
  static readonly Message msg_1 = new Message("#msg1", defaultTemplate: "-ONE-");
  static readonly Message defMsg = new Message("#def", defaultTemplate: "-DEFAULT-");

  static readonly MessageLookup<MyEnum> msg = new MessageLookup<MyEnum>(
      new Dictionary<MyEnum, Message>() {
          {MyEnum.One, msg_1},
      }, defaultMessage: defMsg);

  void Show() {
    Debug.Log(msg.Lookup(MyEnum.One));
    // Prints: "-ONE-"
    Debug.Log(msg.Lookup(MyEnum.Two));
    // Prints: "-DEFAULT-"
    Debug.Log(msg.Lookup(MyEnum.Three));
    // Prints: "-DEFAULT-"
  }
}
#endregion

#region MessageLookupDemo_WithStockDefault
public class MessageLookupDemo3 : PartModule {
  enum MyEnum {
    One,
    Two,
    Three,
  }

  // Create the messages separately from the lookup to allow the LocalizationTool to pick them up.
  static readonly Message msg_1 = new Message("#msg1", defaultTemplate: "-ONE-");

  static readonly MessageLookup<MyEnum> msg = new MessageLookup<MyEnum>(
      new Dictionary<MyEnum, Message>() {
          {MyEnum.One, msg_1},
      });

  void Show() {
    Debug.Log(msg.Lookup(MyEnum.One));
    // Prints: "-ONE-"
    Debug.Log(msg.Lookup(MyEnum.Two));
    // Prints: "#NONE#"
    Debug.Log(msg.Lookup(MyEnum.Three, noDefault: true) == null ? "NOT found" : "FOUND");
    // Prints: "NOT found"
  }
}
#endregion

}  // namespace
