using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public int height, width; // height/width of our grid
    public static Vector2Int playerStart = new Vector2Int(0, 0);

    // rendering:
    public Tilemap tilemap;
    private ObjectSpawner render;
    public bool moveDisabled; // disable movements while renderer is playing. Hacky :(

    // collisions
    public enum Direction {North, East, South, West, None};
    private Overworld grid;

    // other things needed for each level
    public static Jay player;
    public static List<Follower> followers;
    public static List<Car> cars; // this sucks ugh

    // Start is called before the first frame update
    void Awake()
    {
        moveDisabled = false;

        player = new Jay(playerStart.x, playerStart.y);
        followers = new List<Follower> {
            new Cop(1, 0),
            new Cop(2, 0),
            new Cop(3, 0),
            new Cop(4, 0)
        };

        // Initialize the gridworld and spawn a tile object in it
        cars = new List<Car>();
        cars.Add(new Car(2, 5)); // one car

        grid = new Overworld(height, width);

        grid.SpawnTile(new ManHole(3, 3, grid));
        grid.SpawnLiving(player);
        foreach (Follower f in followers)
        {
            grid.SpawnLiving(f);
        }

        grid.SpawnLiving(new Cone(2, 2));

        render = tilemap.GetComponent<ObjectSpawner>();
        render.SetMap(grid.GetAllLiving(), grid.GetAllTiles(), cars);
    }

    // Update is called once per frame
    void Update()
    {
        if (moveDisabled)
        {
            return; // don't listen for key inputs while renderer is animating
        }

        Direction dir = Move();
        if (dir == Direction.None){
            return;
        }
        Vector2Int newPos = ApplyDir(player.position, dir);
        if (!grid.IsTileEmpty(newPos)){
            Debug.Log("Tile is occupied");
            return;
        }

        StartCoroutine(UpdateGameState(newPos));
    }

    // updates state of board and waits for animations to play
    // Entering this method assumes that move is valid.
    IEnumerator UpdateGameState(Vector2Int newPos)
    {
        moveDisabled = true; // players can't input additional moves while we're processing this one

        // Move Jay
        yield return StartCoroutine(MoveJay(newPos)); // then move the chain/animate

        // Check for cars/send them in
        yield return StartCoroutine(SendCars());

        moveDisabled = false;
        yield return null;
    }

    /*
    ALL HELPERS FOR UPDATE GAME STATE:
    (This is basically all GameManager does, so there are a LOT)
    */

    // Moves Jay and the chain behind him
    private IEnumerator MoveJay(Vector2Int newPos)
    {
        Vector2Int oldPos = player.position;
        grid.MoveLiving(player, newPos);
        yield return StartCoroutine(MoveChain(oldPos, 0));

        // then check if any tiles were triggered by movement
        yield return StartCoroutine(UpdateTiles());
    }

    // Moves a portion chain of followers to the given coords, starting from index *head*
    private IEnumerator MoveChain(Vector2Int newPos, int head)
    {
        Vector2Int oldPos;
        for (int i = head; i < followers.Count; i++)
        {
            oldPos = followers[i].position;
            grid.MoveLiving(followers[i], newPos);
            newPos = oldPos;
        }
        render.MoveSprites();
        yield return new WaitUntil(() => !render.IsInAnimation());
    }

    private IEnumerator KillFollower(Follower f) // Deletes follower *i* from the chain
    {
        int i = followers.IndexOf(f); // if this is slow, we can pass in int directly 

        Vector2Int dest = followers[i].position;
        grid.DeleteLiving(followers[i]);
        followers.RemoveAt(i);

        yield return StartCoroutine(MoveChain(dest, i));
    }

    // Animates cars running over living objects, populates killed with runover living
    private IEnumerator SendCars()
    {
        List<Follower> killed = new List<Follower>();
        List<int> carColumns = new List<int>();

        foreach (Car car in cars)
        {
            if (car.countDown()) // if countdown returns true
            {
                carColumns.Add(car.xPos);
                // kinda jank way of doing this... can be cleaned up
                foreach (Follower f in followers)
                {
                    if (f.position.x == car.xPos)
                        killed.Add(f);
                }
            }
        }

        /*--Animate those changes--*/
        render.MoveCars(killed, carColumns);
        yield return new WaitUntil(() => !render.IsInAnimation());

        // Then delete followers who were killed
        foreach (Follower f in killed)
        {
            yield return StartCoroutine(KillFollower(f));
        }
        // then check if any tiles were triggered by movement
        yield return StartCoroutine(UpdateTiles());
    }

    private IEnumerator UpdateTiles()
    {
        List<GameElement> spawns = new List<GameElement>(); 
        List<GameElement> despawns = new List<GameElement>();

        foreach (TileObject tile in grid.GetAllTiles())
        {
            tile.TileUpdate(grid.GetOccupant(tile));
        }

        if (spawns.Count > 0){
            Debug.Log("spawns " + spawns);
        }

        render.SpawnSprites(spawns);
        render.DespawnSprites(spawns);
        yield return null;
    }

    // Returns the tile Jay will move to, else returns the same position as Jay
    private Direction Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Vertical"))
        {
            if (moveVertical < 0 && player.position.y > 0)  // South
            {
                return Direction.South;
            }
            else if (moveVertical > 0 && player.position.y < height - 1)  // North
            {
                return Direction.North;
            }
        }
        else if (Input.GetButtonDown("Horizontal"))
        {
            if (moveHorizontal < 0 && player.position.x > 0)  // West
            {
                return Direction.West;
            }
            else if (moveHorizontal > 0 && player.position.x < width - 1)  // East
            {
                return Direction.East;
            }
        }
        return Direction.None;
    }

    // Returns coordinates 1 step in the given direction from the original coords
    private Vector2Int ApplyDir(Vector2Int orig, Direction dir)
    {
        switch (dir)
        {
        case Direction.South:
            return new Vector2Int(orig.x, orig.y - 1);
        case Direction.North:
            return new Vector2Int(orig.x, orig.y + 1);
        case Direction.West:
            return new Vector2Int(orig.x - 1, orig.y);
        case Direction.East:
            return new Vector2Int(orig.x + 1, orig.y);
        case Direction.None:
            return new Vector2Int(orig.x, orig.y);
        default: // This should never occur
            return new Vector2Int(orig.x, orig.y);
        }
    }
}
