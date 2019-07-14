using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int cost;

    public int Cost
    {
        get
        {
            return cost;
        }
    }

    private Collider obstacleCollider;
    
    // Start is called before the first frame update
    void Awake()
    {
        obstacleCollider = GetComponentInChildren<Collider>();

        // Start with the the obstacle collider disabled
        obstacleCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Place()
    {
        // Enable the collider
        obstacleCollider.enabled = true;
    }
}
