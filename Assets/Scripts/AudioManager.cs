using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager current;

    private void Awake()
    {

        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        current = this;
    }

    public uint AK_PlayEventAt(string eventName, Vector3 pos)
    {
        GameObject tempGO = new GameObject($"TempAudio_{eventName}_"); // create the temp object
        tempGO.transform.position = pos; // set its position
        uint eventID = AkSoundEngine.PostEvent(eventName, tempGO, (uint)AkCallbackType.AK_EndOfEvent, AK_CallbackFunction, tempGO);
        tempGO.name += eventID;
        return eventID;
    }

    //This will follow the object
    public uint AK_PlayClipOnObject(string eventName, GameObject in_gameObjectID)
    {
        return AkSoundEngine.PostEvent(eventName, in_gameObjectID);
    }

    public uint AK_PlayClipOnObjectWithEndEventCallback(string eventName, GameObject in_gameObjectID, AkCallbackManager.EventCallback in_pfnCallback)
    {
        return AkSoundEngine.PostEvent(eventName, in_gameObjectID, (uint)AkCallbackType.AK_EndOfEvent, in_pfnCallback, in_gameObjectID);
    }

    private void AK_CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        switch (in_type)
        {
            case AkCallbackType.AK_EndOfEvent:
                if (in_cookie is GameObject) Destroy((GameObject)in_cookie);
                break;

            default:
                break;
        }
    }
}
