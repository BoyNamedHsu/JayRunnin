using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overworld
{
    // There are 2 layers to represent the gridworld:
    private TileObject[,] tGridworld; // Layer 1 : Tiles/Environment
    private LivingObject[,] lGridworld; // Layer 2 : Jay and followers

    public Overworld(int height, int width)
    {
        tGridworld = new TileObject[height, width];
        lGridworld = new LivingObject[height, width];
    }

    // Returns whether or not coords is occupied
    public bool IsTileEmpty(Vector2Int coords)
    {
        return lGridworld[coords.x, coords.y] == null;
    }

    // Returns the occupant of the given tile
    public LivingObject GetOccupant(TileObject tile)
    {
        return lGridworld[tile.position.x, tile.position.y];
    }

    // Methods for spawning tiles
    // returns false if the tile was already occupied
    public bool SpawnTile(TileObject tile)
    {
        if (this.tGridworld[tile.position.x, tile.position.y] != null){
            return false;
        }
        this.tGridworld[tile.position.x, tile.position.y] = tile;
        return true;
    }

    // Returns false if the delete failed
    public bool DeleteTile(TileObject tile)
    {
        if (this.tGridworld[tile.position.x, tile.position.y] != tile){
            return false;
        }
        this.tGridworld[tile.position.x, tile.position.y] = null;
        return true;
    }

    // Add T/F checks... after?
    public bool SpawnLiving(LivingObject living)
    {
        this.lGridworld[living.position.x, living.position.y] = living;
        return true;
    }

    // Returns false if the delete failed
    public bool DeleteLiving(LivingObject living)
    {
        this.lGridworld[living.position.x, living.position.y] = null;
        return true;
    }

    // Moves a living object to the given location. Fails if tile is occupied
    // or if the move failed
    // Updates the position of the given LivingObject
    public bool MoveLiving(LivingObject living, Vector2Int newPos)
    {
        this.DeleteLiving(living);
        living.position = newPos;
        this.SpawnLiving(living);
        return true;
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
}
