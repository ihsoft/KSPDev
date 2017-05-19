// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.KSPInterfaces {

/// <summary>Interface for the modules that need handling physics.</summary>
/// <remarks>
/// The events of this interface are triggered by the Unity engine via reflections. It's not
/// required for the module to implement the interface to be notified, but by implementing it, the
/// code becomes more consistent and less error prone.
/// </remarks>
/// <example>
/// <code><![CDATA[
/// public class MyModule : PartModule, IsPhysicalObject {
///   /// <inheritdoc/>
///   public void FixedUpdate() {
///     // Do the physics stuff.
///   }
/// }
/// ]]></code>
/// </example>
/// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">
/// Unity 3D: FixedUpdate</seealso>
public interface IsPhysicalObject {
  /// <summary>Notifies that the fixed rate frame is being handled.</summary>
  /// <remarks>
  /// This method is called by Unity via reflections, so it's not required to implement the
  /// interface to get notified. Though, it's a good idea to implement this interface in the
  /// objects and modules that need the physics updates. It makes the code more readable.
  /// </remarks>
  void FixedUpdate();
}

}  // KSPDev.KSPInterfaces
