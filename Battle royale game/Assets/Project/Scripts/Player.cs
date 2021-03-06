﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour, IDamageable
{
    public delegate void DiedDelegate();
    public event DiedDelegate OnPlayerDied;

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
    [SerializeField] private GameObject rotationPoint;
    [SerializeField] private float focalDistance;
    [SerializeField] private float focalSmoothness;
    [SerializeField] private KeyCode changeFocalSideKey;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey;
    [SerializeField] private float interactionDistance;

    [Header("Gameplay")]
    [SerializeField] private KeyCode toolSwitchKey;
    [SerializeField] private PlayerTool tool;
    [SerializeField] private int initialResourceCount;
    [SerializeField] private float resourceCollectionCooldown;


    [Header("Obstacles")]
    [SerializeField] private GameObject[] obstaclePrefabs;

    [Header("Weapons")]
    [SerializeField] private GameObject shootOrigin;
    [SerializeField] private GameObject rocketPrefab;

    [Header("Debug")]
    [SerializeField] private GameObject DebugPositionPrefab;

    private bool isFocalPointOnLeft = true;
    private int resources = 0;
    private float resourceCollectionCooldownTimer = 0;
    private GameObject currentObstacle;
    private bool obstaclePlacementLock;

    private List<Weapon> weapons;
    
    private Weapon weapon;
    private Vector3 shootDirection;

    private HUDController hud;
    private GameCamera gameCamera;
    private GameObject obstaclePlacementContainer;
    private GameObject obstacleContainer;
    private int obstacleToAddIndex;
    private Health health;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Initialize values
        resources = initialResourceCount;
        weapons = new List<Weapon>();
        GetComponent<Health>();
       // health.OnHealthChanged += OnHealthChanged;
       
        if (isLocalPlayer)
        {
            // Game camera
            gameCamera = FindObjectOfType<GameCamera>();
            obstaclePlacementContainer = gameCamera.ObstaclePlacementContainer;
            gameCamera.Target = focalPoint;
            gameCamera.RotationAnchorObject = rotationPoint;

            // HUD elements
            hud = FindObjectOfType<HUDController>();
            hud.ShowScreen("regular");
            hud.Health = health.Value;
            hud.Resources = resources;
            hud.Tool = tool;
            hud.UpdateWeapon(null);
        }
        
        // Obstacle container
        obstacleContainer = GameObject.Find("ObstacleContainer");

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

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
        //Debug.DrawLine(gameCamera.transform.position, gameCamera.transform.position + gameCamera.transform.forward * interactionDistance, Color.blue);
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

            weapon = weapons[index];
            hud.UpdateWeapon(weapon);

            tool = PlayerTool.None;
            hud.Tool = tool;

            if (currentObstacle != null) Destroy(currentObstacle);

            //Zoom Out
            if(!(weapon is Sniper))
            {
                gameCamera.ZoomOut();
                hud.SniperAimVisibility = false;
            }
        }
    }

    private void SwitchTool()
        {

        weapon = null;
        hud.UpdateWeapon(weapon);

        // Zoom the camera out.
            gameCamera.ZoomOut();
        hud.SniperAimVisibility = false;

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
        obstacleToAddIndex = -1;
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

            currentObstacle.GetComponent<Obstacle>().SetPositioningMode();

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
                        int collectedResources = resourceObject.Damage(1);

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

                CmdPlaceObstacle(obstacleToAddIndex, currentObstacle.transform.position, currentObstacle.transform.rotation);
            }
        }

    [Command]
    void CmdPlaceObstacle (int index, Vector3 position, Quaternion rotation)
    {
        GameObject newObstacle = Instantiate(obstaclePrefabs[index]);
        newObstacle.transform.SetParent(obstacleContainer.transform);
        newObstacle.transform.position = position;
        newObstacle.transform.rotation = rotation;
        newObstacle.GetComponent<Obstacle>().Place();

        NetworkServer.Spawn(newObstacle);
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (!isLocalPlayer) return;

        if (otherCollider.gameObject.GetComponent<ItemBox>() != null)
        {
            ItemBox itemBox = otherCollider.gameObject.GetComponent<ItemBox>();

            GiveItem(itemBox.Type, itemBox.Amount);

            CmdCollectBox(otherCollider.gameObject);
        }
    }

    [Command]
    void CmdCollectBox(GameObject box)
    {
        Destroy(box);
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
            else if (type == ItemBox.ItemType.Shotgun && weapons[i] is Shotgun) currentweapon = weapons[i];
            else if (type == ItemBox.ItemType.Sniper && weapons[i] is Sniper) currentweapon = weapons[i];
            else if (type == ItemBox.ItemType.RocketLauncher && weapons[i] is RocketLauncher) currentweapon = weapons[i];

        }

        // if we dont have a weaoin of this type, Create one, and add it to the list
        if (currentweapon == null)
        {
            if (type == ItemBox.ItemType.Pistol) currentweapon = new Pistol();
            else if (type == ItemBox.ItemType.MachineGun) currentweapon = new MachineGun();
            else if (type == ItemBox.ItemType.Shotgun) currentweapon = new Shotgun();
            else if (type == ItemBox.ItemType.Sniper) currentweapon = new Sniper();
            else if (type == ItemBox.ItemType.RocketLauncher) currentweapon = new RocketLauncher();
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

            //Zoom logic
            if(weapon is Sniper)
            {
                if(Input.GetMouseButtonDown(1))
                {
                    gameCamera.TriggerZoom();
                    hud.SniperAimVisibility = gameCamera.isZoomedIn;
                } 
               
            }
        }
    }
    private void Shoot()
    {
        int amountOfBullets = 1;

        if (weapon is Shotgun)
        {
            amountOfBullets = ((Shotgun)weapon).AmountOfBullets;
        }
        for (int i = 0; i < amountOfBullets; i++)
        {
            float distanceFromCamera = Vector3.Distance(gameCamera.transform.position, transform.position);
            RaycastHit targetHit;
            if (Physics.Raycast(gameCamera.transform.position + (gameCamera.transform.forward * distanceFromCamera), gameCamera.transform.forward, out targetHit))
            {
                Vector3 hitPosition = targetHit.point;

                Vector3 shootDirection = (hitPosition - shootOrigin.transform.position).normalized;
                shootDirection = new Vector3
                    (
                    shootDirection.x + Random.Range(-weapon.AimVariation, weapon.AimVariation),
                    shootDirection.y + Random.Range(-weapon.AimVariation, weapon.AimVariation),
                    shootDirection.z + Random.Range(-weapon.AimVariation, weapon.AimVariation)
                    );
                shootDirection.Normalize();

                if (!(weapon is RocketLauncher))
                {
                    RaycastHit shootHit;
                    if (Physics.Raycast(shootOrigin.transform.position, shootDirection, out shootHit))
                    {
                        GameObject debugPositionInstance = Instantiate(DebugPositionPrefab);
                        debugPositionInstance.transform.position = shootHit.point;
                        Destroy(debugPositionInstance, 0.5f);


                        // Testing.
                        // Debug.Log(target.name);

                        if (shootHit.transform.GetComponent<IDamageable>() != null)
                        {
                            CmdDamage(shootHit.transform.gameObject, weapon.Damage);;
                        }
                        else if (shootHit.transform.GetComponentInParent<IDamageable>() != null)
                        {
                            CmdDamage(shootHit.transform.parent.gameObject, weapon.Damage);
                        }

#if UNITY_EDITOR
                        // Draw a line to show the shooting ray.
                        Debug.DrawLine(shootOrigin.transform.position, shootOrigin.transform.position + shootDirection * 100, Color.red);
#endif
                    }



                    if (Physics.Raycast(shootOrigin.transform.position, gameCamera.transform.forward, out targetHit))
                    {
                        GameObject target = targetHit.transform.gameObject;

                        //Just for testing
                        //Debug.Log(target.name);
                    }
                }
                else
                {
                    GameObject rocket = Instantiate(rocketPrefab);
                    rocket.transform.position = shootOrigin.transform.position + shootDirection;
                    rocket.GetComponent<Rocket>().Shoot(shootDirection);
                }
            }
        }
    }

    [Command]
    private void CmdDamage (GameObject target, float damage)
    {
        target.GetComponent<IDamageable>().Damage(damage);
    }

    public int Damage(float amount)
    {
        GetComponent<Health>().Damage(amount);
        return 0;
    }

    private void OnHealthChanged (float newHealth)
    {
        hud.Health = newHealth;
        if(newHealth < 0.01f)
        {
            Destroy(gameObject);
        }
    }
}