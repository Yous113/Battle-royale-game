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
    protected float reloadDuration = 0.0f;
    protected float cooldownDuration = 0.0f;
    protected bool isAutomatic = false;
    protected string name = "";
    protected float aimVariation = 0.0f;
    protected float damage = 0.0f;

    //Private fields
    private float reloadTimer = -1f;
    private float cooldownTimer = 0.0f;
    private bool pressedTrigger = false;
    
    // Properties
    public int ClipAmmunition { get { return clipAmmunition; } set { clipAmmunition = value; } }
    public int TotalAmunnition { get { return totalAmmunition; } set { totalAmmunition = value; } }

    public int ClipSize { get { return clipSize; } }
    public int MaxAmunnition { get { return maxAmmunition; } }
    public float ReloadDuration { get { return reloadDuration; } }
    public float CooldownDuration { get { return cooldownDuration; } }
    public bool IsAutomatic { get { return isAutomatic; } }
    public string Name { get { return name; } }
    public float AimVariation { get { return aimVariation; } }
    public float Damage { get { return damage; } }

    public float ReloadTimer { get { return reloadTimer; } }

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

    public bool Update (float deltaTime, bool isPressingTrigger)
    {
        bool hasShot = false;

        // Cooldown logic.
        cooldownTimer -= deltaTime;
        if(cooldownTimer <= 0)
        {
            bool canShoot = false;

            if (isAutomatic) canShoot = isPressingTrigger;
            else if (!pressedTrigger && isPressingTrigger) canShoot = true;

            if(canShoot && reloadTimer <= 0.0f)
            {
                cooldownTimer = cooldownDuration;

                // Only shoot if there is any available bullets.
                if(clipAmmunition > 0)
                {
                    clipAmmunition--;
                    hasShot = true;
                }

                if(clipAmmunition == 0)
                {
                    // Automatically reload the weapon.
                    Reload();
                }
            }

            pressedTrigger = isPressingTrigger;
        }

        // Reload logic.

        if(reloadTimer > 0)
        {
            reloadTimer -= deltaTime;
            if (reloadTimer <= 0.0f)
            {
                LoadClip();
            }
        }

        return hasShot;
    }

    public void Reload()
    {
        // Only reload if the weapon is not currently reloading.
        // and the clip is not full and we have more bullets left.
        if (reloadTimer <= 0.0f && clipAmmunition < clipSize && totalAmmunition > 0)
        {
            reloadTimer = reloadDuration;
        }
    }
}

