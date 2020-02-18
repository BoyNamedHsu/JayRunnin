using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : Follower
{
    public Fan(int x, int y) : base(x, y)
    {
        this.eid = ElementType.Fan;
    }
}
