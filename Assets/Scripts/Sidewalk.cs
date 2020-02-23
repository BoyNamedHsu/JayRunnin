using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidewalk : LivingObject
{
    public Sidewalk(int x, int y) : base(x, y)
    {
        this.eid = ElementType.Sidewalk;
    }
}
