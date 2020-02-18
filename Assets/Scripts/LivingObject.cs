using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LivingObjects take up space in layer 1 (IE players, moving objects)
public class LivingObject : GameElement
{
    public LivingObject(int x, int y) : base(x, y){}
}