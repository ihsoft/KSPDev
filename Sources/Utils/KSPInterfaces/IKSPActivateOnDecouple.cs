// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.KSPInterfaces {

/// <summary>A documented version of the <see cref="IActivateOnDecouple"/> interface.</summary>
/// <remarks>
/// Inherit from <see cref="IActivateOnDecouple"/> to be able reacting on the parts decoupling.
/// This interface is a full equivalent of <see cref="IActivateOnDecouple"/> except it's documented.
/// The modules that inherit both interfaces get better code documentation.
/// </remarks>
/// <example>
/// <code><![CDATA[
/// public class MyModule : PartModule, IActivateOnDecouple, IKSPActivateOnDecouple {
///   /// <inheritdoc/>
///   public virtual void DecoupleAction(string nodeName, bool weDecouple) {
///     Debug.LogInfo("DecoupleAction");
///   }
/// }
/// ]]></code>
/// </example>
/// <seealso href="https://kerbalspaceprogram.com/api/interface_i_activate_on_decouple.html">
/// KSP: IActivateOnDecouple</seealso>
public interface IKSPActivateOnDecouple {
  /// <summary>Called when two parts decouple.</summary>
  /// <remarks>
  /// The callback is only called on the part if it has an attach node that connects it to the other
  /// part. For this event to fire a decoupling logic must be executed. A simple removal from the
  /// vessel hierarchy won't trigger the event.
  /// </remarks>
  /// <param name="nodeName">The attach node name that has been detached.</param>
  /// <param name="weDecouple">
  /// If <c>true</c> then the part being notified was a child in the relation to the detached part.
  /// </param>
  void DecoupleAction(string nodeName, bool weDecouple);
}

}  // namespace
