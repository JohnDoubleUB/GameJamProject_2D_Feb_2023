using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PointAndClickManager : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    private int pAndCProgression = 0;

    [SerializeField]
    private InteractiveElement[] interactiveElements;

    private int interactedObjects = 0;

    [SerializeField]
    private Image previewImage;

    private void Start()
    {
        InitiateProgress(pAndCProgression);
    }

    private void Update()
    {
        if (pAndCProgression == 2) 
        {
            if (interactedObjects == interactiveElements.Length) 
            {
                pAndCProgression++;
                InitiateProgress(pAndCProgression);
            }
        }
    }

    private void InitiateProgress(int number)
    {
        switch (number)
        {
            case 0:
                videoPlayer.Play();
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("CutsceneAudio", gameObject, AK_CallbackFunction);
                //AkSoundEngine.StopAll(gameObject);
                break;

            case 1:
                AudioManager.current.AK_PlayClipOnObject("EarthAmbience", gameObject);
                AudioManager.current.AK_PlayClipOnObject("EarthSong", gameObject);
                videoPlayer.enabled = false;
                UIManager.current.SetActiveContexts(true, UIContext.PointAndClick);
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA15", gameObject, AK_CallbackFunction);
                break;
            case 2:
                //Enable interactive objects
                break;
            case 3:
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VAAllClicked", gameObject, AK_CallbackFunction);
                break;
            case 4:
                //End?
                UIManager.current.SetActiveContexts(true, UIContext.EndScreen);
                break;
        }
    }

    private void AK_CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        switch (in_type)
        {
            case AkCallbackType.AK_EndOfEvent:
                pAndCProgression++;
                InitiateProgress(pAndCProgression);
                break;

            default:
                break;
        }
    }


    public void ItemClicked(int itemIndex)
    {
        if (pAndCProgression > 1)
        {
            InteractiveElement element = interactiveElements[itemIndex];
            element.button.interactable = false;
            UIManager.current.SetActiveContexts(true, UIContext.ClickPreview);
            previewImage.sprite = element.sprite;
            previewImage.preserveAspect = true;
            AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback(element.VALine, gameObject, AK_OnReturnAudioEnd);
        }


        void AK_OnReturnAudioEnd(object in_cookie, AkCallbackType in_type, object in_info) 
        {
            switch (in_type)
            {
                case AkCallbackType.AK_EndOfEvent:
                    UIManager.current.SetActiveContexts(false, UIContext.ClickPreview);
                    interactedObjects++;
                    break;

                default:
                    break;
            }

            
        }
    }

    private void OnDestroy()
    {
        AudioManager.current.AK_PlayClipOnObject("EarthAmbienceStop", gameObject);
        AudioManager.current.AK_PlayClipOnObject("EarthSongStop", gameObject);
        AkSoundEngine.StopAll();

    }
}


[System.Serializable]
public class InteractiveElement 
{
    public Button button;
    public Sprite sprite;
    public string VALine;
}