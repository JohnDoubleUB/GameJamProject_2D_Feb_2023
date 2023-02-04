using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

public class QuadrantManager : MonoBehaviour
{
    [SerializeField]
    private bool AllowAsteroids;

    [SerializeField]
    private float playerDistanceRadius = 4f;

    [SerializeField]
    private float asteroidCheckIntervalSeconds = 10f;

    private float currentCheckInterval = 0f;

    [SerializeField]
    private Asteroid[] asteroidPrefabs;

    [SerializeField]
    private float asteroidMinScale = 0.6f;
    [SerializeField]
    private float asteroidMaxScale = 2f;

    [SerializeField]
    private int asteroidSpawnCount = 300;

    [SerializeField]
    private SpriteRenderer spaceRenderer;

    public int xQuadrantCount = 2;
    public int yQuadrantCount = 2;

    public float[] yQuadrantBounds;
    public float[] xQuadrantBounds;
    public static QuadrantManager current;

    [SerializeField]
    private List<Quadrant> PossibleQuadrants = new List<Quadrant>();

    [SerializeField]
    private List<Vector2[]> CachedQuadrantBounds = new List<Vector2[]>();

    [SerializeField]
    List<Asteroid> LiveAsteroids = new List<Asteroid>();

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        current = this;

        Vector2[] Bounds = GetBoundsTopLeftRightBottomLeftRight();

        xQuadrantBounds = GenerateQuadrantBounds(Bounds[0].x, Bounds[1].x, xQuadrantCount);
        yQuadrantBounds = GenerateQuadrantBounds(Bounds[2].y, Bounds[0].y, yQuadrantCount);

        for (int i = 0; i < xQuadrantBounds.Length - 1; i++)
        {
            for (int j = 0; j < yQuadrantBounds.Length - 1; j++)
            {
                PossibleQuadrants.Add(new Quadrant { X = i, Y = j });
            }
        }

        foreach (Quadrant possibleQuadrant in PossibleQuadrants)
        {
            CachedQuadrantBounds.Add(GetBoundsForQuadrant(possibleQuadrant));
        }
    }

    private void Update()
    {
        if (AllowAsteroids)
        {
            if (currentCheckInterval < asteroidCheckIntervalSeconds)
            {
                currentCheckInterval += Time.deltaTime;
            }
            else
            {
                int missingAsteroids = asteroidSpawnCount - CleanNullsFromAsteroidsListAndGetCount();

                print("Missing asteroids " + missingAsteroids);

                if (missingAsteroids > 0)
                {
                    SpawnAsteroids(missingAsteroids);
                }

                foreach (Asteroid asteroid in LiveAsteroids)
                {
                    if (asteroid == null)
                        continue;

                    asteroid.rb.AddForce(UnityEngine.Random.insideUnitCircle * 500f);
                }

                currentCheckInterval = 0;
            }
        }
    }

    private int CleanNullsFromAsteroidsListAndGetCount()
    {
        for (int i = 0; i < LiveAsteroids.Count; i++) 
        {
            if (LiveAsteroids[i] == null) 
            {
                LiveAsteroids.RemoveAt(i);
                i--;
            }
        }

        return LiveAsteroids.Count;
    }

    private void Start()
    {
        if(AllowAsteroids) SpawnAsteroids(asteroidSpawnCount);
    }

    private void SpawnAsteroids(int asteroidSpawnCount) 
    {
        int quadrantCount = CachedQuadrantBounds.Count;

        for (int i = 0; i < asteroidSpawnCount; i++) 
        {
            Vector2[] quadrantToUse = CachedQuadrantBounds[i % quadrantCount];

            Vector2 randomPosition = Vector2.zero;

            do
            {
                randomPosition = new Vector2(
                    UnityEngine.Random.Range(quadrantToUse[0].x, quadrantToUse[1].x),
                    UnityEngine.Random.Range(quadrantToUse[2].y, quadrantToUse[0].y));

            } 
            while (Vector2.Distance(GameManager.current.Player.transform.position, randomPosition) < playerDistanceRadius);


            Asteroid spawnedAsteroid = Instantiate(asteroidPrefabs[UnityEngine.Random.Range(0, asteroidPrefabs.Length)], randomPosition, Quaternion.identity);
            spawnedAsteroid.ConfigureAsteroid(false, Vector3.one);
            spawnedAsteroid.rb.AddForce(UnityEngine.Random.insideUnitCircle * 500f);

            LiveAsteroids.Add(spawnedAsteroid);
        }
    }


    private float[] GenerateQuadrantBounds(float min, float max, int quadrantCount)
    {
        float[] result = new float[quadrantCount+1];

        result[0] = min;
        result[result.Length-1] = max;

        float individualQuadrantLength = Mathf.Abs(max - min) / quadrantCount;

        for (int i = 1; i < result.Length - 1; i++)
        {
            result[i] = result[i-1] + individualQuadrantLength;
        }

        return result;
    }

    public Vector2[] GetBoundsForQuadrant(Quadrant quadrant) 
    {
        Vector2[] bounds = new Vector2[4];
        float minX = xQuadrantBounds[quadrant.X];
        float maxX = xQuadrantBounds[quadrant.X + 1];
        float minY = yQuadrantBounds[quadrant.Y];
        float maxY = yQuadrantBounds[quadrant.Y + 1];

        bounds[0] = new Vector2(minX, maxY);
        bounds[1] = new Vector2(maxX, maxY);
        bounds[2] = new Vector2(minX, minY);
        bounds[3] = new Vector2(maxX, minY);

        return bounds;
    }
    public Quadrant GetQuadrant(Vector2 position) 
    {
        //find x quadrant
        int xQuadrant = xQuadrantBounds.Length - 1; //Default to last quadrant
        
        //Skip first quadrant because it has to be within bounds
        for (int i = 1; i < xQuadrantBounds.Length; i++) 
        {
            if (position.x > xQuadrantBounds[i] == false) 
            {
                xQuadrant = i - 1;
                break;
            }
        }

        int yQuadrant = yQuadrantBounds.Length - 1; //Default to last quadrant

        //Skip first quadrant because it has to be within bounds
        for (int i = 1; i < yQuadrantBounds.Length; i++)
        {
            if (position.y > yQuadrantBounds[i] == false)
            {
                yQuadrant = i - 1;
                break;
            }
        }

        return new Quadrant { X = xQuadrant, Y = yQuadrant };
    }

    private Vector2[] GetBoundsTopLeftRightBottomLeftRight() 
    {
        Bounds rendererBounds = spaceRenderer.bounds;

        Vector2 bottomLeft = rendererBounds.min;
        Vector2 topRight = rendererBounds.max;

        Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
        Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);

        return new Vector2[] { topLeft, topRight, bottomLeft, bottomRight };
    }

}

[System.Serializable]
public struct Quadrant 
{
    public int X;
    public int Y;
}
