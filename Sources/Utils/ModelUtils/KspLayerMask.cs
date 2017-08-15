// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ModelUtils {

/// <summary>A convinience type to deal with the <see cref="KspLayer">layers</see>.</summary>
/// <seealso cref="KspLayer"/>
[Flags]
public enum KspLayerMask {
  /// <summary>Just a default value that doesn't match any layer.</summary>
  None = 0,
  
  /// <summary>Mask for the <see cref="KspLayer.Part"/> layer.</summary>
  Part = 1 << (int) KspLayer.Part,
  
  /// <summary>Mask for the <see cref="KspLayer.Service"/> layer.</summary>
  Service = 1 << (int) KspLayer.Service,

  /// <summary>Mask for the <see cref="KspLayer.SurfaceCollider"/> layer.</summary>
  SurfaceCollider = 1 << (int) KspLayer.SurfaceCollider,

  /// <summary>Mask for the <see cref="KspLayer.Kerbal"/> layer.</summary>
  Kerbal = 1 << (int) KspLayer.Kerbal,

  /// <summary>Mask for the <see cref="KspLayer.TriggerCollider"/> layer.</summary>
  TriggerCollider = 1 << (int) KspLayer.TriggerCollider, 

  /// <summary>Mask for the <see cref="KspLayer.Fx"/> layer.</summary>
  Fx = 1 << (int) KspLayer.Fx,
};

}  // namespace
