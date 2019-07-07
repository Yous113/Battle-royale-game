﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    [SerializeField] private int resourceAmount;
    [SerializeField] private int amountOfHit;
    [SerializeField] private float hitScale;
    [SerializeField] private float hitSmoothnees;


    private int hits;
    private float targetScale;
    
    // Start is called before the first frame update
    void Start()
    {
        targetScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
            Mathf.Lerp(transform.localScale.x, targetScale, Time.deltaTime * hitSmoothnees),
            Mathf.Lerp(transform.localScale.y, targetScale, Time.deltaTime * hitSmoothnees),
            Mathf.Lerp(transform.localScale.z, targetScale, Time.deltaTime * hitSmoothnees)
            );
    }

    public int Collect ()
    {
        hits++;
        transform.localScale = Vector3.one * hitScale;
        if (hits == amountOfHit)
        {
            Destroy(gameObject, 1);
            targetScale = 0;

            return resourceAmount;
        }

        return 0;
    }
}
