﻿using System.Collections;
using System.Collections.Generic;

public abstract class Weapon
{
    // Ammunition fields
    private int clipAmmunition = 0;
    private int totalAmmunition = 0;

    // Weapon Settings (costumizable on weapon classes)
    protected int clipSize = 0;
    protected int maxAmmunition = 0;
    protected float reloadTime = 0.0f;
    protected float cooldownDuration = 0.0f;
    protected bool isAutomatic = false;
    protected string name = "";

    //Private fields
    private float reloadTimer = 0.0f;
    private float cooldownTimer = 0.0f;
    
    // Properties
    public int ClipAmmunition { get { return clipAmmunition; } set { clipAmmunition = value; } }
    public int TotalAmunnition { get { return totalAmmunition; } set { totalAmmunition = value; } }

    public int ClipSize { get { return clipSize; } }
    public int MaxAmunnition { get { return maxAmmunition; } }
    public float ReloadDuration { get { return reloadTime; } }
    public float CooldownDuration { get { return cooldownDuration; } }
    public bool IsAutomatic { get { return isAutomatic; } }
    public string Name { get { return name; } }

    public void AddAmmunition(int amount)
    {
        totalAmmunition = System.Math.Min(totalAmmunition + amount, maxAmmunition);
    }

    public void LoadClip()
    {
        int maximumAmunnitionToLoad = clipSize - ClipAmmunition;
        int ammunitionToLoad = System.Math.Min(maximumAmunnitionToLoad, totalAmmunition);


        clipAmmunition += ammunitionToLoad;
        totalAmmunition -= ammunitionToLoad;
    }

    public void Update (float deltaTime, bool isPressingTrigger)
    {
        cooldownTimer -= deltaTime;
        if(cooldownTimer <= 0)
        {
            bool canShoot = false;
        }
    }
}

