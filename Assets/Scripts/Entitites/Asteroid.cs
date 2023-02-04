using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Asteroid : DamageableEntity
{
    public bool hasLimitedLifespan;
    public float maxLifeSpan = 2f;
    public float currentLifeSpan;

    [SerializeField]
    private Asteroid[] asteroidPrefabs;

    [SerializeField]
    public Rigidbody2D rb;

    [SerializeField]
    LayerMask damageableLayers;

    [SerializeField]
    private bool hasBeenConfigured;

    protected override void OnDeath(Vector3 hitPosition)
    {
        if (hasLimitedLifespan == false)
        {
            float angleOffset = 40;
            
            Vector3 hitDirectionNormalized = Vector3.Normalize(hitPosition - transform.position);
            hitDirectionNormalized = -hitDirectionNormalized;

            Vector3[] ForwardOffsets = GetConeOffsetForwardAngles(angleOffset, hitDirectionNormalized);

            foreach (Vector3 forward in ForwardOffsets) 
            {
                Vector3 newAPosition = transform.position + (forward * 1f);
                Asteroid newAsteroid = Instantiate(GetRandomAsteroidPrefab(), newAPosition, Quaternion.identity);
                newAsteroid.ConfigureAsteroid(true, transform.localScale / 4);
                newAsteroid.rb.AddForce(forward * 100f);
                newAsteroid.rb.AddTorque(Random.Range(-angleOffset, angleOffset));
            }
        }

        Destroy(gameObject);
    }


    public override void Damage(Vector3 hitPosition)
    {
        if (hasBeenConfigured == false)
            return;

        base.Damage(hitPosition);
    }

    private Vector3[] GetConeOffsetForwardAngles(float offsetAngle, Vector3 intialForward) 
    {
        Vector3[] forwardsArray = new Vector3[3];
        Quaternion spreadAngle1 = Quaternion.AngleAxis(offsetAngle, Vector3.forward);
        Quaternion spreadAngle2 = new Quaternion(spreadAngle1.x, spreadAngle1.y, -spreadAngle1.z, spreadAngle1.w);

        forwardsArray[0] = spreadAngle1 * intialForward;
        forwardsArray[1] = intialForward;
        forwardsArray[2] = spreadAngle2 * intialForward;

        return forwardsArray;
    }

    public void ConfigureAsteroid(bool hasLimitedLifespan, Vector3 scale) 
    {
        transform.localScale = scale;
        this.hasLimitedLifespan = hasLimitedLifespan;
        if (this.hasLimitedLifespan) currentLifeSpan = 0f;
        hasBeenConfigured = true;
    }

    private void Update()
    {
        if (hasLimitedLifespan) 
        {
            if (currentLifeSpan < maxLifeSpan)
            {
                currentLifeSpan += Time.deltaTime;
            }
            else 
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInLayerMask(collision.gameObject.layer, damageableLayers)) 
        {
            if (collision.gameObject.TryGetComponent(out DamageableEntity damageableEntity) == false)
                return;

            damageableEntity.Damage(transform.position);
        }
    }

    private Asteroid GetRandomAsteroidPrefab() 
    {
        return asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
