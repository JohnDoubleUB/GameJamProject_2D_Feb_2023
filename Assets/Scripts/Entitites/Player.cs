using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : DamageableEntity
{
    public Projectile projectilePrefab;

    //public static Player current;

    public Vector3 mousePosition;
    public GameObject crossHair;

    public Rigidbody2D rb;

    public float predictedPositionDistance = 0.2f;
    private Vector3 predictedPosition;

    [SerializeField]
    private Transform[] projectileSpawnLocations;
    [SerializeField]
    private float projectileTimeToLive = 5f;
    [SerializeField]
    private float projectileSpeed = 0.4f;

    public Vector3 PredictedPosition => predictedPosition;

    [SerializeField]
    private Animator animator;


    //private void Awake()
    //{
    //    if (current != null) return;

    //    current = this;
    //}

    private Vector3 GetPredictedPositionInSeconds(float predictionDistance) 
    {
        return new Vector2(transform.position.x, transform.position.y) + (rb.velocity * predictionDistance);
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 diff = mousePosition - transform.position;

        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rot_z - 90);

        Vector3 newForward = transform.up;

        if (crossHair != null) crossHair.transform.position = mousePosition;


        if (Input.GetButton("Accelerate"))
            rb.AddForce(newForward * 2f);

        predictedPosition = GetPredictedPositionInSeconds(predictedPositionDistance);

        if (Input.GetButtonDown("Fire1"))
        {
            print("bang");
            Fire();
        }
    }

    private void Fire() 
    {
        foreach (Transform projectileSpawnLocation in projectileSpawnLocations) 
        {
            SpawnProjectile(projectileSpawnLocation);
        }
    }

    private void SpawnProjectile(Transform spawnLocation)
    {
        Projectile newProjectile = Instantiate(projectilePrefab, spawnLocation.position, transform.rotation);
        newProjectile.Speed = projectileSpeed;
        newProjectile.Owner = gameObject;
        newProjectile.SetTimeToLive(projectileTimeToLive);
    }

    protected override void OnDeath(Vector3 hitPosition)
    {
        Destroy(gameObject);
    }

    protected override void OnDamage(Vector3 hitPosition)
    {
        animator.Play("Hit");
    }
}
