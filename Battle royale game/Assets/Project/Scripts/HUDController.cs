using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Interface Elements")]
    [SerializeField] private Text resourcesText;

    [Header("Tool selector")]
    [SerializeField] private GameObject toolFocus;
    [SerializeField] private GameObject toolContainer;
    [SerializeField] private float focusSmothness;


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
        }
    }

    private void Start()
    {
        targetFocusX = toolContainer.transform.GetChild(0).transform.position.x;
        toolFocus.transform.position = new Vector3(0, toolFocus.transform.position.y);
    }

    private void Update()
    {
        toolFocus.transform.position = new Vector3
            (
            Mathf.Lerp(toolFocus.transform.position.x,
            targetFocusX, Time.deltaTime * focusSmothness),
            toolFocus.transform.position.y
            );
    }

}
