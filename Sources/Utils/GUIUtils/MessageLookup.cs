﻿// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;

namespace KSPDev.GUIUtils {

/// <summary>Localized message formatting class for a enum value.</summary>
/// <typeparam name="T">Type of the enum to wrap.</typeparam>
/// <remarks>
/// <para>
/// Use it as a generic parameter when creating a <see cref="LocalizableMessage"/> descendants. In
/// spite of the regular enum type, this wrapper returns an integer representation of the value when
/// casted to a string. This is vital for the Lingoona templates since they don't recognize the enum
/// values.
/// </para>
/// <para>
/// The conversion between the enum value and the wrapper can be done implicitly. The wrapper can be
/// used in the places where the enum value would normally be used, and vise versa.
/// </para>
/// </remarks>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_Simple"/></example>
/// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_WithDefault"/></example>
/// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_WithStockDefault"/></example>
//FIXME: examples and docs
public sealed class MessageLookup<T> where T : struct {
  /// <summary>Mapping of the key to the messages.</summary>
  public readonly Dictionary<T, Message> messages;

  /// <summary>Message to return when the requested key is not found.</summary>
  /// <seealso cref="stockDefaultMessage"/>
  public readonly Message defaultMessage;

  /// <summary>Default message to return when the requested key is not found.</summary>
  /// <seealso cref="defaultMessage"/>
  public readonly Message stockDefaultMessage = new Message(null, defaultTemplate: "#NONE#");

  /// <summary>Constructs a lookup from the provided dictionary.</summary>
  /// <remarks>
  /// It's encouraged to construct the dictionary from a statically defined messages instead of
  /// creating them in place. The <c>LocalizationTool</c> cann only find the static messages.
  /// </remarks>
  /// <param name="initDict">
  /// The dictionary to use as the lookup. If <c>null</c> when a new empty dictionary will be
  /// created for <see cref="messages"/>.
  /// </param>
  /// <param name="defaultMessage">
  /// The message to return from the <see cref="Lookup"/> method when no key found. If omitted then
  /// the non-localizable  <see cref="stockDefaultMessage"/> will be returned.
  /// </param>
  /// <seealso cref="Lookup"/>
  /// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_Simple"/></example>
  public MessageLookup(Dictionary<T, Message> initDict = null, Message defaultMessage = null) {
    messages = initDict ?? new Dictionary<T, Message>();
    this.defaultMessage = defaultMessage ?? stockDefaultMessage;
  }

  /// <summary>Find and return a message for the provided key.</summary>
  /// <param name="key">The key to find a message for.</param>
  /// <param name="noDefault">
  /// If <c>true</c> and no key was found, then <c>null</c> is returned instead of the
  /// <see cref="defaultMessage"/>.
  /// </param>
  /// <returns>
  /// The relevant message if the <paramref name="key"/> is found. Otherwise, the
  /// <see cref="defaultMessage"/> or <c>null</c>.
  /// </returns>
  /// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_WithDefault"/></example>
  /// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_WithStockDefault"/></example>
  /// <seealso cref="defaultMessage"/>
  public Message Lookup(T key, bool noDefault = false) {
    Message res;
    if (!messages.TryGetValue(key, out res)) {
      return noDefault ? null : defaultMessage;
    }
    return res;
  }
}

}  // namespace
