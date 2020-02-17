using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Living : GameElement
{
    public bool alive;
    public Vector2Int position;

    public Living(int x, int y)
    {
        this.alive = false;
        this.position = new Vector2Int(x, y);
        this.eid = ElementType.Default;
    }
}
