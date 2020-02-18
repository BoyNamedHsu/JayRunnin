using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteppableTile : TileObject
{
    public bool isTriggered;
    public void TileUpdate(LivingObject occupant) {
        // Someone stepped off the steppabletile
        if (occupant == null && isTriggered)
        {
            OffStep();
            isTriggered = false;
        } else if (occupant != null && !isTriggered)
        {
            OnStep();
            isTriggered = true;
        }
    }

    private void OnStep();
    private void OffStep();
}
