using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jay : LivingObject
{   
    public Jay (int x, int y) : base(x, y) {
        this.eid = ElementType.Jay;
    }
}
