using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeTile : TileObject
{
    // Start is called before the first frame update
    public ConeTile(int x, int y) : base(x, y)
    {
        this.blocked = true;
        this.eid = ElementType.Cone;
    }
}
