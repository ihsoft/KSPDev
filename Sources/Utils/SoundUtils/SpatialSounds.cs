// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.LogUtils;
using UnityEngine;

namespace KSPDev.SoundsUtils {

/// <summary>Helper class to deal with the sounds attached to a game object.</summary>
public static class SpatialSounds {
  /// <summary>Sets up a sound FX group with an audio clip .</summary>
  /// <param name="obj">The game object to attach sound to.</param>
  /// <param name="sndPath">The URL to the audio clip.</param>
  /// <param name="loop">Specifies if the clip playback shold be looped.</param>
  /// <param name="maxDistance">The maximum distance at which the sound is hearable.</param>
  /// <returns>An audio source object attached to the <paramref name="obj"/>.</returns>
  public static AudioSource Create3dSound(GameObject obj, string sndPath,
                                          bool loop = false, float maxDistance = 30f) {
    if (HighLogic.LoadedScene == GameScenes.LOADING
        || HighLogic.LoadedScene == GameScenes.LOADINGBUFFER) {
      // Resources are not avaialble during game load. 
      return null;
    }
    if (!GameDatabase.Instance.ExistsAudioClip(sndPath)) {
      HostedDebugLog.Error(obj.transform, "Sound file not found: {0}", sndPath);
    }
    var audio = obj.AddComponent<AudioSource>();
    audio.volume = GameSettings.SHIP_VOLUME;
    audio.rolloffMode = AudioRolloffMode.Linear;
    audio.dopplerLevel = 0f;
    audio.spatialBlend = 1f;  // Set as 2D audiosource
    audio.maxDistance = maxDistance;
    audio.loop = loop;
    audio.playOnAwake = false;
    audio.clip = GameDatabase.Instance.GetAudioClip(sndPath);
    return audio;
  }
}

}  // namespace
