using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZebraTile : TileObject
{
    public override void TileUpdate (LivingObject occupant) {
        return;
    }

    public ZebraTile(int x, int y) : base (x, y){}
}
