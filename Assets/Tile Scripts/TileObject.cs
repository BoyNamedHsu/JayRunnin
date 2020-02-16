using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public enum TileType {Default, Cone, Zebra, Car};
    public bool blocked; // If blocked, then character can't move on this tile
    public TileType sid; // Unique ID for this tile
    // Start is called before the first frame update
    void Start()
    {   
        sid = TileType.Default; // Empty tile
        blocked = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
