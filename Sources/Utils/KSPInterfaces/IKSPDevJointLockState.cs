// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.KSPInterfaces {

/// <summary>A documented version of the KSP <c>IJointLockState</c> interface.</summary>
/// <remarks>
/// Inherit from <see cref="IJointLockState"/> to let the game know if this part's joint can be
/// unlocked. This interface is a full equivalent of <see cref="IJointLockState"></see> except it's
/// documented. The modules that inherit both interfaces get better code documentation.
/// </remarks>
/// <example>
/// <code><![CDATA[
/// public class MyModule : PartModule, IJointLockState, IKSPDevJointLockState {
///   /// <inheritdoc/>
///   public override bool IsJointUnlocked() {
///     return true;
///   }
/// }
/// ]]></code>
/// </example>
public interface IKSPDevJointLockState {
  /// <summary>Tells if the parts can move relative to each other.</summary>
  /// <remarks>
  /// <para>
  /// It's important to override this method when the joint is not rigid. For the rigid joints the
  /// game may create autostruts when appropriate, which will adhere the parts to each other.
  /// </para>
  /// <para>This method is called on the child part to check it's joint state to the parent.</para>
  /// </remarks>
  /// <returns><c>true</c> if the joint are not fixed relative to each other.</returns>
  bool IsJointUnlocked();
}

}  // namespace
