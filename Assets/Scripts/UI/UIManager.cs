using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager current;

    [EnumFlags]
    public UIContext EnabledContextsFromStartup;


    //public GroupUIContext t;
    public List<SerializedPair<UIContext, UIContextObject>> UIContexts = new List<SerializedPair<UIContext, UIContextObject>>();

    [SerializeField]
    public Dictionary<UIContext, List<UIContextObject>> _UIContexts = new Dictionary<UIContext, List<UIContextObject>>();


    public void SetActiveContexts(bool active, params UIContext[] contexts)
    {
        foreach (UIContext context in contexts)
        {
            if (_UIContexts.ContainsKey(context))
            {
                foreach (UIContextObject contextObject in _UIContexts[context]) contextObject.SetDisplayAndActive(active);
            }
        }
    }

    public void SetActiveContexts(bool active, bool immediate, params UIContext[] contexts)
    {
        foreach (UIContext context in contexts)
        {
            if (_UIContexts.ContainsKey(context))
            {
                foreach (UIContextObject contextObject in _UIContexts[context]) contextObject.SetDisplayAndActive(active, immediate);
            }
        }
    }

    public void SetToggleActiveContexts(params UIContext[] contexts)
    {
        foreach (UIContext context in contexts)
        {
            if (_UIContexts.ContainsKey(context))
            {
                foreach (UIContextObject contextObject in _UIContexts[context]) contextObject.SetDisplayAndActive(!contextObject.Display);
            }
        }
    }

    public void InitializeContexts() 
    {
        List<UIContext> enabledContexts = ReturnEnabledContextsFromStartup();
        
        foreach (KeyValuePair<UIContext, List<UIContextObject>> kVP in _UIContexts) 
        {
            bool contextEnabled = enabledContexts.Contains(kVP.Key);
            foreach (UIContextObject contextObject in kVP.Value) contextObject.SetDisplayAndActive(contextEnabled, true); 
        }
    }

    private void Start()
    {
        InitializeContexts();
    }

    private List<UIContext> ReturnEnabledContextsFromStartup()
    {

        List<UIContext> selectedElements = new List<UIContext>();
        for (int i = 0; i < System.Enum.GetValues(typeof(UIContext)).Length; i++)
        {
            int layer = 1 << i;
            if (((int)EnabledContextsFromStartup & layer) != 0)
            {
                selectedElements.Add((UIContext)i);
            }
        }

        return selectedElements;
    }

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        current = this;
    }

    private void OnValidate()
    {
        _UpdateUIContexts();
    }

    private void OnEnable()
    {
        _UpdateUIContexts();
    }

    private void Reset()
    {
        _ClearUIContexts();
    }

    private void _ClearUIContexts() 
    {
        UIContexts.Clear();
        _UIContexts.Clear();
    }

    private void _UpdateUIContexts()
    {
        _UIContexts.Clear(); //Clear the existing context incase there is a drastic change to things

        foreach (SerializedPair<UIContext, UIContextObject> sKVP in UIContexts) //Go through the new context and load this into a nice dictionary
        {
            if (sKVP.Value == null) continue;


            if (_UIContexts.ContainsKey(sKVP.Key)) //If key already exists then add the value into the existing dictionary list
            {
                _UIContexts[sKVP.Key].Add(sKVP.Value);
            }
            else //If not then create a whole new object in the dictionary and then assign the current canvas group into that context
            {
                _UIContexts.Add(sKVP.Key, new List<UIContextObject> { sKVP.Value });
            }
        }
    }
}


public enum UIContext 
{
    Ingame,
    MainMenu,
    PauseMenu,
    Cutscene,
    DeathScreen
}