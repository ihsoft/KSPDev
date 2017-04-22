// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using KSPDev.LogUtils;
using KSPDev.ConfigUtils;
using UnityEngine;

namespace KSPDev.Types {

/// <summary>Type to hold position and rotation of a transform. It can be serialized.</summary>
/// <remarks>
/// The value serializes into a string of either 2 or 3 triples of numbers separated by a comma:
/// <list type="bullet">
/// <item>The first triple is a position: (x, y, z).</item>
/// <item>
/// The second triple is a "forward" direction: (x, y, z). It's not required to be normalized.
/// </item>
/// <item>
/// The optional third triple is an "upwards" direction: (x, y, z). It's not required to be
/// normalized. If omitted then a standard <c>Vector3.up</c> value is used. For the proper
/// orientation the upwards direction must not be the same as the forward one. Otherwise, the
/// rotation around the forward axis will be undetermined (it will be random, basically).
/// </item>
/// </list>
/// </remarks>
/// <seealso cref="PersistentFieldAttribute"/>
/// <seealso cref="ConfigAccessor"/>
public sealed class PosAndRot : IPersistentField {
  /// <summary>Position of the transform.</summary>
  public Vector3 pos;

  /// <summary>Orientation of the transform.</summary>
  public Quaternion rot;

  /// <inheritdoc/>
  public string SerializeToString() {
    var dir = rot * Vector3.forward;
    var upwards = rot * Vector3.up;
    if (Mathf.Approximately(Vector3.up.x, upwards.x)
        && Mathf.Approximately(Vector3.up.y, upwards.y)
        && Mathf.Approximately(Vector3.up.z, upwards.z)) {
      return string.Format("{0},{1},{2}, {3},{4},{5}", pos.x, pos.y, pos.z, dir.x, dir.y, dir.z);
    }
    return string.Format("{0},{1},{2}, {3},{4},{5}, {6},{7},{8}",
                         pos.x, pos.y, pos.z, dir.x, dir.y, dir.z, upwards.x, upwards.y, upwards.z);
  }

  /// <inheritdoc/>
  public void ParseFromString(string value) {
    var elements = value.Split(',');
    if (elements.Length != 6 && elements.Length != 9) {
      throw new ArgumentException(
          "PosAndRot type needs exactly 6 or 9 elements separated by a comma");
    }
    var args = elements.Select(x => float.Parse(x)).ToArray();
    var upwards = elements.Length == 9 ? new Vector3(args[6], args[7], args[8]) : Vector3.up;
    pos = new Vector3(args[0], args[1], args[2]);
    rot = Quaternion.LookRotation(new Vector3(args[3], args[4], args[5]), upwards);
  }

  /// <inheritdoc/>
  public override string ToString() {
    return string.Format("[PosAndRot Pos={0}, Dir={1}, Up={2}]",
                         DbgFormatter.Vector(pos),
                         DbgFormatter.Vector(rot * Vector3.forward),
                         DbgFormatter.Vector(rot * Vector3.up));
  }
}

}  // namespace
