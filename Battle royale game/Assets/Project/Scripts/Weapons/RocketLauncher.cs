﻿using System.Collections;
using System.Collections.Generic;

public class RocketLauncher : Weapon
{
    public RocketLauncher()
    {
        clipSize = 1;
        maxAmmunition = 5;
        reloadDuration = 3.0f;
        cooldownDuration = 0.5f;
        isAutomatic = false;
        name = "RPG";
        aimVariation = 0.01f;
    }
}
