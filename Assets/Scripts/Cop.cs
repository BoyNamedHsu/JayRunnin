using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cop : Follower
{
    public Cop(int x, int y) : base(x, y)
    {
        this.eid = ElementType.Cop;
    }
}
