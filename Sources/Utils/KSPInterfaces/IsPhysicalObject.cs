// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.KSPInterfaces {

/// <summary>Interface for modules that need handling physics.</summary>
/// <remarks>
/// Events of this inteface are triggered by Unity engine via reflections. It's not required for the
/// module to implement the interface to be notified but by implementing it the code becomes more
/// consistent and less error prone.
/// </remarks>
/// <example>
/// <code><![CDATA[
/// public class MyModule : PartModule, IsPhysicalObject {
///   /// <inheritdoc/>
///   public void FixedUpdate() {
///     // Do physics stuff.
///   }
/// }
/// ]]></code>
/// </example>
/// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">
/// Unity 3D: FixedUpdate</seealso>
public interface IsPhysicalObject {
  /// <summary>Notifies that fixed framerate frame is being handled.</summary>
  /// <remarks>
  /// This method is called by Unity via reflections, so it's not required to implement the
  /// interface. Though, it's a good idea to implement this interface in objects/modules that need
  /// physics updates to make code more readable.
  /// </remarks>
  void FixedUpdate();
}

}  // KSPDev.KSPInterfaces
