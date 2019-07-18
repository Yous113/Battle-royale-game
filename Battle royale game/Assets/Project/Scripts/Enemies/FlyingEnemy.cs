using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] private float distanceFromFloor;
    [SerializeField] private float bounceAmplitude;
    [SerializeField] private float bounceSpeed;

    private float bounceAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // Get the ground position
            Vector3 targetPosition = hit.point;

            // Move the enemy up
            targetPosition = new Vector3(
                targetPosition.x,
                targetPosition.y + distanceFromFloor,
                targetPosition.z
                );

            // Swing the enemy
            bounceAngle += Time.deltaTime * bounceSpeed;
            float offset = Mathf.Cos(bounceAngle) * bounceAmplitude;
            targetPosition = new Vector3(
                targetPosition.x,
                targetPosition.y + offset,
                targetPosition.z
                );

            // Apply the position
            transform.position = targetPosition;
        }
    }
}
