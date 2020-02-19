using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileObject : GameElement
{
    // TileUpdate is run each turn
    public abstract void TileUpdate(LivingObject occupant);

    public TileObject(int x, int y) : base (x, y){}
}