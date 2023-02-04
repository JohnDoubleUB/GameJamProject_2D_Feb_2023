using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Quadrant QuadrantQuadrant;


    private void Start()
    {
       QuadrantQuadrant = QuadrantManager.current.GetQuadrant(new Vector2(transform.position.x, transform.position.y));
    }
}
