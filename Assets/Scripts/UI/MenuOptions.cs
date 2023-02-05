using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour
{
    public static MenuOptions current;


    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        current = this;
    }

    private void Update()
    {
        //Set pause menu visibility here
    }

    public void RestartEntireGame() 
    {
        SaveSystem<int> progressionSave = new SaveSystem<int>()
        {
            SaveLocation = Application.persistentDataPath,
            SaveName = "ProgressionSave",
            SaveExtension = "OriginOfUs",
            VersioningMustMatch = false
        };

        progressionSave.SaveDataToXMLFile(0);


        //LoadLevel(0);
    }

    public void QuitGame() 
    {
        print("quiittt");
        Application.Quit();
    }

    public void StartGame() 
    {
        LoadLevel(1);
    }

    public void ReturnToMainMenu() 
    {
        AkSoundEngine.StopAll();
        LoadLevel(0);
    }


    private void LoadLevel(int BuildIndex)
    {
        SceneManager.LoadScene(BuildIndex);
    }
}


