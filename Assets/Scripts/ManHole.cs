using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManHole : PressurePlate
{
    public ManHole(int x, int y, Func<TileObject, bool> OffStep, Func<TileObject, bool> OnStep) 
        : base(x, y, OffStep, OnStep, ElementType.ManHole){}
}