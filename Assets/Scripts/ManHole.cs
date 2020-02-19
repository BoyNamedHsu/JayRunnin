using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Later this is going to be a subclass of "steppable"
public class ManHole : PressurePlate
{
    public ManHole(int x, int y, Overworld grid) 
        : base(x, y, grid)
    {
        this.eid = ElementType.ManHole;
    }

    public override void OnStep(){
        Debug.Log("STEP ON!");
        return; // Nothing needed on step
    }
    public override void OffStep(){
        Debug.Log("STEP OFF!");
        LivingObject cop = new Cop(position.x, position.y);

        grid.SpawnLiving(cop);
        grid.DeleteTile(this);
        return;
    }
}