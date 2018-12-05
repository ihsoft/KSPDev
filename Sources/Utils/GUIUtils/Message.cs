// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSP.Localization;

namespace KSPDev.GUIUtils {

/// <summary>A class to wrap a simple localizable UI string.</summary>
/// <remarks>
/// Messages of this type don't have placeholders and can be just casted to a string. 
/// </remarks>
/// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="MessageDemo"/></example>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
public sealed class Message : LocalizableMessage {
  /// <inheritdoc cref="LocalizableMessage(string,string,string,string)"/>
  /// <seealso cref="LocalizableMessage"/>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="MessageDemo"/></example>
  public Message(string tag,
                 string defaultTemplate = null,
                 string description = null, string example = null)
      : base(tag, defaultTemplate, description, example) {
  }

  /// <summary>Allows casting a localization tag to the message.</summary>
  /// <remarks>
  /// It can be used to create a message by simply assigning a localization tag. However, it's
  /// highly recommended to create a message via the constructor and provide all the arguments.
  /// </remarks>
  /// <param name="tag">The tag string to use for getting the localized content.</param>
  /// <returns>A message instance.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="MessageDemo"/></example>
  public static implicit operator Message(string tag) {
    return new Message(tag);
  }

  /// <summary>Allows casting messages to string.</summary>
  /// <param name="msg">A message to cast.</param>
  /// <returns>Message value.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="MessageDemo"/></example>
  public static implicit operator string(Message msg) {
    return msg.localizedTemplate;
  }

  /// <summary>Returns the message string.</summary>
  /// <returns>A complete message string.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="MessageDemo"/></example>
  public string Format() {
    return localizedTemplate;
  }
}

/// <summary>A class to wrap a localizable UI string with parameter(s).</summary>
/// <typeparam name="T1">Type of argument <![CDATA[<<1>>]]> in the Lingoona template.</typeparam>
/// <remarks>
/// <para>
/// Define the parameter(s) type via the generic argument(s). When the string needs to be
/// presented, use the <see cref="Format"/> method to get the final value.
/// </para>
/// <para>
/// The arguments can be complex types that override the <c>ToString()</c> method. This approach
/// can be used to customize the output format of the specific argument types. E.g. such values as
/// "distance" can be formatted in a user friendly manner using
/// <see cref="TypeFormatters.DistanceType"/>.
/// </para>
/// </remarks>
/// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message1Demo"/></example>
/// <seealso cref="Message"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/Lingoona/*"/>
public sealed class Message<T1> : LocalizableMessage {
  /// <inheritdoc cref="LocalizableMessage(string,string,string,string)"/>
  /// <seealso cref="LocalizableMessage"/>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message1Demo"/></example>
  public Message(string tag,
                 string defaultTemplate = null,
                 string description = null,
                 string example = null)
      : base(tag, defaultTemplate, description, example) {
  }

  /// <summary>Formats the message template string with the provided arguments.</summary>
  /// <remarks>
  /// The string parameter(s) can be template tags. If the type of the argument is <c>string</c>,
  /// and the value matches a tag, then the localized string of this tag is used instead. Keep it
  /// in mind when defining the tags to avoid the collisions.
  /// </remarks>
  /// <param name="arg1">The substitute for the <![CDATA[<<1>>]]> argument.</param>
  /// <returns>A complete and localized message string.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message1Demo"/></example>
  public string Format(T1 arg1) {
    return Localizer.Format(localizedTemplate, arg1);
  }

  /// <summary>Allows casting a string to a message.</summary>
  /// <remarks>
  /// It can be used to create a message by simply assigning a localization tag. However, it's
  /// highly recommended to create a message via the constructor and provide all the arguments.
  /// </remarks>
  /// <param name="tag">The tag string to use for getting the localized content.</param>
  /// <returns>A message instance.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message1Demo"/></example>
  public static implicit operator Message<T1>(string tag) {
    return new Message<T1>(tag);
  }
}

/// <summary>A class to wrap a localizable UI string with parameter(s).</summary>
/// <typeparam name="T1">Type of argument <![CDATA[<<1>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T2">Type of argument <![CDATA[<<2>>]]> in the Lingoona template.</typeparam>
/// <remarks>
/// <para>
/// Define the parameter(s) type via the generic argument(s). When the string needs to be
/// presented, use the <see cref="Format"/> method to get the final value.
/// </para>
/// <para>
/// The arguments can be complex types that override the <c>ToString()</c> method. This approach
/// can be used to customize the output format of the specific argument types. E.g. such values as
/// "distance" can be formatted in a user friendly manner using
/// <see cref="TypeFormatters.DistanceType"/>.
/// </para>
/// </remarks>
/// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message2Demo"/></example>
/// <seealso cref="Message"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/Lingoona/*"/>
public class Message<T1, T2> : LocalizableMessage {
  /// <inheritdoc cref="LocalizableMessage(string,string,string,string)"/>
  /// <seealso cref="LocalizableMessage"/>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message2Demo"/></example>
  public Message(string tag,
                 string defaultTemplate = null,
                 string description = null,
                 string example = null)
      : base(tag, defaultTemplate, description, example) {
  }

  /// <summary>Formats the message template string with the provided arguments.</summary>
  /// <remarks>
  /// The string parameter(s) can be template tags. If the type of the argument is <c>string</c>,
  /// and the value matches a tag, then the localized string of this tag is used instead. Keep it
  /// in mind when defining the tags to avoid the collisions.
  /// </remarks>
  /// <param name="arg1">The substitute for the <![CDATA[<<1>>]]> argument.</param>
  /// <param name="arg2">The substitute for the <![CDATA[<<2>>]]> argument.</param>
  /// <returns>A complete and localized message string.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message2Demo"/></example>
  public string Format(T1 arg1, T2 arg2) {
    return Localizer.Format(localizedTemplate, arg1, arg2);
  }

  /// <summary>Allows casting a string to a message.</summary>
  /// <remarks>
  /// It can be used to create a message by simply assigning a localization tag. However, it's
  /// highly recommended to create a message via the constructor and provide all the arguments.
  /// </remarks>
  /// <param name="tag">The tag string to use for getting the localized content.</param>
  /// <returns>A message instance.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message2Demo"/></example>
  public static implicit operator Message<T1, T2>(string tag) {
    return new Message<T1, T2>(tag);
  }
}

/// <summary>A class to wrap a localizable UI string with parameter(s).</summary>
/// <typeparam name="T1">Type of argument <![CDATA[<<1>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T2">Type of argument <![CDATA[<<2>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T3">Type of argument <![CDATA[<<3>>]]> in the Lingoona template.</typeparam>
/// <remarks>
/// <para>
/// Define the parameter(s) type via the generic argument(s). When the string needs to be
/// presented, use the <see cref="Format"/> method to get the final value.
/// </para>
/// <para>
/// The arguments can be complex types that override the <c>ToString()</c> method. This approach
/// can be used to customize the output format of the specific argument types. E.g. such values as
/// "distance" can be formatted in a user friendly manner using
/// <see cref="TypeFormatters.DistanceType"/>.
/// </para>
/// </remarks>
/// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message3Demo"/></example>
/// <seealso cref="Message"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/Lingoona/*"/>
public class Message<T1, T2, T3> : LocalizableMessage {
  /// <inheritdoc cref="LocalizableMessage(string,string,string,string)"/>
  /// <seealso cref="LocalizableMessage"/>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message3Demo"/></example>
  public Message(string tag,
                 string defaultTemplate = null,
                 string description = null,
                 string example = null)
      : base(tag, defaultTemplate, description, example) {
  }

  /// <summary>Formats the message template string with the provided arguments.</summary>
  /// <remarks>
  /// The string parameter(s) can be template tags. If the type of the argument is <c>string</c>,
  /// and the value matches a tag, then the localized string of this tag is used instead. Keep it
  /// in mind when defining the tags to avoid the collisions.
  /// </remarks>
  /// <param name="arg1">The substitute for the <![CDATA[<<1>>]]> argument.</param>
  /// <param name="arg2">The substitute for the <![CDATA[<<2>>]]> argument.</param>
  /// <param name="arg3">The substitute for the <![CDATA[<<3>>]]> argument.</param>
  /// <returns>A complete and localized message string.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message3Demo"/></example>
  public string Format(T1 arg1, T2 arg2, T3 arg3) {
    return Localizer.Format(localizedTemplate, arg1, arg2, arg3);
  }

  /// <summary>Allows casting a string to a message.</summary>
  /// <remarks>
  /// It can be used to create a message by simply assigning a localization tag. However, it's
  /// highly recommended to create a message via the constructor and provide all the arguments.
  /// </remarks>
  /// <param name="tag">The tag string to use for getting the localized content.</param>
  /// <returns>A message instance.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message3Demo"/></example>
  public static implicit operator Message<T1, T2, T3>(string tag) {
    return new Message<T1, T2, T3>(tag);
  }
}

/// <summary>A class to wrap a localizable UI string with parameter(s).</summary>
/// <typeparam name="T1">Type of argument <![CDATA[<<1>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T2">Type of argument <![CDATA[<<2>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T3">Type of argument <![CDATA[<<3>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T4">Type of argument <![CDATA[<<4>>]]> in the Lingoona template.</typeparam>
/// <remarks>
/// <para>
/// Define the parameter(s) type via the generic argument(s). When the string needs to be
/// presented, use the <see cref="Format"/> method to get the final value.
/// </para>
/// <para>
/// The arguments can be complex types that override the <c>ToString()</c> method. This approach
/// can be used to customize the output format of the specific argument types. E.g. such values as
/// "distance" can be formatted in a user friendly manner using
/// <see cref="TypeFormatters.DistanceType"/>.
/// </para>
/// </remarks>
/// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message4Demo"/></example>
/// <seealso cref="Message"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/Lingoona/*"/>
public class Message<T1, T2, T3, T4> : LocalizableMessage {
  /// <inheritdoc cref="LocalizableMessage(string,string,string,string)"/>
  /// <seealso cref="LocalizableMessage"/>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message4Demo"/></example>
  public Message(string tag,
                 string defaultTemplate = null,
                 string description = null,
                 string example = null)
      : base(tag, defaultTemplate, description, example) {
  }

  /// <summary>Formats the message template string with the provided arguments.</summary>
  /// <remarks>
  /// The string parameter(s) can be template tags. If the type of the argument is <c>string</c>,
  /// and the value matches a tag, then the localized string of this tag is used instead. Keep it
  /// in mind when defining the tags to avoid the collisions.
  /// </remarks>
  /// <param name="arg1">The substitute for the <![CDATA[<<1>>]]> argument.</param>
  /// <param name="arg2">The substitute for the <![CDATA[<<2>>]]> argument.</param>
  /// <param name="arg3">The substitute for the <![CDATA[<<3>>]]> argument.</param>
  /// <param name="arg4">The substitute for the <![CDATA[<<4>>]]> argument.</param>
  /// <returns>A complete and localized message string.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message4Demo"/></example>
  public string Format(T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
    return Localizer.Format(localizedTemplate, arg1, arg2, arg3, arg4);
  }

  /// <summary>Allows casting a string to a message.</summary>
  /// <remarks>
  /// It can be used to create a message by simply assigning a localization tag. However, it's
  /// highly recommended to create a message via the constructor and provide all the arguments.
  /// </remarks>
  /// <param name="tag">The tag string to use for getting the localized content.</param>
  /// <returns>A message instance.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message4Demo"/></example>
  public static implicit operator Message<T1, T2, T3, T4>(string tag) {
    return new Message<T1, T2, T3, T4>(tag);
  }
}

/// <summary>A class to wrap a localizable UI string with parameter(s).</summary>
/// <typeparam name="T1">Type of argument <![CDATA[<<1>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T2">Type of argument <![CDATA[<<2>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T3">Type of argument <![CDATA[<<3>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T4">Type of argument <![CDATA[<<4>>]]> in the Lingoona template.</typeparam>
/// <typeparam name="T5">Type of argument <![CDATA[<<5>>]]> in the Lingoona template.</typeparam>
/// <remarks>
/// <para>
/// Define the parameter(s) type via the generic argument(s). When the string needs to be
/// presented, use the <see cref="Format"/> method to get the final value.
/// </para>
/// <para>
/// The arguments can be complex types that override the <c>ToString()</c> method. This approach
/// can be used to customize the output format of the specific argument types. E.g. such values as
/// "distance" can be formatted in a user friendly manner using
/// <see cref="TypeFormatters.DistanceType"/>.
/// </para>
/// </remarks>
/// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message5Demo"/></example>
/// <seealso cref="Message"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/Lingoona/*"/>
public class Message<T1, T2, T3, T4, T5> : LocalizableMessage {
  /// <inheritdoc cref="LocalizableMessage(string,string,string,string)"/>
  /// <seealso cref="LocalizableMessage"/>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message5Demo"/></example>
  public Message(string tag,
                 string defaultTemplate = null,
                 string description = null,
                 string example = null)
      : base(tag, defaultTemplate, description, example) {
  }

  /// <summary>Formats the message template string with the provided arguments.</summary>
  /// <remarks>
  /// The string parameter(s) can be template tags. If the type of the argument is <c>string</c>,
  /// and the value matches a tag, then the localized string of this tag is used instead. Keep it
  /// in mind when defining the tags to avoid the collisions.
  /// </remarks>
  /// <param name="arg1">The substitute for the <![CDATA[<<1>>]]> argument.</param>
  /// <param name="arg2">The substitute for the <![CDATA[<<2>>]]> argument.</param>
  /// <param name="arg3">The substitute for the <![CDATA[<<3>>]]> argument.</param>
  /// <param name="arg4">The substitute for the <![CDATA[<<4>>]]> argument.</param>
  /// <param name="arg5">The substitute for the <![CDATA[<<5>>]]> argument.</param>
  /// <returns>A complete and localized message string.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message5Demo"/></example>
  public string Format(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
    return Localizer.Format(localizedTemplate, arg1, arg2, arg3, arg4, arg5);
  }

  /// <summary>Allows casting a string to a message.</summary>
  /// <remarks>
  /// It can be used to create a message by simply assigning a localization tag. However, it's
  /// highly recommended to create a message via the constructor and provide all the arguments.
  /// </remarks>
  /// <param name="tag">The tag string to use for getting the localized content.</param>
  /// <returns>A message instance.</returns>
  /// <example><code source="Examples/GUIUtils/Message-Examples.cs" region="Message5Demo"/></example>
  public static implicit operator Message<T1, T2, T3, T4, T5>(string tag) {
    return new Message<T1, T2, T3, T4, T5>(tag);
  }
}

}  // namespace
