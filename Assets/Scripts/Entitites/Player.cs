using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : DamageableEntity
{
    public Projectile projectilePrefab;

    public GameObject shipFire;

    public float minFireSize = 0;
    public float maxFireSize = 0;

    [SerializeField]
    private float movementSpeed = 1f;

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

    [SerializeField]
    private bool canAccelerate = true;

    [SerializeField]
    private bool canUseMouse = true;

    [SerializeField]
    private bool canFire = true;

    [SerializeField]
    private bool playerCanDie = true;

    private Vector3 GetPredictedPositionInSeconds(float predictionDistance)
    {
        return new Vector2(transform.position.x, transform.position.y) + (rb.velocity * predictionDistance);
    }


    public void SetEnableCrosshair(bool enable)
    {
        crossHair.SetActive(enable);
        canUseMouse = enable;
    }

    public void SetCanFire(bool enable)
    {
        canFire = enable;
    }

    public void SetCanAccelerate(bool enable)
    {
        canAccelerate = enable;
    }

    public void SetPlayerCanDie(bool enable) 
    {
        playerCanDie = enable;
    }

    private void Update()
    {
        if (GameManager.current.IsPaused) 
            return;

        if (shipFire != null)
        {
            float magnitudeValue = Mathf.Min(1, rb.velocity.magnitude);//Mathf.Clamp(rb.velocity.magnitude.Remap(0, 2, minFireSize, maxFireSize), minFireSize, maxFireSize);
            Vector3 newScale = Vector3.one * magnitudeValue;
            shipFire.transform.localScale = newScale;
        }



        if (canUseMouse)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;

            if (crossHair != null)
                crossHair.transform.position = mousePosition;

            Vector3 diff = mousePosition - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rot_z - 90);
        }


        Vector3 newForward = transform.up;


        if (canAccelerate && Input.GetButton("Accelerate"))
            rb.AddForce(newForward * (movementSpeed * 1000 * Time.deltaTime));

        predictedPosition = GetPredictedPositionInSeconds(predictedPositionDistance);

        if (canFire && Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }

    private void Fire()
    {
        foreach (Transform projectileSpawnLocation in projectileSpawnLocations)
        {
            SpawnProjectile(projectileSpawnLocation);
            AudioManager.current.AK_PlayEventAt("ShipFire", transform.position);
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
        if (playerCanDie)
        {
            SpawnDeathEffect();
            gameObject.SetActive(false);
            UIManager.current.SetActiveContexts(true, UIContext.DeathScreen);
        }
    }

    protected override void OnDamage(Vector3 hitPosition)
    {
        animator.Play("Hit");
    }
}
