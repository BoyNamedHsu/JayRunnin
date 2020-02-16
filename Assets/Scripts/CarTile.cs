using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTile
{
    public int countdown;
    public bool gone;
    public int yPos; // col that car starts at, must be >= 0
    // Start is called before the first frame update
    public CarTile(int y, int count)
    {
        this.countdown = count;
        this.gone = false;
        this.yPos = y;
    }

    public void setCountdown(int count)
    {
        this.countdown = count;
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
