using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu current;

    public bool AllowPauseMenu;

    private bool pauseMenuActive;

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        current = this;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) 
        {
            pauseMenuActive = !pauseMenuActive;
        }

        if (AllowPauseMenu == false && pauseMenuActive) 
        {
            //Force close pause menu
        }


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


        LoadLevel(0);
    }

    public void QuitGame() 
    {
        Application.Quit();
    }


    private void LoadLevel(int BuildIndex)
    {
        SceneManager.LoadScene(BuildIndex);
    }
}


