using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Living
{
    private bool alive;
    public (int x, int y) position;

    public Living(int x, int y)
    {
        this.position = (x, y);
        this.alive = true;
    }
}
