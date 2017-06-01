// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;

namespace KSPDev.Extensions {

/// <summary>Helper extensions for the generic dictionary container.</summary>
/// <example><code source="Examples/Extensions/Dictionary-Examples.cs" region="SetDefaultAddToDict"/></example>
//TODO: Add an example why it's so good.
public static class Dictionaries {
  /// <summary>
  /// Returns a value from the dictionary by a key. If the key is not present yet, then a new
  /// default entry is created and returned.
  /// </summary>
  /// <param name="dict">The dictionary to get value from.</param>
  /// <param name="key">The key to lookup.</param>
  /// <typeparam name="K">The type of the dictionary key.</typeparam>
  /// <typeparam name="V">The type of the dictionary value.</typeparam>
  /// <returns>Either an existing value for the key or a default instance of the value.</returns>
  /// <example>
  /// If there is a dictionary which values are collections or a class, then a special code is
  /// always needed to properly access this dictionary:
  /// <code source="Examples/Extensions/Dictionary-Examples.cs" region="ClassicAddToDict"/>
  /// With this extension the key can safely be accessed with just one call:   
  /// <code source="Examples/Extensions/Dictionary-Examples.cs" region="SetDefaultAddToDict"/>
  /// </example>
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
