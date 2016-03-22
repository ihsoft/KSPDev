// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;
using System.Collections.Generic;

namespace KSPDev.ConfigUtils {

/// <summary>Implements (de)seriazlization of <seealso cref="UnityEngine.Color"/> values.</summary>
/// <remarks>See <seealso cref="CustomValueHandlerAttribute"/> for more details.</remarks>
public class UnityColorValueHandler : ICustomValueHandler {
  /// <summary>A mapping for standard colors to their string names.</summary>
  private readonly static Dictionary<Color, string> stdColorToStr =
      new Dictionary<Color, string>() {
          { Color.red, "red" },
          { Color.green, "green" },
          { Color.blue, "blue" },
          { Color.white, "white" },
          { Color.black, "black" },
          { Color.yellow, "yellow" },
          { Color.cyan, "cyan" },
          { Color.magenta, "magenta" },
          { Color.gray, "gray"}
      };

  /// <summary>A mapping for names of standard colors to the respective color values.</summary>
  private readonly static Dictionary<string, Color> strToStdColor =
      new Dictionary<string, Color>() {
          { "red", Color.red},
          { "green", Color.green},
          { "blue", Color.blue},
          { "white", Color.white},
          { "black", Color.black},
          { "yellow", Color.yellow},
          { "cyan", Color.cyan},
          { "magenta", Color.magenta},
          { "gray", Color.gray}
      };
  
  public string ToString(object value) {
    string strValue;
    var color = (Color) value;
    if (!stdColorToStr.TryGetValue(color, out strValue)) {
      strValue = string.Format("{0},{1},{2},{3}", color.r, color.g, color.b, color.a);
    }
    return strValue;
  }

  public object ToValue(string strValue) {
    Color color;
    if (!strToStdColor.TryGetValue(strValue, out color)) {
      //FIXME: Implement RGBA parsing and throw when cannot parse
      //color = Color.white;
      throw new ArgumentException("Value " + strValue + " is not a valid value for Unity.Color");
    }
    return color;
  }
}
  
}  // namespace
