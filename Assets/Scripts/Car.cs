using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public int countdown;
    public int xPos; // col that car starts at, must be >= 0

    // Start is called before the first frame update
    public Car(int x, int count)
    {
        this.countdown = count;
        this.xPos = x;
    }

    // tics the countDown of the current car
    // returns true if the car just triggered
    public bool countDown()
    {
        countdown--;
        if (countdown == 0) {
            Debug.Log("vroom");         
        }
        return countdown == 0;
    }
}
