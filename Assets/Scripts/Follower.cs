using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Living
{
    private bool cop;

    public Follower(int x, int y, bool cop) : base(x, y)
    {
        this.cop = cop;
    }

/*    public (int x, int y) Move((int x, int )
    {
        (int x, int y) temp = position;
        if (direction == GameManager.Direction.South)
        {
            temp = (position.x + 1, position.y);
        }
        else if (direction == GameManager.Direction.North)
        {
            temp = (position.x - 1, position.y);
        }
        else if (direction == GameManager.Direction.East)
        {
            temp = (position.x, position.y + 1);
        }
        else 
        {
            temp = (position.x, position.y - 1);
        }
        return temp;
    }*/
}
