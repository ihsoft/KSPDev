// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;

namespace KSPDev.Extensions {

/// <summary>Helper extensions for the generic dictionary container.</summary>
public static class Dictionaries {
  /// <summary>
  /// Returns a value from the dictionary by a key. If the key is not present yet, then a new
  /// default entry is created and returned.
  /// </summary>
  /// <example>
  /// It's most useful when dealing with the dictionaries of a complex type:
  /// <code><![CDATA[
  /// var a = new Dictionary<int, HashSet<string>>();
  /// // An empty "string set" is created for the key 1, and "abc" is added into it.
  /// a.SetDefault(1).Add("abc");
  /// // "def" is added into the existing string set at the key 1. 
  /// a.SetDefault(1).Add("def");
  /// ]]></code>
  /// </example>
  /// <param name="dict">The dictionary to get value from.</param>
  /// <param name="key">The key to lookup.</param>
  /// <typeparam name="K">The type of the dictionary key.</typeparam>
  /// <typeparam name="V">The type of the dictionary value.</typeparam>
  /// <returns>Either an existing value for the key or a default instance of the value.</returns>
  public static V SetDefault<K, V>(this Dictionary<K, V> dict, K key) where V : new() {
    V value;
    if (dict.TryGetValue(key, out value)) {
      return value;
    }
    value = new V();
    dict.Add(key, value);
    return value;
  }
}

}  // namespace
