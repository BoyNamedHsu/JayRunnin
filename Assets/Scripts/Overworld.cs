using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Overworld : MonoBehaviour
{
    //There are 2 layers to represent the gridworld:
    private TileObject[,] eGridworld; // Layer 1 :Environment
    private Living[,] lGridworld; // Layer 2 : Jay and followers
    public int height;
    public int width;

    // Start is called before the first frame update
    void Awake()
    {
        // Generate environment
        initializeEnvironment();

        // Generate Second layer
        initializeLiving();

    }

    private void initializeEnvironment()
    {
        //Initializing new TileObjects to each index of array
        eGridworld = new TileObject[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                eGridworld[i, j] = new TileObject(i, j);
    }

    private void initializeLiving()
    {
        //Initializing new TileObjects to each index of array
        lGridworld = new Living[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                lGridworld[i, j] = new Living(i, j);

        lGridworld[GameManager.playerStart.x, GameManager.playerStart.y].eid = GameElement.ElementType.Jay; // Set Jay

        // Generate Followers
        for (int i = 1; i < GameManager.followers.Count(); i++)
        {
            lGridworld[GameManager.followers[i].position.x,
                        GameManager.followers[i].position.y].eid = GameElement.ElementType.Follower;
        }
    }

    public bool Move(Vector2Int prev, Vector2Int current, GameElement.ElementType character)
    {
        if (TileOccupied(current))
        {
            lGridworld[prev.x, prev.y].eid = GameElement.ElementType.Default;
            lGridworld[current.x, current.y].eid = character;
            return true;
        }
        return false;
    }

    // **Fix this method**
    public void kill(int col) {
        for (int x = 0; x < height; x++) { // check if living too...
            if (lGridworld[x, col].eid != GameElement.ElementType.Default) {
                lGridworld[x, col].alive = false;
                lGridworld[x, col].eid = GameElement.ElementType.Default;
            }
        }
    }

    // Returns whether or not coords is occupied
    public bool TileOccupied(Vector2Int coords)
    {   
        // return if tiles are unnocupied
        return (lGridworld[coords.x, coords.y].eid == GameElement.ElementType.Jay ||
               lGridworld[coords.x, coords.y].eid != GameElement.ElementType.Follower) &&
               !eGridworld[coords.x, coords.y].blocked;
    }
    
    // Spawns a tileobject at a certain coordinate
    public void spawnTile(Vector2Int coords, GameElement.ElementType eType)
    {
        Debug.Log("hello");
        TileObject newObj = new TileObject(coords.x, coords.y);
        switch (eType)
        {   
            case GameElement.ElementType.Cone:
                eGridworld[coords.x, coords.y] = new ConeTile(coords.x, coords.y);
                break;
            case GameElement.ElementType.Zebra:
                eGridworld[coords.x, coords.y] = new ZebraTile(coords.x, coords.y);
                break;
            case GameElement.ElementType.ManHole:
                //newObj = Instantiate(follower_sprite) as GameObject;
                eGridworld[coords.x, coords.y] = new ConeTile(coords.x, coords.y);
                break;
            default:
                Debug.Log("Spawn failed!");
                newObj = new ConeTile(coords.x, coords.y);
                eGridworld[coords.x, coords.y] = new TileObject(coords.x, coords.y);
                return;
        }
    }
}
