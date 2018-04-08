// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.MathUtils {

/// <summary>
/// Gives some common methods for handling dobule values in the game.
/// </summary>
public static class Mathd {
  /// <summary>Value which can be safely considered to be <c>0</c>.</summary>
  public const double Epsilon = 1E-06;
  
  /// <summary>Tells if the two double values look the same in the geme's units.</summary>
  /// <remarks>
  /// This method gives the same logic as <c>Mathf.Approximately</c>, but for the double values.
  /// </remarks>
  /// <param name="a">The first value to test.</param>
  /// <param name="b">The second value to test.</param>
  /// <returns>
  /// <c>true</c> if the values difference is negligible from the game's perspective.
  /// </returns>
  public static bool Approximately(double a, double b) {
    return Math.Abs(b - a) < Math.Max(
        1E-06 * Math.Max(Math.Abs(a), Math.Abs(b)), Mathd.Epsilon * 8.0);
  }

  /// <summary>Tells if the two double are the same, allowing some small error.</summary>
  /// <remarks>
  /// This method requires the difference between the values to be negligible. The absolute values
  /// are not counted.
  /// </remarks>
  /// <param name="a">The first value to test.</param>
  /// <param name="b">The second value to test.</param>
  /// <returns>
  /// <c>true</c> if the values difference is negligible.
  /// </returns>
  public static bool AreSame(double a, double b) {
    return Math.Abs(b - a) < Epsilon;
  }
}

}  // namespace
