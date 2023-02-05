using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed;
    public GameObject Owner;

    private bool limitLifeTime = false;
    private float timeToLive;
    private float currentTimeToLive;

    private bool isVisible = true;


    [SerializeField]
    private LayerMask validLayerMasks;

    [SerializeField]
    private LayerMask borderLayerMask;

    [SerializeField]
    private string[] validTags;
    [SerializeField]
    private string borderTag;

    [SerializeField]
    private EffectObject EffectPrefab;

    private void Update()
    {
        transform.position += transform.up * (1000 * Speed * Time.deltaTime);


        if (limitLifeTime)
        {
            if (currentTimeToLive < timeToLive)
            {
                currentTimeToLive += Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Owner != null && Owner.layer == collision.gameObject.layer)
            return;

        if (IsInLayerMask(collision.gameObject.layer, borderLayerMask))
        {
            Destroy(gameObject);
        }

        if (IsInLayerMask(collision.gameObject.layer, validLayerMasks))
        {
            if (collision.gameObject.TryGetComponent(out DamageableEntity damageableEntity) == false) 
                return;

            damageableEntity.Damage(transform.position);
            Destroy(gameObject);
        }
    }

    public void SetTimeToLive(float timeToLive)
    {
        this.timeToLive = timeToLive;
        currentTimeToLive = 0;
        limitLifeTime = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnDestroy()
    {
        if (EffectPrefab == null)
            return;

        if (isVisible == false)
            return;

        Instantiate(EffectPrefab, transform.position, transform.rotation);
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
