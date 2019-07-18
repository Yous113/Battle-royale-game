using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody rocketRigidbody;
    
    // Start is called before the first frame update
    void Awake()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot (Vector3 direction)
    {
        transform.forward = direction;
        rocketRigidbody.velocity = direction * speed;
    }
}
