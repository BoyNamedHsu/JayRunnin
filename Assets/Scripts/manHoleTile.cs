using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manHoleTile : TileObject
{
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

    /*public checkActivate()
    {
        switch(ManHoleState)
        {
            case ManHoleState.Unoccupied:
            case ManHoleState.Occupied:
            case ManHoleState.Activated:
        }
    }*/

}
