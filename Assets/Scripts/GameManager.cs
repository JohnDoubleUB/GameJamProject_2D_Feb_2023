using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public Player Player;

    [SerializeField]
    private Text playerHealthField;


    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        current = this;
    }

    private void Start()
    {
        if (Player != null) 
        {
            SetPlayerHealthUI(Player.Health);
            Player.OnHealthUpdate += OnPlayerHealthUpdate;
        }



    }

    private void Update()
    {
        


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
    }
}
