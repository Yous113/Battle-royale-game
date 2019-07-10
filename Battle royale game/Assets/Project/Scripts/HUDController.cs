using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Interface Elements")]
    [SerializeField] private Text resourcesText;
    [SerializeField] private Text resourcesRequirementText;

    [Header("Tool selector")]
    [SerializeField] private GameObject toolFocus;
    [SerializeField] private GameObject toolContainer;
    [SerializeField] private float focusSmoothness;


    private float targetFocusX = 0;



    public int Resources {
        set
        {
            resourcesText.text = "Resources: " + value;
        }

    }

    public Player.PlayerTool Tool
    {
        set
        {
            targetFocusX = toolContainer.transform.GetChild((int)value).transform.position.x;
            if (value != Player.PlayerTool.ObstacleHorizontal &&
                value != Player.PlayerTool.ObstacleRamp &&
                value != Player.PlayerTool.ObstacleVertical
                )
            {
                resourcesRequirementText.enabled = false;
            }else
            {
                resourcesRequirementText.enabled = true;
            }
        }
    }

    private void Start()
    {
        targetFocusX = toolContainer.transform.GetChild(0).transform.position.x;
        toolFocus.transform.position = new Vector3(targetFocusX, toolFocus.transform.position.y);
    }

    private void Update()
    {
        toolFocus.transform.position = new Vector3(
          Mathf.Lerp(toolFocus.transform.position.x, targetFocusX, Time.deltaTime * focusSmoothness),
          toolFocus.transform.position.y
      );
    }

    public void UpdateResourcesRequirement(int cost, int balance)
    {
        resourcesRequirementText.text = "Requires: " + cost;
        if(balance < cost)
        {
            resourcesRequirementText.color = Color.red;
        } else
        {
            resourcesRequirementText.color = Color.white;
        }
    }
}
