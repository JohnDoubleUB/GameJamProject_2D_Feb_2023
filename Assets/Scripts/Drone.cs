using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : DamageableEntity
{
    public Projectile projectilePrefab;

    public Rigidbody2D rb;

    public Transform projectileSpawnLocation;


    public bool usePredictedPosition = true;

    private float TurnTimeMultiplier = 50f;
    public float projectileSpeed = 0.01f;
    public float projectileTimeToLive = 5f;


    private float minumumAimAngle = 70f;
    private float aimAngleMinModifier = 10f;
    private float aimAngleMaxModifier = 70f;

    public float baseFireInterval = 3f;
    public float fireIntervalVariation = 1f;
    private float currentFireInterval;

    private float fireInterval;

    private float maxDistanceFromPlayer = 6f;
    private float minDistanceFromPlayer;

    private void Start()
    {
        //Generate fire interval
        currentFireInterval = GenerateFireInterval(baseFireInterval, fireIntervalVariation);
    }

    private float GenerateFireInterval(float fireInterval, float fireIntervalVariation) 
    {
        return fireInterval + Random.Range(-fireIntervalVariation, fireIntervalVariation);
    }

    private void Update()
    {
        Vector3 playerPosition = usePredictedPosition ? Player.current.PredictedPosition : Player.current.transform.position;

        Quaternion targetRotation = GetRotationToTarget(playerPosition);
        
        float playerAimAngle = Quaternion.Angle(transform.rotation, targetRotation);

        float offAngleModifier = Mathf.Clamp(Mathf.Abs(playerAimAngle).Remap(aimAngleMinModifier, aimAngleMaxModifier, 1f, 4f), 1, 4f);

        float rotationModifier = TurnTimeMultiplier * offAngleModifier; 

        //Rotate towards player
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationModifier);

      

        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            rb.AddForce(transform.up * 0.5f);
        }
        else 
        {
            rb.AddForce(transform.up * offAngleModifier.Remap(1f, 4f, 0, 1));
        }

        if (playerAimAngle < minumumAimAngle)
        {
            if (fireInterval < currentFireInterval)
            {
                fireInterval += Time.deltaTime;
            }
            else 
            {
                Fire();
            }
        }
        else 
        {
            fireInterval = 0f;
        }
    }

    private void Fire() 
    {
        print("Fire");
        currentFireInterval = GenerateFireInterval(baseFireInterval, fireIntervalVariation);
        fireInterval = 0;
        SpawnProjectile();
    }

    private void SpawnProjectile() 
    {
        Projectile newProjectile = Instantiate(projectilePrefab, projectileSpawnLocation.position, transform.rotation);
        newProjectile.Speed = projectileSpeed;
        newProjectile.Owner = gameObject;
        newProjectile.SetTimeToLive(projectileTimeToLive);
    }

    private Quaternion GetRotationToTarget(Vector3 target)
    {
        Vector3 diff = target - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(transform.rotation.x, transform.rotation.y, rot_z - 90);
    }

    public override void OnDeath()
    {
        Destroy(gameObject);
    }
}
