using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameElement
{
    public enum ElementType 
        { Cop, Fan, Cone, Zebra, ManHole, Flagpole, Portal, Car, Jay, Default };
        // Portal and Default aren't spawnable

    public ElementType eid = ElementType.Default;
    public Vector2Int position;

    public GameElement(int x, int y)
    {
        this.position = new Vector2Int(x, y);
        this.eid = ElementType.Default; // Figure out a better way to do this later
    }
}
