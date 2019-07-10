using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int cost;

    private Renderer obstacleRenderer;

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

        // Transparency
        obstacleRenderer = GetComponentInChildren<Renderer>();
        obstacleRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void place()
    {
        // Enable the collider
        obstacleCollider.enabled = true;

        //Make the obstacle opaque
        obstacleRenderer.material.color = Color.white;
    }
}
