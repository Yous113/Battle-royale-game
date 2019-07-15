using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Interface Elements")]
    [SerializeField] private Text resourcesText;
    [SerializeField] private Text resourcesRequirementText;
    [SerializeField] private Text weaponNameText;
    [SerializeField] private Text weaponAmmunitionText;
    [SerializeField] private RectTransform weaponReloadBar;

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
            if(value != Player.PlayerTool.None)
            {
                toolFocus.SetActive(true);
                targetFocusX = toolContainer.transform.GetChild((int)value).transform.position.x;
            }else
            {
                toolFocus.SetActive(false);
            }
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

    public void UpdateWeapon (Weapon weapon)
    {
        if(weapon == null)
        {
            weaponNameText.enabled = false;
            weaponAmmunitionText.enabled = false;
            weaponReloadBar.localScale = new Vector3(0, 1, 1);
        } else
        {
            weaponNameText.enabled = true;
            weaponAmmunitionText.enabled = true;

            weaponNameText.text = weapon.Name;
            weaponAmmunitionText.text = weapon.ClipAmmunition + " / " + weapon.TotalAmunnition;

            if(weapon.ReloadTimer > 0)
            {
                weaponReloadBar.localScale = new Vector3(weapon.ReloadTimer / weapon.ReloadDuration, 1, 1);
            } else
            {
                weaponReloadBar.localScale = new Vector3(0, 1, 1);
            }
        }
    }
}
