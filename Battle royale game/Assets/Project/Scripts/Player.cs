using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Focal Point variables")]
    [SerializeField] private GameObject focalPoint;
    [SerializeField] private float focalDistance;
    [SerializeField] private float focalSmoothness;
    [SerializeField] private KeyCode changeFocalSideKey;

    [Header("Interaction")]
    [SerializeField] private GameCamera gameCamera;
    [SerializeField] private KeyCode interactionKey;
    [SerializeField] private float interactionDistance;

    private bool isFocalPointOnLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(changeFocalSideKey))
        {
            isFocalPointOnLeft = !isFocalPointOnLeft;
        }

        float targetX = focalDistance * (isFocalPointOnLeft ? -1 : 1);
        float smoothX = Mathf.Lerp(focalPoint.transform.localPosition.x, targetX, focalSmoothness * Time.deltaTime);
        focalPoint.transform.localPosition = new Vector3(smoothX, focalPoint.transform.localPosition.y, focalPoint.transform.localPosition.z);

        // Interaction logic.
#if UNITY_EDITOR
        // Draw interaction line
        Debug.DrawLine(gameCamera.transform.position, gameCamera.transform.position + gameCamera.transform.forward * interactionDistance, Color.blue);
#endif
        if (Input.GetKeyDown(interactionKey))
        {
            RaycastHit hit;
            if (Physics.Raycast(gameCamera.transform.position, gameCamera.transform.forward, out hit, interactionDistance))
            {
                if(hit.transform.GetComponent<Door>())
                {
                    hit.transform.GetComponent<Door>().Interact();
                }
            }
        }
    }
}
