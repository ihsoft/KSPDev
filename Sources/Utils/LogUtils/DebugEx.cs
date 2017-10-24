// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using UnityEngine;

namespace KSPDev.LogUtils {

/// <summary>An extended version of the logging capabilities in the game.</summary>
/// <remarks>
/// One of the benefit of this logging class is that it can better resolve the arguments of the
/// certain types. E.g. when logging out a value referring a <see cref="Transform"/> type, the
/// resulted record will represent a full hierrachy path instead of just the object name. See
/// <see cref="DebugEx.ObjectToString"/> for the full list of the supported types.
/// </remarks>
/// <seealso cref="HostedDebugLog"/>
public static class DebugEx {
  /// <summary>
  /// Logs a formatted INFO message giving a better context on the objects in the parameters.
  /// </summary>
  /// <remarks>
  /// The arguments are not just transformed into the strings by using their <c>ToString</c> method.
  /// Instead, this method tries to make a best guess of what the object is, and gives more context
  /// when possible. Read the full list of the supported objects in the
  /// <see cref="ObjectToString"/> method docs.
  /// </remarks>
  /// <param name="format">The format string for the log message.</param>
  /// <param name="args">The arguments for the format string.</param>
  /// <seealso cref="ObjectToString"/>
  /// <seealso cref="Log"/>
  public static void Info(string format, params object[] args) {
    Log(LogType.Log, format, args);
  }

  /// <summary>
  /// Logs a formatted INFO message with a host identifier when the <i>verbose</i> logging mode is
  /// enabled.
  /// </summary>
  /// <inheritdoc cref="Info"/>
  public static void Fine(string format, params object[] args) {
    if (GameSettings.VERBOSE_DEBUG_LOG) {
      Log(LogType.Log, format, args);
    }
  }

  /// <summary>Logs a formatted WARNING message with a host identifier.</summary>
  /// <inheritdoc cref="Info"/>
  public static void Warning(string format, params object[] args) {
    Log(LogType.Warning, format, args);
  }

  /// <summary>Logs a formatted ERROR message with a host identifier.</summary>
  /// <inheritdoc cref="Info"/>
  public static void Error(string format, params object[] args) {
    Log(LogType.Error, format, args);
  }

  /// <summary>Generic method to emit a log record.</summary>
  /// <remarks>
  /// It also catches the improperly declared formatting strings, and reports the error instead of
  /// throwing.
  /// </remarks>
  /// <param name="type">The type of the log record.</param>
  /// <param name="format">The format string for the log message.</param>
  /// <param name="args">The arguments for the format string.</param>
  /// <seealso cref="ObjectToString"/>
  public static void Log(LogType type, string format, params object[] args) {
    try {
      Debug.logger.LogFormat(type, format, args.Select(x => ObjectToString(x)).ToArray());
    } catch (Exception e) {
      Debug.LogErrorFormat(
          "Failed to format logging string: {0}.\n{1}", format, e.StackTrace.ToString());
    }
  }

  /// <summary>Helper method to make a user friendly object name for the logs.</summary>
  /// <remarks>
  /// This method is much more intelligent than a regular <c>ToString()</c>, it can detect some
  /// common types and give a more context on them while keeping the output short. The currently
  /// supported object types are:
  /// <list type="bullet">
  /// <item>The primitive types and strings are returned as is.</item>
  /// <item><see cref="Part"/>. The string will have a part ID.</item>
  /// <item><see cref="PartModule"/>. The string will have a part ID and a module index.</item>
  /// <item>
  /// <see cref="Component"/>. The string will have a full path in the game objects hirerachy.
  /// </item>
  /// </list>
  /// <para>The other types are stringified via a regular <c>ToString()</c> call.</para>
  /// </remarks>
  /// <param name="obj">The object to stringify. It can be <c>null</c>.</param>
  /// <returns>A human friendly string or the original object.</returns>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:Part']"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:PartModule']"/>
  /// <include file="Unity3D_HelpIndex.xml" path="//item[@name='T:UnityEngine.Transform']"/>
  /// <include file="Unity3D_HelpIndex.xml" path="//item[@name='T:UnityEngine.GameObject']"/>
  public static object ObjectToString(object obj) {
    if (obj == null) {
      return "[NULL]";
    }
    if (obj is string || obj.GetType().IsPrimitive) {
      return obj;  // Skip types don't override ToString() and don't have special representaion.
    }
    var partHost = obj as Part;
    if (partHost != null) {
      return "[Part:" + DbgFormatter.PartId(partHost) + "]";
    }
    var moduleHost = obj as PartModule;
    if (moduleHost != null) {
      var moduleNum = moduleHost.part.Modules.IndexOf(moduleHost);
      return "[Part:" + DbgFormatter.PartId(moduleHost.part) + "#Module:" + moduleNum + "]";
    }
    var componentHost = obj as Component;
    if (componentHost != null) {
      return "[" + componentHost.GetType().Name + ":"
          + DbgFormatter.TranformPath(componentHost.transform) + "]";
    }
    return obj.ToString();
  }
}

}  // namespace
