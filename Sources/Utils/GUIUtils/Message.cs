// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.GUIUtils {

/// <summary>A class to wrap a simple UI string.</summary>
/// <remarks>
/// <para>Messages of this type don't have parameters and can be just casted to a string.</para> 
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>Message</c>. Don't declare them <c>readonly</c> since it will block future
/// localization.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   Message MyMessage = "This is a message without parameters";
///
///   void Awake() {
///     Debug.Log("Localized: {0}", MyMessage);
///   }
/// }
/// ]]></code>
/// <para>Note, that it's OK to name such members as constants in spite of they are not constants by
/// the C# language semantics. I.e. instead of <c>myMessage</c> you spell <c>MyMessage</c> to
/// highlight the fact it won't (and must not) change from the code.</para>  
/// </example>
public struct Message {
  readonly string messageString;

  /// <summary>Creates a message.</summary>
  /// <param name="messageString">A message string.</param>
  public Message(string messageString) {
    this.messageString = messageString;
  }

  /// <summary>Allows casting string to message.</summary>
  /// <remarks>Instead of creating new insatnce for every string just do the cast as it was a
  /// regular literal constant.</remarks>
  /// <param name="messageString">A string value to assign.</param>
  /// <returns>Message instance.</returns>
  public static implicit operator Message(string messageString) {
    return new Message(messageString);
  }

  /// <summary>Allows casting messages to string.</summary>
  /// <param name="msg">A message to cast.</param>
  /// <returns>Message value.</returns>
  public static implicit operator string(Message msg) {
    return msg.messageString;
  }
}

/// <summary>A class to wrap a UI string with one parameter.</summary>
/// <typeparam name="T1">Type of the first substitute argument in the string.</typeparam>
/// <remarks>
/// <para>Define parameter type via generic argument. When string needs to be presented use
/// <see cref="Format"/> to make the parameter substitute.</para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>Message</c>. Don't declare them <c>readonly</c> since it will block future
/// localization.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   Message<string> MyMessage = "Param: {0}";
///
///   void Awake() {
///     Debug.Log("Localized: {0}", MyMessage.Format("Blah!"));
///   }
/// }
/// ]]></code>
/// <para>Note, that it's OK to name such members as constants in spite of they are not constants by
/// the C# language semantics. I.e. instead of <c>myMessage</c> you spell <c>MyMessage</c> to
/// highlight the fact it won't (and must not) change from the code.</para>  
/// </example>
public struct Message<T1> {
  readonly string fmtString;
  
  /// <summary>Creates a message.</summary>
  /// <param name="fmtString">A message format string.</param>
  public Message(string fmtString) {
    this.fmtString = fmtString;
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(T1 arg1) {
    return string.Format(fmtString, arg1);
  }

  /// <summary>Allows casting strings to message.</summary>
  /// <param name="fmtString">A string value to assign.</param>
  /// <returns>Message instance.</returns>
  public static implicit operator Message<T1>(string fmtString) {
    return new Message<T1>(fmtString);
  }
}

/// <summary>
/// A class to wrap a UI string with one parameter which may have special meaning.
/// </summary>
/// <remarks>
/// <para>When string needs to be presented use <see cref="Format"/> to make the parameter
/// substitute.</para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>Message</c>. Don't declare them <c>readonly</c> since it will block future
/// localization.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   MessageSpecialFloatValue MyMessage =
///       new MessageSpecialFloatValue("Param: {0}", 0, "Param is ZERO");
///
///   void Awake() {
///     Debug.Log("Localized: {0}", MyMessage.Format(1));  // Param: 1
///     Debug.Log("Localized: {0}", MyMessage.Format(0));  // Param is ZERO
///   }
/// }
/// ]]></code>
/// <para>Note, that it's OK to name such members as constants in spite of they are not constants by
/// the C# language semantics. I.e. instead of <c>myMessage</c> you spell <c>MyMessage</c> to
/// highlight the fact it won't (and must not) change from the code.</para>  
/// </example>
public struct MessageSpecialFloatValue {
  readonly string fmtString;
  readonly string specialValueString;
  readonly float specialValue;
  
  /// <summary>Creates a message.</summary>
  /// <param name="fmtString">A message format string.</param>
  /// <param name="specialValue">Value to use a special message string for.</param>
  /// <param name="specialString">Special message string for the value.</param>
  public MessageSpecialFloatValue(string fmtString, float specialValue, string specialString) {
    this.fmtString = fmtString;
    this.specialValueString = specialString;
    this.specialValue = specialValue;
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(float arg1) {
    if (UnityEngine.Mathf.Approximately(arg1, specialValue)) {
      return specialValueString;
    }
    return string.Format(fmtString, arg1);
  }
}

/// <summary>A class to wrap a UI string with two parameters.</summary>
/// <typeparam name="T1">Type of the first substitute argument in the string.</typeparam>
/// <typeparam name="T2">Type of the second substitute argument in the string.</typeparam>
/// <remarks>
/// <para>Define parameter types via generic argument. When string needs to be presented use
/// <see cref="Format"/> to make parameters substitute.</para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>Message</c>. Don't declare them <c>readonly</c> since it will block future
/// localization.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   Message<string, int> MyMessage = "Params: {0}, {1}";
///
///   void Awake() {
///     Debug.Log("Localized: {0}", MyMessage.Format("Blah!", 123));
///   }
/// }
/// ]]></code>
/// <para>Note, that it's OK to name such members as constants in spite of they are not constants by
/// the C# language semantics. I.e. instead of <c>myMessage</c> you spell <c>MyMessage</c> to
/// highlight the fact it won't (and must not) change from the code.</para>  
/// </example>
public struct Message<T1, T2> {
  readonly string fmtString;
  
  /// <summary>Creates a message.</summary>
  /// <param name="fmtString">A message format string.</param>
  public Message(string fmtString) {
    this.fmtString = fmtString;
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <param name="arg2">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(T1 arg1, T2 arg2) {
    return string.Format(fmtString, arg1, arg2);
  }

  /// <summary>Allows casting strings to message.</summary>
  /// <param name="fmtString">A string value to assign.</param>
  /// <returns>Message instance.</returns>
  public static implicit operator Message<T1, T2>(string fmtString) {
    return new Message<T1, T2>(fmtString);
  }
}

/// <summary>A class to wrap a UI string with three parameters.</summary>
/// <typeparam name="T1">Type of the first substitute argument in the string.</typeparam>
/// <typeparam name="T2">Type of the second substitute argument in the string.</typeparam>
/// <typeparam name="T3">Type of the third substitute argument in the string.</typeparam>
/// <remarks>
/// <para>Define parameter types via generic argument. When string needs to be presented use
/// <see cref="Format"/> to make parameters substitute.</para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>Message</c>.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   Message<string, int, float> MyMessage = "Params: {0}, {1}, {2}";
///
///   void Awake() {
///     Debug.Log("Localized: {0}", MyMessage.Format("Blah!", 123, 321f));
///   }
/// }
/// ]]></code>
/// <para>Note, that it's OK to name such members as constants in spite of they are not constants by
/// the C# language semantics. I.e. instead of <c>myMessage</c> you spell <c>MyMessage</c> to
/// highlight the fact it won't (and must not) change from the code.</para>  
/// </example>
public struct Message<T1, T2, T3> {
  readonly string fmtString;
  
  /// <summary>Creates a message.</summary>
  /// <param name="fmtString">A message format string.</param>
  public Message(string fmtString) {
    this.fmtString = fmtString;
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <param name="arg2">An argument to substitute.</param>
  /// <param name="arg3">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(T1 arg1, T2 arg2, T3 arg3) {
    return string.Format(fmtString, arg1, arg2, arg3);
  }

  /// <summary>Allows casting strings to message.</summary>
  /// <param name="fmtString">A string value to assign.</param>
  /// <returns>Message instance.</returns>
  public static implicit operator Message<T1, T2, T3>(string fmtString) {
    return new Message<T1, T2, T3>(fmtString);
  }
}

/// <summary>A class to wrap a UI string with four parameters.</summary>
/// <typeparam name="T1">Type of the first substitute argument in the string.</typeparam>
/// <typeparam name="T2">Type of the second substitute argument in the string.</typeparam>
/// <typeparam name="T3">Type of the third substitute argument in the string.</typeparam>
/// <typeparam name="T4">Type of the fourth substitute argument in the string.</typeparam>
/// <remarks>
/// <para>Define parameter types via generic argument. When string needs to be presented use
/// <see cref="Format"/> to make parameters substitute.</para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>Message</c>.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   Message<string, int, float, int> MyMessage = "Params: {0}, {1}, {2}, {3}";
///
///   void Awake() {
///     Debug.Log("Localized: {0}", MyMessage.Format("Blah!", 123, 321f, 456));
///   }
/// }
/// ]]></code>
/// <para>Note, that it's OK to name such members as constants in spite of they are not constants by
/// the C# language semantics. I.e. instead of <c>myMessage</c> you spell <c>MyMessage</c> to
/// highlight the fact it won't (and must not) change from the code.</para>  
/// </example>
public struct Message<T1, T2, T3, T4> {
  readonly string fmtString;
  
  /// <summary>Creates a message.</summary>
  /// <param name="fmtString">A message format string.</param>
  public Message(string fmtString) {
    this.fmtString = fmtString;
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <param name="arg2">An argument to substitute.</param>
  /// <param name="arg3">An argument to substitute.</param>
  /// <param name="arg4">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
    return string.Format(fmtString, arg1, arg2, arg3, arg4);
  }

  /// <summary>Allows casting strings to message.</summary>
  /// <param name="fmtString">A string value to assign.</param>
  /// <returns>Message instance.</returns>
  public static implicit operator Message<T1, T2, T3, T4>(string fmtString) {
    return new Message<T1, T2, T3, T4>(fmtString);
  }
}

/// <summary>A class to wrap a UI string with five parameters.</summary>
/// <typeparam name="T1">Type of the first substitute argument in the string.</typeparam>
/// <typeparam name="T2">Type of the second substitute argument in the string.</typeparam>
/// <typeparam name="T3">Type of the third substitute argument in the string.</typeparam>
/// <typeparam name="T4">Type of the fourth substitute argument in the string.</typeparam>
/// <typeparam name="T5">Type of the fifth substitute argument in the string.</typeparam>
/// <remarks>
/// <para>Define parameter types via generic argument. When string needs to be presented use
/// <see cref="Format"/> to make parameters substitute.</para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>Message</c>.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   Message<string, int, float, int, float> MyMessage = "Params: {0}, {1}, {2}, {3}, {4}";
///
///   void Awake() {
///     Debug.Log("Localized: {0}", MyMessage.Format("Blah!", 123, 321f, 456, 456f));
///   }
/// }
/// ]]></code>
/// <para>Note, that it's OK to name such members as constants in spite of they are not constants by
/// the C# language semantics. I.e. instead of <c>myMessage</c> you spell <c>MyMessage</c> to
/// highlight the fact it won't (and must not) change from the code.</para>  
/// </example>
public struct Message<T1, T2, T3, T4, T5> {
  readonly string fmtString;
  
  /// <summary>Creates a message.</summary>
  /// <param name="fmtString">A message format string.</param>
  public Message(string fmtString) {
    this.fmtString = fmtString;
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <param name="arg2">An argument to substitute.</param>
  /// <param name="arg3">An argument to substitute.</param>
  /// <param name="arg4">An argument to substitute.</param>
  /// <param name="arg5">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
    return string.Format(fmtString, arg1, arg2, arg3, arg4, arg5);
  }

  /// <summary>Allows casting strings to message.</summary>
  /// <param name="fmtString">A string value to assign.</param>
  /// <returns>Message instance.</returns>
  public static implicit operator Message<T1, T2, T3, T4, T5>(string fmtString) {
    return new Message<T1, T2, T3, T4, T5>(fmtString);
  }
}

}  // namespace
