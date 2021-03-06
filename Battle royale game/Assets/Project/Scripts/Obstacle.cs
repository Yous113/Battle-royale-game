﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private int cost;
    [SerializeField] private float hitSmoothness;

    private Renderer obstacleRenderer;
    private int targetScale = 1;

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
        obstacleRenderer = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3
            (
            Mathf.Lerp(transform.localScale.x, targetScale, hitSmoothness * Time.deltaTime),
            Mathf.Lerp(transform.localScale.y, targetScale, hitSmoothness * Time.deltaTime),
            Mathf.Lerp(transform.localScale.z, targetScale, hitSmoothness * Time.deltaTime)
            );
    }

    public void SetPositioningMode()
    {
        // Start with the the obstacle collider disabled
        obstacleCollider.enabled = false;

        // Make the obstacle transparent.
        obstacleRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public void Place()
    {
        // Enable the collider
        obstacleCollider.enabled = true;

        //Make the obstacle opaque
        obstacleRenderer.material.color = Color.white;
    }

    public int Damage(float amount)
    {
        transform.localScale = Vector3.one * 0.8f;

        health -= amount;
        if (health <= 0)
        {
            targetScale = 0;
            Destroy(gameObject, 1.0f);
        }

        return 0;
    }
}
