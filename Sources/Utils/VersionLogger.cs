// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Reflection;
using UnityEngine;
using KSPDev.FSUtils;

/// <summary>
/// Debugging class that reports which DLL was actually loaded for the Utils assembly.
/// </summary>
/// <remarks>
/// It helps understanding which binary is used in case of multiple mods are using same version of
/// KSPDev_utils assembly.
/// </remarks>
[KSPAddon(KSPAddon.Startup.Instantly, true /*once*/)]
class VersionLogger : MonoBehaviour {
  void Awake() {
    var assembly = Assembly.GetExecutingAssembly();
    var relPath = KspPaths.MakeRelativePathToGameData(assembly.Location);
    Debug.LogFormat("Assembly {0} is loaded from {1}", assembly.FullName, relPath);
  }
}
