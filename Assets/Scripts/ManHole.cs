using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Later this is going to be a subclass of "steppable"
public class ManHole : TileObject
{
    public ManHole(int x, int y) : base(x, y)
    {
        this.eid = ElementType.Zebra;
    }
}