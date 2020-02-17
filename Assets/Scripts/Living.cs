using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Living : GameElement
{
    public bool alive;
    public TileObject occupiedTile;

    public Living(int x, int y) : base(x, y)
    {
        this.alive = false;
    }
}
