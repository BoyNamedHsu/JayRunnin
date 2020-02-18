using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Later this is going to be a subclass of "steppable"
public class ManHole : TileObject
{
    public ManHole(int x, int y) : base(x, y)
    {
        this.eid = ElementType.Zebra;
    }

    /*
    enum ManHoleState {Unoccupied, Occupied, Activated}
    private ManHoleState state;
    public Follower cop;
    private Living occupant;

    public manHoleTile(int x, int y) : base(x, y)
    {
        this.eid = ElementType.ManHole; // Empty tile
        state = ManHoleState.Unoccupied;
        cop = null;
    }
    */
}