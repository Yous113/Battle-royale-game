using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Rocket")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionDamage;

    private Rigidbody rocketRigidbody;
    private float timer;
    
    // Start is called before the first frame update
    void Awake()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= lifetime)
        {
            Explode();
        }
    }

    public void Shoot (Vector3 direction)
    {
        transform.forward = direction;
        rocketRigidbody.velocity = direction * speed;
    }

    public void OnTriggerEnter(Collider otherCollider)
    {
        Explode();
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = transform.position;
        explosion.GetComponent<Explosion>().Explode(explosionRange, explosionDamage);
        Destroy(gameObject);
    }
}