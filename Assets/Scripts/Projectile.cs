using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Projectile : MonoBehaviour
{
    public float Speed;
    public GameObject Owner;

    private bool limitLifeTime = false;
    private float timeToLive;
    private float currentTimeToLive;


    [SerializeField]
    private LayerMask validLayerMasks;

    [SerializeField]
    private LayerMask borderLayerMask;

    [SerializeField]
    private string[] validTags;
    [SerializeField]
    private string borderTag;

    private void Update()
    {
        transform.position += transform.up * Speed;


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
        if (Owner == null)
            return;

        //if (Owner.gameObject == collision.gameObject)
        //    return;

        if (Owner.layer == collision.gameObject.layer)
            return;

        if (IsInLayerMask(collision.gameObject.layer, borderLayerMask))
        {
            Destroy(gameObject);
        }

        if (IsInLayerMask(collision.gameObject.layer, validLayerMasks))
        {
            if (collision.gameObject.TryGetComponent(out DamageableEntity damageableEntity) == false) 
                return;

            damageableEntity.Damage();
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
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        print("cool effect");
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
