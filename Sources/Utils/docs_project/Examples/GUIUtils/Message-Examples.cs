// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.LogUtils;
using KSPDev.GUIUtils;

namespace Examples {

#region MessageDemo
public class MessageDemo : PartModule {
  // The encouraged way of defining a message.
  static readonly Message msg1 = new Message(
      "#myLocalizationTag",
      defaultTemplate: "Sample text in English",
      description: "A string to present in the KSPDevUtils documentation example. It illustrates"
      + " how the class can be used to localize a message.",
      example: "Format() => Sample text in English");

  // A simple way when no extra details are provided.
  static readonly Message msg2 = "#myLocalizationTag";

  public override void OnAwake() {
    base.OnAwake();

    // This will load the localized string and print it into the log.
    HostedDebugLog.Info(this, msg1.Format());

    // The next example will only work if there is a localizable string defined. Otherwise, it will
    // print the tag.
    HostedDebugLog.Info(this, msg2.Format());

    // A simple message can be just casted to string to get the localized content.
    PrintString(msg1);
  }

  void PrintString(string str) {
    HostedDebugLog.Info(this, str);
  }
}
#endregion

#region Message1Demo
public class Message1Demo : PartModule {
  // The encouraged way of defining a message.
  static readonly Message<int> msg1 = new Message<int>(
      "#myLocalizationTag",
      defaultTemplate: "The value is <<1>>",
      description: "A string to present in the KSPDevUtils documentation example. It illustrates"
      + " how the class can be used to localize a message.",
      example: "Format(123) => The value is 123");

  // A simple way when no extra details are provided.
  static readonly Message<int> msg2 = "#myLocalizationTag";

  public override void OnAwake() {
    base.OnAwake();

    // This will load the localized string and print it into the log.
    HostedDebugLog.Info(this, msg1.Format(123));

    // The next example will only work if there is a localizable string defined. Otherwise, it will
    // print the tag.
    HostedDebugLog.Info(this, msg2.Format(123));
  }
}
#endregion

#region Message2Demo
public class Message2Demo : PartModule {
  // The encouraged way of defining a message.
  static readonly Message<string, int> msg1 = new Message<string, int>(
      "#myLocalizationTag",
      defaultTemplate: "The value of <<1>> is <<2>>",
      description: "A string to present in the KSPDevUtils documentation example. It illustrates"
      + " how the class can be used to localize a message.",
      example: "Format(\"Blah\", 123) => The value of Blah is 123");

  // A simple way when no extra details are provided.
  static readonly Message<string, int> msg2 = "#myLocalizationTag";

  public override void OnAwake() {
    base.OnAwake();

    // This will load the localized string and print it into the log.
    HostedDebugLog.Info(this, msg1.Format("Blah", 123));

    // The next example will only work if there is a localizable string defined. Otherwise, it will
    // print the tag.
    HostedDebugLog.Info(this, msg2.Format("Blah", 123));
  }
}
#endregion

#region Message3Demo
public class Message3Demo : PartModule {
  // The encouraged way of defining a message.
  static readonly Message<string, int, float> msg1 = new Message<string, int, float>(
      "#myLocalizationTag",
      defaultTemplate: "The value of <<1>> is <<2>> or <<3>>",
      description: "A string to present in the KSPDevUtils documentation example. It illustrates"
      + " how the class can be used to localize a message.",
      example: "Format(\"Blah\", 123, 123.5f) => The value of Blah is 123 or 123.5");

  // A simple way when no extra details are provided.
  static readonly Message<string, int, float> msg2 = "#myLocalizationTag";

  public override void OnAwake() {
    base.OnAwake();

    // This will load the localized string and print it into the log.
    HostedDebugLog.Info(this, msg1.Format("Blah", 123, 123.5f));

    // The next example will only work if there is a localizable string defined. Otherwise, it will
    // print the tag.
    HostedDebugLog.Info(this, msg2.Format("Blah", 123, 123.5f));
  }
}
#endregion

#region Message4Demo
public class Message4Demo : PartModule {
  // The encouraged way of defining a message.
  static readonly Message<string, int, string, float> msg1 =
      new Message<string, int, string, float>(
          "#myLocalizationTag",
          defaultTemplate: "<<1>> = <<2>>, <<3>> = <<4>>",
          description: "A string to present in the KSPDevUtils documentation example. It"
          + " illustrates how the class can be used to localize a message.",
          example: "Format(\"val1\", 123, \"val2\", 123.5f) => val1 = 123, val2 = 123.5");

  // A simple way when no extra details are provided.
  static readonly Message<string, int, string, float> msg2 = "#myLocalizationTag";

  public override void OnAwake() {
    base.OnAwake();

    // This will load the localized string and print it into the log.
    HostedDebugLog.Info(this, msg1.Format("val1", 123, "val2", 123.5f));

    // The next example will only work if there is a localizable string defined. Otherwise, it will
    // print the tag.
    HostedDebugLog.Info(this, msg2.Format("val1", 123, "val2", 123.5f));
  }
}
#endregion

#region Message5Demo
public class Message5Demo : PartModule {
  // The encouraged way of defining a message.
  static readonly Message<string, int, string, int, float> msg1 =
      new Message<string, int, string, int, float>(
          "#myLocalizationTag",
          defaultTemplate: "<<1>> = <<2>>, <<3>> = <<4>>, avg = <<5>>",
          description: "A string to present in the KSPDevUtils documentation example. It"
          + " illustrates how the class can be used to localize a message.",
          example: "Format(\"v1\", 1, \"v2\", 2, 1.5f) => v1 = 1, v2 = 2, avg = 1.5");

  // A simple way when no extra details are provided.
  static readonly Message<string, int, string, int, float> msg2 = "#myLocalizationTag";

  public override void OnAwake() {
    base.OnAwake();

    // This will load the localized string and print it into the log.
    HostedDebugLog.Info(this, msg1.Format("v1", 1, "v2", 2, 1.5f));

    // The next example will only work if there is a localizable string defined. Otherwise, it will
    // print the tag.
    HostedDebugLog.Info(this, msg2.Format("v1", 1, "v2", 2, 1.5f));
  }
}
#endregion

}  // namespace
