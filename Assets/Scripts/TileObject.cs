using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : GameElement
{
    public bool blocked; // If blocked, then character can't move on this tile
    public Vector2Int position;
    // Start is called before the first frame update

    public TileObject(int x, int y)
    {
        this.position = new Vector2Int(x, y);
        this.eid = ElementType.Default; // Empty tile
        this.blocked = false;
    }
}