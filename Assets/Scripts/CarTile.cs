using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTile : TileObject
{
    public int countdown;
    public bool gone;
    public int xPos; // col that car starts at, must be >= 0
    // Start is called before the first frame update
    public CarTile(int x, int count) : base(x, 0)
    {
        this.eid = ElementType.Car;
        this.countdown = count;
        this.gone = false;
        this.xPos = x;
    }

    public void countDown()
    {
        if (!gone && countdown == 1) {
            // Car animation
            Debug.Log("vroom");
            gone = true;
         
        }
        countdown--;

    }
}
