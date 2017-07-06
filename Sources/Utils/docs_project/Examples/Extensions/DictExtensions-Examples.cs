// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System.Collections.Generic;
using KSPDev.Extensions;

namespace Examples {

public static class DictExtensions {
  #region ClassicAddToDict
  public static void ClassicAddToDict(Dictionary<int, HashSet<string>> dict) {
    if (!dict.ContainsKey(123)) {
      // Create an empty string set if the key is not yet initialized.  
      dict[123] = new HashSet<string>();
    }
    dict[123].Add("abc");  // Add the value.
  }
  #endregion

  #region SetDefaultAddToDict
  public static void SetDefaultAddToDict(Dictionary<int, HashSet<string>> dict) {
    // If key 123 doesn't exist it will be created automatically.
    dict.SetDefault(123).Add("abc");
  }
  #endregion
}

}  // namespace
