// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.Types {

/// <summary>Persistent type to hold bare ConfigNode.</summary>
/// <remarks>
/// This type can be used if the module needs to do a run-time parsing of the values.
/// </remarks>
/// <seealso cref="ConfigUtils.PersistentFieldAttribute"/>
public class PersistentConfigNode : ConfigNode, IConfigNode {
  #region IConfigNode implementation
  /// <summary>Copies valus from a node.</summary>
  /// <param name="node">The node to copy from.</param>
  public void Load(ConfigNode node) {
    node.CopyTo(this);
  }

  /// <summary>Copies values into the node.</summary>
  /// <param name="node">The node to copy the values into.</param>
  public void Save(ConfigNode node) {
    CopyTo(node);
  }
  #endregion
}

}  // namespace
