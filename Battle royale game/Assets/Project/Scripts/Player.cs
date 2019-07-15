using System.Collections;
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
    [SerializeField] private int initialResourceCount;
    [SerializeField] private float resourceCollectionCooldown;


    [Header("Obstacles")]
    [SerializeField] private GameObject obstaclePlacementContainer;
    [SerializeField] private GameObject obstacleContainer;
    [SerializeField] private GameObject[] obstaclePrefabs;

    [Header("Weapons")]
    [SerializeField] private GameObject shootOrigin;

    [Header("Debug")]
    [SerializeField] private GameObject DebugPositionPrefab;

    private bool isFocalPointOnLeft = true;
    private int resources = 0;
    private float resourceCollectionCooldownTimer = 0;
    private GameObject currentObstacle;
    private bool obstaclePlacementLock;

    private List<Weapon> weapons;

    private bool isUsingTools = true;
    private Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        resources = initialResourceCount;
        hud.Resources = resources;
        hud.Tool = tool;
        hud.UpdateWeapon(null);

        weapons = new List<Weapon>();
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
                if (hit.transform.GetComponent<Door>())
                {
                    hit.transform.GetComponent<Door>().Interact();
                }
            }
        }

        // Select weapons
        if(Input.GetKeyDown("1"))
        {
            SwitchWeapon(0);
        } else if (Input.GetKeyDown("2"))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            SwitchWeapon(2);
        }
        else if (Input.GetKeyDown("4"))
        {
            SwitchWeapon(3);
        }
        else if (Input.GetKeyDown("5"))
        {
            SwitchWeapon(4);
        }



        // Tool switch logic
        if (Input.GetKeyDown(toolSwitchKey))
        {
            SwitchTool();
        }

        //Preservering(Bevare) the obstacles horizontal rotation
        if (currentObstacle != null)
        {
            currentObstacle.transform.eulerAngles = new Vector3
                (
                0,
                currentObstacle.transform.eulerAngles.y,
                currentObstacle.transform.eulerAngles.z
                );
        }

        // Tool usage Logic (continuous).
        if (Input.GetAxis("Fire1") > 0.1f)
        {
            UseToolContinuous();
        }

        // Tool usage logic (trigger).
        if (Input.GetAxis("Fire1") > 0.1f)
        {
            if (!obstaclePlacementLock)
            {
                obstaclePlacementLock = true;
                UseToolTrigger();
            }
        }
        else
        {
            obstaclePlacementLock = false;

        }

        UpdateWeapon();

    }


    private void SwitchWeapon(int index)
    {
        if (index < weapons.Count)
        {

            isUsingTools = false;

            weapon = weapons[index];
            hud.UpdateWeapon(weapon);

            tool = PlayerTool.None;
            hud.Tool = tool;

            if (currentObstacle != null) Destroy(currentObstacle);
        }
    }

    private void SwitchTool()
        {

        isUsingTools = true;

        weapon = null;
        hud.UpdateWeapon(weapon);

       // PlayerTool previousTool = tool;

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

            hud.UpdateResourcesRequirement(currentObstacle.GetComponent<Obstacle>().Cost, resources);

        }

        }

    private void UseToolContinuous()
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

    private void UseToolTrigger()
        {
            if (currentObstacle != null && resources >= currentObstacle.GetComponent<Obstacle>().Cost)
            {
                int cost = currentObstacle.GetComponent<Obstacle>().Cost;
                resources -= cost;

                hud.Resources = resources;
                hud.UpdateResourcesRequirement(cost, resources);

                GameObject newObstacle = Instantiate(currentObstacle);
                newObstacle.transform.SetParent(obstacleContainer.transform);
                newObstacle.transform.position = currentObstacle.transform.position;
                newObstacle.transform.rotation = currentObstacle.transform.rotation;
                newObstacle.GetComponent<Obstacle>().Place();
            }
        }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<ItemBox>() != null)
        {
            ItemBox itemBox = otherCollider.gameObject.GetComponent<ItemBox>();

            GiveItem(itemBox.Type, itemBox.Amount);

            Destroy(otherCollider.gameObject);
        }
    }

    private void GiveItem(ItemBox.ItemType type, int amount)
    {
        // Create a weapon reference
        Weapon currentweapon = null;

        // Check if we already have an instance of this weapon.
        for (int i = 0; i < weapons.Count; i++)
        {
            if (type == ItemBox.ItemType.Pistol && weapons[i] is Pistol) currentweapon = weapons[i];
            else if (type == ItemBox.ItemType.MachineGun && weapons[i] is MachineGun) currentweapon = weapons[i];
        }

        // if we dont have a weaoin of this type, Create one, and add it to the list
        if (currentweapon == null)
        {
            if (type == ItemBox.ItemType.Pistol) currentweapon = new Pistol();
            else if (type == ItemBox.ItemType.MachineGun) currentweapon = new MachineGun();
            weapons.Add(currentweapon);
        }

        currentweapon.AddAmmunition(amount);
        currentweapon.LoadClip();

        if(currentweapon == weapon)
        {
            hud.UpdateWeapon(weapon);
        }

    }

    private void UpdateWeapon()
    {
        if(weapon != null)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                weapon.Reload();
            }
            float timeElapsed = Time.deltaTime;
            bool isPressingTrigger = Input.GetAxis("Fire1") > 0.1f;

            bool hasShot = weapon.Update(timeElapsed, isPressingTrigger);
                hud.UpdateWeapon(weapon);
            if(hasShot)
            {
                Shoot();
            }
        }
    }
    private void Shoot()
    {
        float distanceFromCamera = Vector3.Distance(gameCamera.transform.position, transform.position);
        RaycastHit targetHit;
        if (Physics.Raycast(gameCamera.transform.position + (gameCamera.transform.forward * distanceFromCamera), gameCamera.transform.forward, out targetHit))
        {
            Vector3 hitPosition = targetHit.point;
            hitPosition = new Vector3
           (
                hitPosition.x + Random.Range(-weapon.AimVariation, weapon.AimVariation), 
                hitPosition.y + Random.Range(-weapon.AimVariation, weapon.AimVariation), 
                hitPosition.z + Random.Range(-weapon.AimVariation, weapon.AimVariation)
                );

            Vector3 shootDirection = (hitPosition - shootOrigin.transform.position).normalized;

            RaycastHit shootHit;
            if(Physics.Raycast(shootOrigin.transform.position, shootDirection, out shootHit))
            {
                GameObject debugPositionInstance = Instantiate(DebugPositionPrefab);
                debugPositionInstance.transform.position = shootHit.point;
                Destroy(debugPositionInstance, 0.5f);

                GameObject target = shootHit.transform.gameObject;
                Debug.Log(target.name);

                if (target.GetComponent<Obstacle>() != null)
                {
                    target.GetComponent<Obstacle>().Hit();
                }

#if UNITY_EDITOR
                // Draw a line to show the shooting ray.
                Debug.DrawLine(shootOrigin.transform.position, shootOrigin.transform.position + shootDirection * 100, Color.red);
#endif
            }
        }
        

        if (Physics.Raycast(shootOrigin.transform.position, gameCamera.transform.forward, out targetHit))
        {
            GameObject target = targetHit.transform.gameObject;
            Debug.Log(target.name);
        }
    }

}






