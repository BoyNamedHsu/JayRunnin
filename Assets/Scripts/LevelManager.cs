using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // rendering:
    public Tilemap tilemap;
    private OverworldRenderer render;

    private bool moveDisabled; // disable movements while renderer is playing
    private bool alive; // checks if the player is alive

    // collisions
    public enum Direction {North, East, South, West, None};
    private Overworld grid;

    // other things needed for each level
    private Jay player;
    private List<Follower> followers;

    // Start is called before the first frame update
    void Awake()
    {
        moveDisabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveDisabled)
        {
            return; // don't listen for key inputs while renderer is animating
        }

        Direction dir = GetKeyboardDir();
        if (dir == Direction.None){
            return;
        }
        Vector2Int newPos = ApplyDir(player.position, dir);
        if (!grid.TileIsEmpty(newPos)){
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
        grid.turnCount++;

        // Move Jay
        yield return StartCoroutine(MoveJay(newPos)); // then move the chain/animate

        // Check for cars/send them in
        yield return StartCoroutine(SendCars());

        if (alive){
            moveDisabled = false;
        }
        yield return null;
    }

    private void FinishLvl()
    {
        grid.Clear();
        render.SyncSprites(grid);
        alive = false;
    }

    // Might be worth considering a coroutine instead of coupling this with flags but this works
    private void WinLvl()
    {
        Debug.Log("You won!");
        FinishLvl();
        LevelSelector.levelChosen++;
        SceneManager.LoadScene("Level");
    }

    private void LoseLvl()
    {
        Debug.Log("You died");
        FinishLvl();
        SceneManager.LoadScene("Level");
    }

    /*
    ALL HELPERS FOR UPDATE GAME STATE BELOW:
    (This is basically all GameManager does, so there are a LOT)
    */

    // Moves Jay and the chain behind him
    private IEnumerator MoveJay(Vector2Int newPos)
    {
        Vector2Int oldPos = player.position;
        grid.MoveLiving(player, newPos);
        yield return StartCoroutine(MoveChain(oldPos, 0));
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

        // then check if any tiles were triggered by movement
        yield return StartCoroutine(UpdateTiles());
    }

    private IEnumerator KillFollower(Follower f) // Deletes follower *i* from the chain
    {
        int i = followers.IndexOf(f); // if this is slow, we can pass in int directly 

        Vector2Int dest = followers[i].position;
        grid.DeleteLiving(followers[i]);
        followers.RemoveAt(i);
        render.SyncSprites(grid); // sync, since we just deleted it

        yield return StartCoroutine(MoveChain(dest, i));
    }

    // Animates cars running over living objects, populates killed with runover living
    private IEnumerator SendCars()
    {
        List<Follower> killed = new List<Follower>();
        List<int> carColumns = new List<int>();

        List<Car> cars = grid.cars;

        for (int i = cars.Count - 1; i >= 0; i--){
            Car car = cars[i];

            if (car.triggerTurn == grid.turnCount)
            {
                carColumns.Add(car.xPos);
                // kinda jank way of doing this... can be cleaned up
                foreach (Follower f in followers)
                {
                    if (f.position.x == car.xPos)
                        killed.Add(f);
                }
                cars.RemoveAt(i); // then consume that car
            }
        }

        /*--Animate those changes--*/
        render.MoveCars(killed, carColumns, grid);
        yield return new WaitUntil(() => !render.IsInAnimation());

        // Check if the car killed the player
        if (carColumns.Contains(player.position.x)){
            LoseLvl(); // This is still glitchy
            yield return null;
        } else {
            // Then delete followers who were killed
            foreach (Follower f in killed)
            {
                yield return StartCoroutine(KillFollower(f));
            }
        }
    }

    private IEnumerator UpdateTiles()
    {
        List<GameElement> spawns = new List<GameElement>(); 
        List<GameElement> despawns = new List<GameElement>();

        foreach (TileObject tile in grid.GetAllTiles())
        {
            tile.TileUpdate(grid.GetOccupant(tile));
        }
        // render changes if any living were moved by tiles
        Debug.Log("grid height: "  + grid.height);

        render.SyncSprites(grid);
        render.MoveSprites();
        yield return new WaitUntil(() => !render.IsInAnimation());
    }

    // Returns the tile Jay will move to, else returns the same position as Jay
    private Direction GetKeyboardDir()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            float moveVertical = Input.GetAxis("Vertical");
            return moveVertical < 0 ? Direction.South : Direction.North;
        }
        else if (Input.GetButtonDown("Horizontal"))
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            return moveHorizontal < 0 ? Direction.West : Direction.East;
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

    /*
    ~Level Loader~:
    */
    public void LoadLevel(Vector2Int JayPos,
                        GameElement.ElementType?[, ] objects,  
                        List<Vector2Int> cars, // misuse of Vector2Int
                        List<(Vector2Int, Vector2Int)> portals){
        int height, width;
        
        width = objects.GetLength(0);
        height = objects.GetLength(1);

        Overworld world = new Overworld(width, height);

        for (int x = 0; x < objects.GetLength(0); x++){
            for (int y = 0; y < objects.GetLength(1); y++){ 
                if (objects[x, y] != null){
                    GameElement.ElementType curr = objects[x, y].Value;
                    switch (curr)
                    {
                        case GameElement.ElementType.Cone:
                            world.SpawnLiving(new Cone(x, y));
                            break;
                        case GameElement.ElementType.Sidewalk:
                            world.SpawnTile(CreateSidewalk(x, y));
                            break;
                        case GameElement.ElementType.Zebra:
                            world.SpawnTile(CreateZebraTile(x, y));
                            break;
                        case GameElement.ElementType.ManHole:
                            world.SpawnTile(CreateManhole(x, y));
                            break;
                        case GameElement.ElementType.Flagpole:
                            world.SpawnTile(CreateFlagpole(x, y));
                            break;
                        default: // Any other ID's don't really make sense
                            Debug.Log("Invalid Level");
                            return;
                    }
                }
            }
        }

        foreach (Vector2Int carPos in cars){
            world.SpawnCar(new Car(carPos.x, carPos.y));
        }

        foreach ((Vector2Int, Vector2Int) portalPair in portals){
            Vector2Int p1 = portalPair.Item1;
            Vector2Int p2 = portalPair.Item2;
            if (objects[p1.x, p1.y] != null || objects[p2.x, p2.y] != null){
                Debug.Log("Invalid Level");
                return; // portals can't overlap with world
            }
            (PressurePlate, PressurePlate) portalTiles = CreatePortals(p1.x, p1.y, p2.x, p2.y);
            world.SpawnTile(portalTiles.Item1);
            world.SpawnTile(portalTiles.Item2);
        }

        if (!world.TileIsEmpty(JayPos)){
            Debug.Log("Invalid Level");
            return; // Jay needs a valid start position
        }

        player = new Jay(JayPos.x, JayPos.y);
        followers = new List<Follower>();
        world.SpawnLiving(player);

        grid = world;

        render = tilemap.GetComponent<OverworldRenderer>();
        render.ScaleCamera(height, width);
        render.SyncSprites(grid);

        moveDisabled = false;
        alive = true;
    }


    /*

    I really don't think this should be in manager class, but uh these functions generate tiles

    */
    Func<TileObject, LivingObject, bool> TileNoop = (TileObject _1, LivingObject _2) => {return true;};

    private PressurePlate CreateSidewalk(int x, int y)
    {
        return new PressurePlate(x, y, null, TileNoop,
            GameElement.ElementType.Sidewalk);
    }

    private PressurePlate CreateManhole(int x, int y)
    {
        Func<TileObject, LivingObject, bool> CopSpawner = (TileObject tile, LivingObject _) => {
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
        Func<TileObject, LivingObject, bool> DecrementTurn = (TileObject _1, LivingObject _2) => {
            grid.turnCount--;
            return true;
        };

        return new PressurePlate(x, y, TileNoop, DecrementTurn, 
            GameElement.ElementType.Zebra);
    }

    private PressurePlate CreateFlagpole(int x, int y)
    {
        Func<TileObject, LivingObject, bool> Win = (TileObject _1, LivingObject _2) => {
            WinLvl();
            return true;
        };

        return new PressurePlate(x, y, TileNoop, Win, 
            GameElement.ElementType.Flagpole);
    }

    private (PressurePlate, PressurePlate) CreatePortals(int x1, int y1, int x2, int y2)
    {
        int lastTurnUsed = -1; // prevents infinite loops
        Vector2Int entrance = new Vector2Int(x1, y1);
        Vector2Int exit = new Vector2Int(x2, y2);

        Func<TileObject, LivingObject, bool> SendToExit = (TileObject _, LivingObject occupant) => {
            if (lastTurnUsed == grid.turnCount || !grid.TileIsEmpty(exit) || occupant != player){
                return true;
            }
            lastTurnUsed = grid.turnCount;
            grid.MoveLiving(occupant, exit);
            return true;
        };

        Func<TileObject, LivingObject, bool> SendToEntrance = (TileObject _, LivingObject occupant) => {
            if (lastTurnUsed == grid.turnCount || !grid.TileIsEmpty(entrance) || occupant != player){
                return true;
            }
            lastTurnUsed = grid.turnCount;
            grid.MoveLiving(occupant, entrance);
            return true;
        };

        return (new PressurePlate(x1, y1, TileNoop, SendToExit, GameElement.ElementType.Zebra),
                new PressurePlate(x2, y2, TileNoop, SendToEntrance, GameElement.ElementType.Zebra));
    }
}
