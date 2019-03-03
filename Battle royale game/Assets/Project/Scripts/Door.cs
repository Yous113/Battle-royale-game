using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool opensInwards;
    [SerializeField] private float openingSpeed;

    private bool isOpen;
    private float targetAngel;
    
    // Start is called before the first frame update
    void Start()
    {
        Interact();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion smoothRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, targetAngel, 0), openingSpeed * Time.deltaTime);
        transform.localRotation = smoothRotation;
    }
    public void Interact ()
    {
        isOpen = !isOpen;
        if(isOpen)
        {
            if (opensInwards) targetAngel = -90.0f;
            else targetAngel = 90.0f;

        } else
        {
            targetAngel = 0;
        }
    }
}
