using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopBlocker : LivingObject
{
    public CopBlocker(int x, int y) : base(x, y)
    {
        this.eid = ElementType.CopBlocker;
    }
}
