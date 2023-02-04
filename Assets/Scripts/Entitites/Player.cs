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

    [SerializeField]
    private bool canAccelerate = true;

    [SerializeField]
    private bool canUseMouse = true;

    [SerializeField]
    private bool canFire = true;

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

    private void Update()
    {

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
            rb.AddForce(newForward * 2f);

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
        SpawnDeathEffect();
        gameObject.SetActive(false);
        UIManager.current.SetActiveContexts(true, UIContext.DeathScreen);
        //GameManager.current.LoadingLevel = true;
    }

    protected override void OnDamage(Vector3 hitPosition)
    {
        animator.Play("Hit");
    }
}
