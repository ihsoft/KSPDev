// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;

namespace KSPDev.GUIUtils {

/// <summary>Holds a mapping of a value to a localized message.</summary>
/// <typeparam name="T">Type of the key. It must be non-nullable.</typeparam>
/// <remarks>
/// <para>
/// Use it when a definite set of values of the same kind needs to be mapped to the localized
/// strings. A good example of such mapping is a localization of the enum type values. However, for
/// this class the key type doesn't need to be enum. It can be any number, or even a struct. It
/// cannot be a string, though.
/// </para>
/// </remarks>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_Simple"/></example>
/// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_WithDefault"/></example>
/// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_WithStockDefault"/></example>
public sealed class MessageLookup<T> where T : struct {
  /// <summary>Mapping of the key to the messages.</summary>
  public readonly Dictionary<T, Message> messages;

  /// <summary>Message to return when the requested key is not found.</summary>
  public readonly Message defaultMessage;

  /// <summary>Constructs a lookup from the provided dictionary.</summary>
  /// <remarks>
  /// It's encouraged to construct the dictionary from a statically defined messages instead of
  /// creating them in place. The <c>LocalizationTool</c> can only find the static messages.
  /// </remarks>
  /// <param name="initDict">
  /// The dictionary to use as the lookup. If <c>null</c> when a new empty dictionary will be
  /// created for <see cref="messages"/>.
  /// </param>
  /// <param name="defaultMessage">
  /// The message to return from the <see cref="Lookup"/> method when no key found.
  /// </param>
  /// <seealso cref="Lookup"/>
  /// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_Simple"/></example>
  public MessageLookup(Dictionary<T, Message> initDict = null, Message defaultMessage = null) {
    messages = initDict ?? new Dictionary<T, Message>();
    this.defaultMessage = defaultMessage;
  }

  /// <summary>Finds and returns a message for the provided key.</summary>
  /// <param name="key">The key to find a message for.</param>
  /// <returns>
  /// The relevant message if the <paramref name="key"/> is found. Otherwise, either the
  /// <see cref="defaultMessage"/> or a string represenation of the key. It's never <c>null</c>. 
  /// </returns>
  /// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_WithDefault"/></example>
  /// <example><code source="Examples/GUIUtils/MessageLookup-Examples.cs" region="MessageLookupDemo_WithStockDefault"/></example>
  /// <seealso cref="defaultMessage"/>
  public Message Lookup(T key) {
    Message res;
    if (!messages.TryGetValue(key, out res)) {
      return defaultMessage ?? new Message(null, defaultTemplate: key.ToString());
    }
    return res;
  }
}

}  // namespace
