using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class levelOne : Level
{
    private Overworld grid;
    private Jay player;
    private static List<Follower> followers;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Overworld(8, 10);
        player = new Jay(0, 4);
        
        
        followers = new List<Follower> {
        };

        grid.SpawnTile(CreateFlagpole(9, 4));

        grid.SpawnLiving(player);
        foreach (Follower f in followers)
        {
            grid.SpawnLiving(f);
        }

        grid.SpawnCar(new Car(7, 7));

        // Instantiate(GameManager);
    }
    Func<TileObject, bool> TileNoop = (TileObject tile) => {return true;};

    private PressurePlate CreateManhole(int x, int y)
    {
        Func<TileObject, bool> CopSpawner = (TileObject tile) => {
            Follower cop = new Cop(tile.position.x, tile.position.y);
            grid.DeleteTile(tile);
            grid.SpawnLiving(cop);
            followers.Add(cop);
            return true;
        };

        return new PressurePlate(x, y, CopSpawner, TileNoop, 
            GameElement.ElementType.ManHole);
    }

    private PressurePlate CreateZebraTile(int x, int y)
    {
        Func<TileObject, bool> DecrementTurn = (TileObject tile) => {
            grid.turnCount--;
            return true;
        };

        return new PressurePlate(x, y, TileNoop, DecrementTurn, 
            GameElement.ElementType.Zebra);
    }

    private PressurePlate CreateFlagpole(int x, int y)
    {
        Func<TileObject, bool> Win = (TileObject tile) => {
            Debug.Log("You won!");
            grid.Clear();
            return true;
        };

        return new PressurePlate(x, y, TileNoop, Win, 
            GameElement.ElementType.Flagpole);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
