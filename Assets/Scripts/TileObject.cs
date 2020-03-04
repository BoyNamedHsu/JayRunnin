using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : GameElement
{
    // TileUpdate is run each turn
    public virtual void TileUpdate(LivingObject occupant){}

    public TileObject(int x, int y, ElementType eid = ElementType.Default) : base (x, y){
        this.eid = eid;
    }
}