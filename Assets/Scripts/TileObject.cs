using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileObject : GameElement
{
    // there HAS to be a better way to do this but... I give up lmao
    public Overworld grid;

    // TileUpdate is run each turn
    public abstract void TileUpdate(LivingObject occupant);

    public TileObject(int x, int y, Overworld grid) : base (x, y)
    {
        this.grid = grid;
    }
}