using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : TileObject
{
    public bool isTriggered;

    private Func<TileObject, bool> OffStep;
    private Func<TileObject, bool> OnStep;

    public PressurePlate(int x, int y, Func<TileObject, bool> OffStep, 
        Func<TileObject, bool> OnStep, ElementType eid) : base (x, y)
        {
            this.OffStep = OffStep;
            this.OnStep = OnStep;
            this.eid = eid;
        }

    public override void TileUpdate (LivingObject occupant) {
        // Someone stepped off the steppabletile
        if (occupant == null && isTriggered)
        {
            isTriggered = false;
            OffStep(this);
        } else if (occupant != null && !isTriggered)
        {
            isTriggered = true;
            OnStep(this);
        }
    }
}
