using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public readonly int triggerTurn; // turn this car drives
    public readonly int xPos; // col that car starts at, must be >= 0

    public Car(int x, int triggerTurn)
    {
        this.triggerTurn = triggerTurn;
        this.xPos = x;
    }
}
