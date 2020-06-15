using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Overworld
{
    // There are 2 layers to represent the overworld:
    private TileObject[,] tGridworld; // Layer 1 : Tiles/Environment
    private LivingObject[,] lGridworld; // Layer 2 : Jay and followers
    public readonly int height, width;

    // And a turn count is used to figure out when cars drive
    public int turnCount;
    public List<Car> cars;
    public List<Follower> followers;
    public Jay player;

    // And cop counters to check level completion
    public int copsDefeated, copsGoal;
    public bool alive;

    public Overworld(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.Clear();
    }

    // Clone constructor of a given overworld
    public Overworld(Overworld orig)
    {
        this.width = orig.width;
        this.height = orig.height;
        tGridworld = orig.tGridworld.Clone() as TileObject[,];
        lGridworld = orig.lGridworld.Clone() as LivingObject[,];
        this.turnCount = orig.turnCount;

        this.cars = new List<Car>(orig.cars);
        this.followers = new List<Follower>(orig.followers);
        this.player = orig.player;
    }

    public bool CopsKilled()
    {
        return this.copsDefeated >= this.copsGoal;
    }

    // Resets the grid
    public void Clear()
    {
        tGridworld = new TileObject[this.width, this.height];
        lGridworld = new LivingObject[this.width, this.height];
        turnCount = 0;
        this.cars = new List<Car>();
        this.alive = true;
    }

    // Adds the given car to this grid
    public void SpawnCar(Car car){
        this.cars.Add(car);
    }

    // Returns whether or not coords is occupied
    public bool TileIsEmpty(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= lGridworld.GetLength(0) 
            || coords.y < 0 || coords.y >= lGridworld.GetLength(1)){
            return false; // invalid indexes are occupied
        }
        return lGridworld[coords.x, coords.y] == null;
    }

    // Returns the occupant of the given tile
    public LivingObject GetOccupant(TileObject tile)
    {
        return lGridworld[tile.position.x, tile.position.y];
    }

    // Returns which tile is at coordinates (x, y)
    public TileObject GetTile(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= tGridworld.GetLength(0)
            || coords.y < 0 || coords.y >= tGridworld.GetLength(1))
        {
            return null; // invalid indexes are occupied
        }
        return tGridworld[coords.x, coords.y];
    }

    // Returns whether the given tile is of tyleType
    public bool IsElement(TileObject tile, GameElement.ElementType tyleType)
    {
        return tile != null && tile.eid == tyleType;
    }

    // Note - This can overwrite other TileObjects
    public void SpawnTile(TileObject tile)
    {
        this.tGridworld[tile.position.x, tile.position.y] = tile;
    }

    // Note - Can fail if tile is not on Grid
    public void DeleteTile(TileObject tile)
    {
        this.tGridworld[tile.position.x, tile.position.y] = null;
    }

    // Note - This can overwrite other Living
    public void SpawnLiving(LivingObject living)
    {
        this.lGridworld[living.position.x, living.position.y] = living;
    }

    // Note - Can fail if living is not on Grid
    public void DeleteLiving(LivingObject living)
    {
        this.lGridworld[living.position.x, living.position.y] = null;
    }

    // Moves a living object to the given location.
    // Unsafe - Can overwrite objects
    public void MoveLiving(LivingObject living, Vector2Int newPos)
    {
        this.DeleteLiving(living);
        living.position = newPos;
        this.SpawnLiving(living);
    }

    private bool IsSurrounded(Vector2Int pos)
    {
        return
            !this.TileIsEmpty(pos + new Vector2Int(0, 1)) &&
            !this.TileIsEmpty(pos + new Vector2Int(0, -1)) &&
            !this.TileIsEmpty(pos + new Vector2Int(1, 0)) &&
            !this.TileIsEmpty(pos + new Vector2Int(-1, 0));
    }

    private bool CarsLeft() // Returns whether there are any other cars that will trigger
    {
        foreach (Car car in this.cars)
        {
            if (car.triggerTurn > this.turnCount)
                return true;
        }
        return false;
    }
    public bool IsStuck() // Returns whether the player is in an unwinnable state
    {
        return !this.alive ||
            ((!this.CarsLeft() && !this.CopsKilled()) || this.IsSurrounded(this.player.position));
    }

    public List<LivingObject> GetAllLiving()
    {
        List<LivingObject> allLiving = new List<LivingObject>();
        for (int x = 0; x < this.lGridworld.GetLength(0); x++){
            for (int y = 0; y < this.lGridworld.GetLength(1); y++){
                LivingObject curr = this.lGridworld[x, y];
                if (curr != null){
                    allLiving.Add(curr);
                }
            }
        }
        return allLiving;
    }

    public List<TileObject> GetAllTiles()
    {
        List<TileObject> allTiles = new List<TileObject>();
        for (int x = 0; x < this.tGridworld.GetLength(0); x++){
            for (int y = 0; y < this.tGridworld.GetLength(1); y++){
                TileObject curr = this.tGridworld[x, y];
                if (curr != null){
                    allTiles.Add(curr);
                }
            }
        }
        return allTiles;
    }

    public HashSet<GameElement> GetAllObjects()
    {
        HashSet<GameElement> res = new HashSet<GameElement>();
        foreach (LivingObject l in this.GetAllLiving()){
            res.Add(l);
        }
        foreach (TileObject t in this.GetAllTiles()){
            res.Add(t);
        }
        return res;
    }
}
