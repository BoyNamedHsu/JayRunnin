using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone : LivingObject
{
    public Cone(int x, int y) : base(x, y)
    {
        this.eid = ElementType.Cone;
    }
}
