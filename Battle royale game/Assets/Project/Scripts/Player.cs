﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerTool
    {
        Pickaxe,
        ObstacleVertical,
        ObstacleRamp,
        ObstacleHorizontal,
        None
        
    }

    [Header("Focal Point variables")]
    [SerializeField] private GameObject focalPoint;
    [SerializeField] private float focalDistance;
    [SerializeField] private float focalSmoothness;
    [SerializeField] private KeyCode changeFocalSideKey;

    [Header("Interaction")]
    [SerializeField] private GameCamera gameCamera;
    [SerializeField] private KeyCode interactionKey;
    [SerializeField] private float interactionDistance;

    [Header("Interface")]
    [SerializeField] private HUDController hud;

    [Header("Gameplay")]
    [SerializeField] private KeyCode toolSwitchKey;
    [SerializeField] private PlayerTool tool;
    [SerializeField] private float resourceCollectionCooldown;


    [Header("Obstacles")]
    [SerializeField] private GameObject obstaclePlacementContainer;
    [SerializeField] private GameObject[] obstaclePrefabs;

    private bool isFocalPointOnLeft = true;
    private int resources = 0;
    private float resourceCollectionCooldownTimer = 0;
    private GameObject currentObstacle;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        hud.Resources = resources;
        hud.Tool = tool;
    }

    // Update is called once per frame
    void Update()
    {
        // Update timers.
        resourceCollectionCooldownTimer -= Time.deltaTime;



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
        
        // Tool switch logic
        if (Input.GetKeyDown(toolSwitchKey))
        {
            SwitchTool();

        }

       //Tool usage Logic.

        if(Input.GetAxis("Fire1") > 0)
        {
            UseTool();
            
        }
    }

    private void SwitchTool()
    {
        PlayerTool previousTool = tool;

        //Cycle between the available tools.
        int currentToolIndex = (int)tool;
        currentToolIndex++;

        if (currentToolIndex == System.Enum.GetNames(typeof(PlayerTool)).Length)
        {
            currentToolIndex = 0;
        }
        // Get the new Tool
        tool = (PlayerTool)currentToolIndex;
        hud.Tool = tool;

        // Check for obstacle placement logic
        int obstacleToAddIndex = -1;
        if (tool == PlayerTool.ObstacleVertical)
        {
            obstacleToAddIndex = 0;
        }
        else if (tool == PlayerTool.ObstacleRamp)
        {
            obstacleToAddIndex = 1;
        }
        else if (tool == PlayerTool.ObstacleHorizontal)
        {
            obstacleToAddIndex = 2;
        }

        if (currentObstacle != null) Destroy(currentObstacle);
        if (obstacleToAddIndex >= 0)
        {
            currentObstacle = Instantiate(obstaclePrefabs[obstacleToAddIndex]);
            currentObstacle.transform.SetParent(obstaclePlacementContainer.transform);

            currentObstacle.transform.localPosition = Vector3.zero;
            currentObstacle.transform.localRotation = Quaternion.identity;
        }

    }

    private void UseTool()
    {
        if (tool == PlayerTool.Pickaxe)
        {
            RaycastHit hit;
            if (Physics.Raycast(gameCamera.transform.position, gameCamera.transform.forward, out hit, interactionDistance))
            {
                if (resourceCollectionCooldownTimer <= 0 && hit.transform.GetComponent<ResourceObject>() != null)
                {
                    resourceCollectionCooldownTimer = resourceCollectionCooldown;

                    ResourceObject resourceObject = hit.transform.GetComponent<ResourceObject>();
                    Debug.Log("Hit the object");
                    int collectedResources = resourceObject.Collect();

                    resources += collectedResources;
                    hud.Resources = resources;
                }
            }
        }
    }
}

