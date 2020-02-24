using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : LivingObject
{
    public InvisibleWall(int x, int y) : base(x, y)
    {
        this.eid = ElementType.InvisibleWall;
    }
}
