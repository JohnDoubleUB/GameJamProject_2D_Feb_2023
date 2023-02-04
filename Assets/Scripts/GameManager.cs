using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public Player Player;

    [SerializeField]
    private Text playerHealthField;

    [SerializeField]
    private int questProgression;
    private bool firstLoad;

    [SerializeField]
    private GameObject cutsceneDrones;

    private float keyPressTimer = 0.3f;
    [SerializeField]
    private float currentTimer;

    private int fireAmountBeforeProgression = 3;
    private int currentFireAmount;

    private float transitionBackToPlayerAfterSeconds = 6f;

    [SerializeField]
    [ReadOnlyField]
    private int dronesStillAlive = 9999;

    [SerializeField]
    private Drone[] objectiveDrones;

    SaveSystem<int> ProgressionSave;

    private bool loadingLevel;

    public bool LoadingLevel { get { return loadingLevel; } set { loadingLevel = value; } }


    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        current = this;

        ProgressionSave = new SaveSystem<int>()
        {
            SaveLocation = Application.persistentDataPath,
            SaveName = "ProgressionSave",
            SaveExtension = "OriginOfUs",
            VersioningMustMatch = false
        };
    }

    private void Start()
    {
        if (Player != null)
        {
            SetPlayerHealthUI(Player.Health);
            Player.OnHealthUpdate += OnPlayerHealthUpdate;
        }

        if (ProgressionSave.TryLoadDataFromXMLFile(out int progressionValue)) 
        {
            questProgression = progressionValue;
        }

        //Load game state
        //questProgression = 9;
        IntiateQuest(questProgression);

    }

    private void QuestUpdate(int progression)
    {
        switch (progression)
        {
            case 3:

                if (Input.GetButton("Accelerate"))
                {
                    if (currentTimer < keyPressTimer)
                    {
                        currentTimer += Time.deltaTime;
                    }
                    else
                    {
                        currentTimer = 0;
                        this.questProgression++;
                        IntiateQuest(this.questProgression);
                    }
                }

                break;

            case 6:
                if (Input.GetButtonDown("Fire1"))
                {
                    currentFireAmount++;
                }

                break;

            case 7:
                if (Input.GetButtonDown("Fire1"))
                {
                    currentFireAmount++;
                }


                if (currentFireAmount > fireAmountBeforeProgression)
                {
                    currentFireAmount = 0;
                    this.questProgression++;
                    IntiateQuest(this.questProgression);
                }
                break;

            case 8:

                if (currentTimer < transitionBackToPlayerAfterSeconds)
                {
                    currentTimer += Time.deltaTime;
                }
                else
                {
                    CameraFollow.current.SetFollowPlayer(true);
                }
                break;

            case 9:
                if (dronesStillAlive <= 0)
                {
                    this.questProgression++;
                    IntiateQuest(this.questProgression);
                }
                break;
        }
    }

    public void ReloadLevel() 
    {
        //loadingLevel = true;
        //QuadrantManager.current.RemoveAllAsteroids();
        
        //foreach (Drone drone in objectiveDrones) 
        //{
        //    if (drone == null)
        //        continue;

        //    Destroy(drone.gameObject);
        //}

        UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene();

        LoadLevel(scene);
        //SceneManager.LoadScene(scene.name);
    }

    public void LoadLevel(UnityEngine.SceneManagement.Scene scene) 
    {
        loadingLevel = true;
        QuadrantManager.current.RemoveAllAsteroids();

        foreach (Drone drone in objectiveDrones)
        {
            if (drone == null)
                continue;

            Destroy(drone.gameObject);
        }

        SceneManager.LoadScene(scene.name);
    }

    private void IntiateQuest(int progression)
    {
        print("Quest progression: " + progression);
        switch (progression)
        {
            case 0:
                Player.SetCanFire(false);
                Player.SetEnableCrosshair(false);
                Player.SetCanAccelerate(false);

                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA1", Player.gameObject, AK_CallbackFunction);
                break;
            case 1:
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA2", Player.gameObject, AK_CallbackFunction);
                Player.SetCanFire(false);
                Player.SetEnableCrosshair(false);
                Player.SetCanAccelerate(false);
                break;

            case 2:
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA3", Player.gameObject, AK_CallbackFunction);
                Player.SetCanFire(false);
                Player.SetEnableCrosshair(false);
                Player.SetCanAccelerate(false);
                //Allow player to accelerate using w

                currentTimer = 0;
                break;
            case 3:
                Player.SetCanFire(false);
                Player.SetEnableCrosshair(false);
                Player.SetCanAccelerate(true);
                break;

            case 4:
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA4", Player.gameObject, AK_CallbackFunction);
                Player.SetCanFire(false);
                Player.SetEnableCrosshair(false);
                Player.SetCanAccelerate(false);
                break;
            case 5:
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA5", Player.gameObject, AK_CallbackFunction);
                Player.SetEnableCrosshair(true);

                //Enable crosshair
                break;

            case 6:
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA6", Player.gameObject, AK_CallbackFunction);
                currentFireAmount = 0;
                Player.SetCanFire(true);
                //listen for firing
                break;

            case 7:
                //Listen for firing
                break;
            case 8:
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA7", Player.gameObject, AK_CallbackFunction);
                Player.SetCanFire(false);
                Player.SetEnableCrosshair(false);
                Player.SetCanAccelerate(false);
                CameraFollow.current.SetFollowPlayer(false);
                cutsceneDrones.SetActive(true);
                currentTimer = 0;

                //
                //Spawn new drones, unable to shoot
                //Enable Firing
                break;

            case 9:
                Player.SetCanFire(true);
                Player.SetEnableCrosshair(true);
                Player.SetCanAccelerate(true);
                cutsceneDrones.SetActive(false);
                QuadrantManager.current.InitializeAsteroids();
                objectiveDrones = QuadrantManager.current.SpawnDrones(10, false);
                dronesStillAlive = objectiveDrones.Length;

                ProgressionSave.SaveDataToXMLFile(questProgression);

                foreach (Drone drone in objectiveDrones)
                {
                    drone.OnDeathUpdate += OnDroneDeathUpdate;
                }

                break;

            case 10:
                AudioManager.current.AK_PlayClipOnObjectWithEndEventCallback("VA9", Player.gameObject, AK_CallbackFunction);
                Player.SetCanFire(true);
                Player.SetEnableCrosshair(true);
                Player.SetCanAccelerate(true);
                cutsceneDrones.SetActive(false);
                QuadrantManager.current.DisableFurtherAsteroids();
                ProgressionSave.SaveDataToXMLFile(questProgression);


                break;

            case 11:
                //Continue to the point and click times!
                break;

        }


    }

    private void OnDroneDeathUpdate()
    {
        dronesStillAlive--;
    }


    private void AK_CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        switch (in_type)
        {
            case AkCallbackType.AK_EndOfEvent:
                questProgression++;
                IntiateQuest(questProgression);

                break;

            default:
                break;
        }
    }

    private void Update()
    {
        QuestUpdate(questProgression);
    }


    private void OnPlayerHealthUpdate(int healthValue, bool hasDied)
    {
        SetPlayerHealthUI(healthValue);
        if (hasDied) PlayerDeath();
    }

    private void SetPlayerHealthUI(int value)
    {
        if (playerHealthField == null)
            return;

        playerHealthField.text = $"HEALTH: {value}";
    }

    private void PlayerDeath()
    {

    }

    private void OnDestroy()
    {
        if (Player != null)
        {
            Player.OnHealthUpdate -= OnPlayerHealthUpdate;
        }

        if (objectiveDrones != null)
        {
            foreach (Drone drone in objectiveDrones)
            {
                if (drone != null) drone.OnDeathUpdate -= OnDroneDeathUpdate;
            }
        }
    }
}
