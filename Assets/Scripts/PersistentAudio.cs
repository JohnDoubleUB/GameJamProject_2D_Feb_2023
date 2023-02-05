using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentAudio : MonoBehaviour
{
    public static PersistentAudio current;

    [SerializeField]
    private static bool spaceAudioPlaying;

    public void PlaySpaceAudio() 
    {
        if (spaceAudioPlaying)
            return;

        AudioManager.current.AK_PlayClipOnObject("ShipRumble", gameObject);
        AudioManager.current.AK_PlayClipOnObject("SpaceSong", gameObject);
        //AkSoundEngine.Stop
        spaceAudioPlaying = true;
    }

    public void PlayEarthAudio()
    {
        AudioManager.current.AK_PlayClipOnObject("EarthSong", gameObject);
        AudioManager.current.AK_PlayClipOnObject("EarthAmbience", gameObject);
        //AkSoundEngine.Stop
        //spaceAudioPlaying = true;
    }

    private void Awake()
    {
        if (current != null)
        {
            Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        }


        current = this;

        DontDestroyOnLoad(gameObject);
    }

    public void StopAllPersistentAudio() 
    {
        AudioManager.current.AK_PlayClipOnObject("SpaceSongStop", gameObject);
        AkSoundEngine.StopAll();
        spaceAudioPlaying = false;
    }


}
