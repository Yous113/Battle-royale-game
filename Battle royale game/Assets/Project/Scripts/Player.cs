using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName = "Yous";
    public int age = 15;
    private float health = 100.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello" + playerName);
        Debug.Log("You were born at" + (2018 - age));
        Debug.Log("Health:" + health);
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
