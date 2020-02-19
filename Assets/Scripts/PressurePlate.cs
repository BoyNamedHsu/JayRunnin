using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PressurePlate : TileObject
{
    public bool isTriggered;

    public abstract void OffStep();
    public abstract void OnStep();

    public PressurePlate(int x, int y, Overworld grid) 
        : base (x, y, grid){}

    public override void TileUpdate (LivingObject occupant) {
        // Someone stepped off the steppabletile
        if (occupant == null && isTriggered)
        {
            isTriggered = false;
            OffStep();
        } else if (occupant != null && !isTriggered)
        {
            isTriggered = true;
            OnStep();
        }
    }
}
