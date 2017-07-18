// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KSPDev.GUIUtils {

/// <summary>Helper class to play sounds in the game GUI. Such sounds are not 3D aligned.</summary>
/// <remarks>
/// <para>
/// Use this player when the source of the sound is a GUI object (e.g. a GUI control). This class
/// implements all the boilerplate to load and play the sound resources. All the sounds are cached
/// wihtin the scene, so repeating requests to the same sound won't add extra latency.
/// </para>
/// <para>
/// In case of the very first usage of the sound is latency restricted, the sound resource can be
/// pre-cached via the <see cref="CacheSound"/> method. It will increase the loading time, though.
/// </para>
/// </remarks>
/// <seealso cref="LibraryLoader"/>
/// <example><code source="Examples/GUIUtils/UISoundPlayer-Examples.cs" region="UISoundPlayerDemo1"/></example>
public sealed class UISoundPlayer : MonoBehaviour {
  /// <summary>Returns the instance of the player in the current scene.</summary>
  /// <value>Instance of the player.</value>
  public static UISoundPlayer instance { get; private set; }

  /// <summary>Global scene cache for all the sounds.</summary>
  readonly Dictionary<string, AudioSource> audioCache = new Dictionary<string, AudioSource>();

  /// <summary>Plays the specified sound.</summary>
  /// <remarks>
  /// Every request is cached, so that the subsequent calls to play the same sound won't require the
  /// audio clip to load. However, the same cached sound cannot be played simultaneously from the
  /// different calls - each call will abort the previous play action of the sound.
  /// </remarks>
  /// <param name="audioPath">
  /// The file path relative to <c>GameData</c>. It can be empty, in which case nothing is played.
  /// </param>
  /// <example><code source="Examples/GUIUtils/UISoundPlayer-Examples.cs" region="UISoundPlayerDemo1"/></example>
  public void Play(string audioPath) {
    if (audioPath == "") {
      return;
    }
    var audio = GetOrLoadAudio(audioPath);
    if (audio != null) {
      audio.Play();
    }
  }

  /// <summary>Loads the sound into cache but doesn't play it.</summary>
  /// <remarks>
  /// Use this method when the sound is expected to be frequently played in the scene. However, it
  /// it only makes sense to pre-cache a resource if the first usage of the sound is a latency
  /// critical. The latency difference is not hight enough to be significant for the GUI actions.
  /// </remarks>
  /// <param name="audioPath">File path relative to <c>GameData</c>.</param>
  /// <example><code source="Examples/GUIUtils/UISoundPlayer-Examples.cs" region="UISoundPlayerDemo1"/></example>
  public void CacheSound(string audioPath) {
    GetOrLoadAudio(audioPath);
  }

  /// <summary>Initializes the <see cref="instance"/>.</summary>
  /// <remarks>It's expected to be called only once.</remarks>
  /// <seealso cref="LibraryLoader"/>
  void Awake() {
    SceneManager.sceneLoaded += (scene, mode) => {
      audioCache.Values.ToList().ForEach(UnityEngine.Object.Destroy);
      audioCache.Clear();
    };
    instance = this;
  }

  /// <summary>Loads the audio sample and plays it.</summary>
  /// <param name="audioPath">The file path relative to <c>GameData</c>.</param>
  /// <returns>An audio resource if loaded or found in the cache, otherwise <c>null</c>.</returns>
  AudioSource GetOrLoadAudio(string audioPath) {
    if (HighLogic.LoadedScene == GameScenes.LOADING
        || HighLogic.LoadedScene == GameScenes.LOADINGBUFFER) {
      // Resources are not avaialble during game load. 
      return null;
    }
    AudioSource audio;
    if (audioCache.TryGetValue(audioPath, out audio)) {
      return audio;
    }
    if (!GameDatabase.Instance.ExistsAudioClip(audioPath)) {
      Debug.LogErrorFormat("Cannot locate audio clip: {0}", audioPath);
      return null;
    }
    Debug.LogFormat("Loading sound audio clip: {0}", audioPath);
    audio = gameObject.AddComponent<AudioSource>();
    audio.volume = GameSettings.UI_VOLUME;
    audio.spatialBlend = 0;  // Set as 2D audiosource
    audio.clip = GameDatabase.Instance.GetAudioClip(audioPath);
    audioCache[audioPath] = audio;
    return audio;
  }
}

}  // namespace