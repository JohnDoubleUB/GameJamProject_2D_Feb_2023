using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DamageableEntity : MonoBehaviour
{
    public int Health = 1;
    public bool IsDead { get { return Health <= 0; } }
    public void Damage() 
    {
        Health = Mathf.Max(Health - 1, 0);

        if (Health == 0) 
        {
            OnDeath();
        }
    }

    public abstract void OnDeath();
}
