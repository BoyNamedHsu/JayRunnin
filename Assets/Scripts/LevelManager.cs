using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using cse481.logging;

public class LevelManager : MonoBehaviour
{
    // rendering:
    public Tilemap tilemap;
    private OverworldRenderer render;
    public LevelChanger sceneFader;
    private LevelDescrip lvl; // For quickly reloading level

    private bool moveDisabled; // disable movements while renderer is playing

    // collisions
    public enum Direction { North, East, South, West, None };
    private Overworld grid;
    public List<Direction> playerDirections;

    void Awake() { moveDisabled = true; }

    // Update is called once per frame
    void Update()
    {
        if (moveDisabled)
            return; // don't listen for key inputs while renderer is animating

        // return to level select
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("World_1");
            return;
        }

        if (Input.GetKeyDown("r"))
        {
            RestartLvl(); // Losing is just the same as restarting lol
            return;
        }

        if (Input.GetKeyDown("u")) // Undos the last move
        {
            List<Direction> toReplay = new List<Direction>();
            for (int i = 0; i < playerDirections.Count - 1; i++)
            {
                toReplay.Add(playerDirections[i]);
            }

            RestartLvl(); // Restart the level
            foreach (Direction d in toReplay)
            {
                UpdateGameStateM(d);
            } // Then play back the tape of moves we've done
            render.SyncSpritesHard(grid); // Render the end-product!
            return;
        }

        if (grid.IsStuck())
            return; // If the game detects the player is stuck, don't let them move

        Direction dir = GetKeyboardDir();
        if (dir == Direction.None) return;
        Vector2Int newPos = ApplyDir(grid.player.position, dir);
        if (!grid.TileIsEmpty(newPos) || grid.IsStuck()) return;

        StartCoroutine(UpdateGameAnimated(dir));
    }

    // Use these methods to avoid desyncing the model and the renderer
    private void MoveLiving(LivingObject obj, Vector2Int newPos)
    {
        render.MoveLiving(obj, newPos);
        grid.MoveLiving(obj, newPos);
    }

    private void DeleteLiving(LivingObject obj)
    {
        render.DeleteObject(obj);
        grid.DeleteLiving(obj);
    }

    private void SpawnLiving(LivingObject obj)
    {
        render.SpawnObject(obj);
        grid.SpawnLiving(obj);
    }

    private IEnumerator UpdateGameAnimated(Direction dir)
    {
        moveDisabled = true;
        yield return StartCoroutine(render.PlayAnimations()); // *Should* do nothing


        Vector2Int newPos = ApplyDir(grid.player.position, dir);
        grid.turnCount++;
        playerDirections.Add(dir); // For the sake of undos!

        // Move Jay
        MoveJayM(newPos); // then move the chain/animate
        yield return StartCoroutine(render.PlayAnimations());

        // Check for cars/send them in
        List<Follower> killed = SendCarsM();
        yield return StartCoroutine(render.PlayAnimations());

        foreach (Follower f in killed)
        {
            if (f.eid == GameElement.ElementType.Fan)
                grid.alive = false;
            if (f.eid == GameElement.ElementType.Cop)
                grid.copsDefeated++;
            KillFollowerM(f);
            yield return StartCoroutine(render.PlayAnimations());
        }

        if (grid.alive)
        {
            TileObject tileOccupied = grid.GetTile(grid.player.position);
            bool onFlagpole = grid.IsElement(tileOccupied, GameElement.ElementType.Flagpole);
            if (onFlagpole && grid.CopsKilled())
                WinLvl();
        }
        render.SyncSprites(grid); // Produces a wack snappping animation
        moveDisabled = false;
        yield return null;
    }

    // Pre: The given move is valid
    private void UpdateGameStateM(Direction dir)
    {
        Vector2Int newPos = ApplyDir(grid.player.position, dir);
        grid.turnCount++;
        playerDirections.Add(dir); // For the sake of undos!

        // Move Jay
        MoveJayM(newPos); // then move the chain/animate

        // Check for cars/send them in
        List<Follower> killed = SendCarsM();
        // Could render this

        foreach (Follower f in killed)
        {
            if (f.eid == GameElement.ElementType.Fan || f.eid == GameElement.ElementType.Jay)
                grid.alive = false;
            if (f.eid == GameElement.ElementType.Cop)
                grid.copsDefeated++;
            KillFollowerM(f);
        }

        TileObject tileOccupied = grid.GetTile(grid.player.position);
        bool onFlagpole = grid.IsElement(tileOccupied, GameElement.ElementType.Flagpole);
        if (onFlagpole && grid.CopsKilled())
            WinLvl();
    }

    private void MoveJayM(Vector2Int newPos)
    {
        Vector2Int oldPos = grid.player.position;
        MoveLiving(grid.player, newPos);
        MoveChainM(oldPos, 0);
    }

    private void MoveChainM(Vector2Int newPos, int head)
    {
        Vector2Int oldPos;
        for (int i = head; i < grid.followers.Count; i++)
        {
            oldPos = grid.followers[i].position;
            MoveLiving(grid.followers[i], newPos);
            newPos = oldPos;
        }

        // then check if any tiles were triggered by movement
        UpdateTilesM();
    }

    private void KillFollowerM(Follower f) // Deletes follower *i* from the chain
    {
        int i = grid.followers.IndexOf(f);
        Vector2Int dest = grid.followers[i].position;
        DeleteLiving(grid.followers[i]);
        grid.followers.RemoveAt(i);
        MoveChainM(dest, i);
    }

    private void UpdateTilesM()
    {
        foreach (TileObject tile in grid.GetAllTiles())
        {
            tile.TileUpdate(grid.GetOccupant(tile));
        }
        // Changing warning signs to stop signs if Jay is on zebra tile
        TileObject tileOccupied = grid.GetTile(grid.player.position);
        bool onZebra = grid.IsElement(tileOccupied, GameElement.ElementType.Zebra);
    }

    private List<Follower> SendCarsM()
    {
        List<Follower> allKills = new List<Follower>();
        List<int> carColumns = new List<int>();
        List<Car> cars = grid.cars;

        for (int i = cars.Count - 1; i >= 0; i--)
        {
            Car car = cars[i];
            if (car.triggerTurn == grid.turnCount)
            {
                List<LivingObject> carKills = new List<LivingObject>();
                carColumns.Add(car.xPos);
                foreach (Follower f in grid.followers)
                {
                    if (f.position.x == car.xPos)
                    {
                        allKills.Add(f);
                        carKills.Add(f);
                    }
                }
                if (grid.player.position.x == car.xPos)
                {
                    carKills.Add(grid.player);
                    grid.alive = false; // The player has been killed :(
                }
                render.SendCar(car, grid, carKills);
            }
        }
        return allKills;
    }

    private void FinishLvl()
    {
        LevelSelector.currentRetries = 0;
        grid.Clear();
        render.SyncSprites(grid);
        Destroy(GameObject.Find("CanvasUI"));
    }

    public void NextLevel()
    {
        if (LevelSelector.levelChosen < Levels.LAST_LEVEL)
            FinishLvl();

        if (LevelSelector.levelChosen > Levels.LAST_LEVEL)
        {
            sceneFader.FadeToLevel("Ending");
        }
        else
        {
            SceneManager.LoadScene("Level");
        }
    }

    private void WinLvl()
    {
        NextLevel();
    }

    // Returns the tile Jay will move to, else returns the same position as Jay
    private Direction GetKeyboardDir()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            return Direction.North;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            return Direction.South;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            return Direction.West;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            return Direction.East;
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
    I really don't think this should be in manager class, but uh these functions generate tiles
    */
    Func<TileObject, LivingObject, bool> TileNoop = (TileObject _1, LivingObject _2) => { return true; };

    private PressurePlate CreateSidewalk(int x, int y)
    {
        return new PressurePlate(x, y, TileNoop, TileNoop,
            GameElement.ElementType.Sidewalk);
    }

    private PressurePlate CreateManhole(int x, int y)
    {
        Cop cop = new Cop(x, y);
        return CreateHoleSpawner(cop, GameElement.ElementType.ManHole, x, y);
    }

    private PressurePlate CreateFanHole(int x, int y)
    {
        Fan fan = new Fan(x, y);
        return CreateHoleSpawner(fan, GameElement.ElementType.FanHole, x, y);
    }

    private PressurePlate CreateHoleSpawner(Follower spawned, GameElement.ElementType icon, int x, int y)
    {
        Func<TileObject, LivingObject, bool> FanSpanwer = (TileObject tile, LivingObject _) =>
        {
            grid.DeleteTile(tile);
            render.DeleteObject(tile);
            SpawnLiving(spawned);
            grid.followers.Add(spawned);
            return true;
        };

        Func<TileObject, LivingObject, bool> CloseHole = (TileObject tile, LivingObject _) =>
        {
            render.PlayManholeClose(tile);
            return true;
        };

        return new PressurePlate(x, y, FanSpanwer, CloseHole, icon);
    }

    private PressurePlate CreateZebraTile(int x, int y)
    {
        Func<TileObject, LivingObject, bool> DecrementTurn = (TileObject _1, LivingObject _2) =>
        {
            grid.turnCount--;
            return true;
        };

        return new PressurePlate(x, y, TileNoop, DecrementTurn,
            GameElement.ElementType.Zebra);
    }

    private (PressurePlate, PressurePlate) CreatePortals(int x1, int y1, int x2, int y2)
    {
        int lastTurnUsed = -1; // prevents infinite loops
        Vector2Int entrance = new Vector2Int(x1, y1);
        Vector2Int exit = new Vector2Int(x2, y2);

        Func<TileObject, LivingObject, bool> SendToExit = (TileObject _, LivingObject occupant) =>
        {
            if (lastTurnUsed == grid.turnCount || !grid.TileIsEmpty(exit) || occupant != grid.player)
            {
                return true;
            }
            lastTurnUsed = grid.turnCount;
            MoveLiving(occupant, exit);
            return true;
        };

        Func<TileObject, LivingObject, bool> SendToEntrance = (TileObject _, LivingObject occupant) =>
        {
            if (lastTurnUsed == grid.turnCount || !grid.TileIsEmpty(entrance) || occupant != grid.player)
            {
                return true;
            }
            lastTurnUsed = grid.turnCount;
            MoveLiving(occupant, entrance);
            return true;
        };

        return (new PressurePlate(x1, y1, TileNoop, SendToExit, GameElement.ElementType.Portal),
                new PressurePlate(x2, y2, TileNoop, SendToEntrance, GameElement.ElementType.Portal));
    }

    /*
    ~Level Loader~:
    */
    private void RestartLvl()
    {
        moveDisabled = false;
        playerDirections = new List<Direction>();

        grid = ConstructLevel(lvl); // Reconstruct the level
        render.SyncSprites(grid);
    }
    public void LoadLevel(Vector2Int JayPos,
                        GameElement.ElementType?[,] objects,
                        List<Vector2Int> cars, // misuse of Vector2Int
                        List<(Vector2Int, Vector2Int)> portals,
                        int copsGoal, Vector2Int star)
    {
        this.lvl = new LevelDescrip(JayPos, objects, cars, portals, copsGoal, star);

        int height, width;
        width = objects.GetLength(0);
        height = objects.GetLength(1);
        height++; // make rooom for an invisible wall on the top

        render = tilemap.GetComponent<OverworldRenderer>();
        render.ScaleCamera(height, width);
        RestartLvl();
    }

    private Overworld ConstructLevel(LevelDescrip lvl)
    {
        int height, width;
        width = lvl.objects.GetLength(0);
        height = lvl.objects.GetLength(1) + 1;

        Overworld world = new Overworld(width, height);

        for (int x = 0; x < lvl.objects.GetLength(0); x++)
        {
            for (int y = 0; y < lvl.objects.GetLength(1); y++)
            {
                if (lvl.objects[x, y] != null)
                {
                    GameElement.ElementType curr = lvl.objects[x, y].Value;
                    switch (curr)
                    {
                        case GameElement.ElementType.Cone:
                            world.SpawnLiving(new Cone(x, y));
                            break;
                        case GameElement.ElementType.InvisibleWall:
                            world.SpawnLiving(new InvisibleWall(x, y));
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
                        case GameElement.ElementType.FanHole:
                            world.SpawnTile(CreateFanHole(x, y));
                            break;
                        case GameElement.ElementType.Flagpole:
                            world.SpawnTile(new TileObject(x, y, GameElement.ElementType.Flagpole));
                            break;
                        case GameElement.ElementType.ConeWalk:
                            world.SpawnTile(CreateSidewalk(x, y));
                            world.SpawnLiving(new Cone(x, y));
                            break;
                        default: // Any other ID's don't really make sense
                            return null;
                    }
                }
            }
        }

        world.SpawnTile(CreateSidewalk(0, height - 1));
        world.SpawnTile(CreateSidewalk(width - 1, height - 1));
        for (int x = 0; x < lvl.objects.GetLength(0); x++)
        {
            world.SpawnLiving(new InvisibleWall(x, height - 1));
        }

        foreach (Vector2Int carPos in lvl.cars)
        {
            world.SpawnCar(new Car(carPos.x, carPos.y));
        }

        foreach ((Vector2Int, Vector2Int) portalPair in lvl.portals)
        {
            Vector2Int p1 = portalPair.Item1;
            Vector2Int p2 = portalPair.Item2;
            if (lvl.objects[p1.x, p1.y] != null || lvl.objects[p2.x, p2.y] != null)
            {
                return null; // portals can't overlap with world
            }
            (PressurePlate, PressurePlate) portalTiles = CreatePortals(p1.x, p1.y, p2.x, p2.y);
            world.SpawnTile(portalTiles.Item1);
            world.SpawnTile(portalTiles.Item2);
        }

        if (!world.TileIsEmpty(lvl.JayPos))
        {
            return null; // Jay needs a valid start position
        }

        world.player = new Jay(lvl.JayPos.x, lvl.JayPos.y);

        world.followers = new List<Follower>();
        world.SpawnLiving(world.player);

        world.copsGoal = lvl.copsGoal;
        world.copsDefeated = 0;

        return world;
    }

    // Awful temp solution
    private class LevelDescrip
    {
        public Vector2Int JayPos;
        public GameElement.ElementType?[,] objects;
        public List<Vector2Int> cars;
        public List<(Vector2Int, Vector2Int)> portals;
        public int copsGoal;
        public Vector2Int star;

        public LevelDescrip(Vector2Int JayPos,
                        GameElement.ElementType?[,] objects,
                        List<Vector2Int> cars, // misuse of Vector2Int
                        List<(Vector2Int, Vector2Int)> portals,
                        int copsGoal, Vector2Int star)
        {
            this.JayPos = JayPos;
            this.objects = objects;
            this.cars = cars;
            this.portals = portals;
            this.copsGoal = copsGoal;
            this.star = star;
        }
    }
}
