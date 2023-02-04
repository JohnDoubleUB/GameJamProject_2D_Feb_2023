using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DamageableEntity : MonoBehaviour
{
    [SerializeField]
    private int health = 1;

    public int Health => health;
    public bool IsDead { get { return health <= 0; } }

    public delegate void HealthUpdate(int health, bool hasDied);
    public event HealthUpdate OnHealthUpdate;

    [SerializeField]
    private GameObject destroyEffectPrefab;
    
    [SerializeField]
    private bool spawnEffectOnDestroy = true;



    public void Damage() 
    {
        health = Mathf.Max(health - 1, 0);

        bool hasDied = health == 0;

        OnHealthUpdate?.Invoke(health, hasDied);

        if (hasDied) 
        {
            OnDeath();
        }
    }

    protected abstract void OnDeath();

    protected void OnDestroy()
    {
        if (spawnEffectOnDestroy == false)
            return;

        if (destroyEffectPrefab == null)
            return;

        Instantiate(destroyEffectPrefab, transform.position, transform.rotation);
    }
}
