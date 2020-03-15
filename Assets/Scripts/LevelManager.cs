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
    public SoundPlayer audio;
    public LevelChanger sceneFader;

    //Level Complete panel
    private Vector2Int starRange;
    private int three; // anything lower than this is 2 stars
    private int two; // anything lower than this is 1 sta
    public LevelEndPanel elPanel;
    private bool GameOver;

    private bool moveDisabled; // disable movements while renderer is playing
    private bool alive; // checks if the player is alive

    private int copsGoal; // the amount of cops to be defeated in this level
    private int copsDefeated; // the amount of cops we've defeated so far
    private int followersDead;
    private int numFollowers;
    private int followersUp;

    // collisions
    public enum Direction {North, East, South, West, None};
    private Overworld grid;
    private List<Overworld> prevStates;
    public List<Direction> playerDirections;

    // Logging fields
    public CapstoneLogger logger;
    private int moveCount;
    private string movePath; // A string representing the directions moved before retry/win
    private string startPos; // Where Jay begins for the purpose of knowing where movePath begins.



    void Awake()
    {
        GameOver = false;
        moveDisabled = true;

        // Below we log level start and initialize fields for logging
        moveCount = 0;
        movePath = "";
        startPos = "";
        logger = LoggerController.LOGGER;
        playerDirections = new List<Direction>();
        StartCoroutine(logger.LogLevelStart(LevelSelector.levelChosen, ""));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameOver)
        {
            return; //for endlevelpanel interactions
        }
        // return to level select
        if (Input.GetKeyDown(KeyCode.Escape)){
            logger.LogLevelEnd((LoggerController.numRestarts + LoggerController.deathCount) + " E");
            SceneManager.LoadScene("World_1");
            return;
        }

        if (moveDisabled)
        {
            return; // don't listen for key inputs while renderer is animating
        }

        if (Input.GetKeyDown("r")){
            RestartLvl();
            return;
        }

        Direction dir = GetKeyboardDir();
        Vector2Int newPos = ApplyDir(grid.player.position, dir);

        if (dir == Direction.None){
            return;
        }
        if (!grid.TileIsEmpty(newPos)){
            return;
        }

        moveCount++;
        movePath += ConvertDirToStr(dir);
       

        playerDirections.Insert(0, dir);
        //print(string.Join(",", playerDirections));

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

        // check if player is unwinnable state
        if (grid.cars.Count == 0 && copsDefeated < copsGoal ||
            // if the player is surrouneded on all sides
            (!grid.TileIsEmpty(ApplyDir(grid.player.position, Direction.North)) && 
            !grid.TileIsEmpty(ApplyDir(grid.player.position, Direction.South)) &&
            !grid.TileIsEmpty(ApplyDir(grid.player.position, Direction.East)) &&
            !grid.TileIsEmpty(ApplyDir(grid.player.position, Direction.West))
            )){
            render.SuggestRestart();
        }
        TileObject tileOccupied = grid.GetTile(grid.player.position);
        bool onFlagpole = grid.IsElement(tileOccupied, GameElement.ElementType.Flagpole);
        if (onFlagpole)
        {
            if (copsDefeated >= copsGoal)
            {
                WinLvl();
            }
            else
            {
                Debug.Log("More cops to kill still");
            }
            yield return null;
        }
        yield return null;
    }

    private void FinishLvl()
    {
        LevelSelector.currentRetries = 0;
        grid.Clear();
        render.SyncSprites(grid, copsGoal, copsDefeated);
        Destroy(GameObject.Find("CanvasUI"));
        alive = false;

    }



    // On click for next level button (should be moved elsewhere?)
    public void NextLevel()
    {

        if (LevelSelector.levelChosen < Levels.LAST_LEVEL)
            FinishLvl();

        if (LevelSelector.levelChosen > Levels.LAST_LEVEL)
        {
            Debug.Log("end");
            sceneFader.FadeToLevel("Ending");
        }
        else
        {
            SceneManager.LoadScene("Level");
        }
    }

    // On click for retry level button
    public void RetryLevel()
    {
        if (LevelSelector.levelChosen < Levels.LAST_LEVEL)
            FinishLvl();
        LevelSelector.levelChosen--;
        SceneManager.LoadScene("Level");
    }

    // On click for exit to level select button
    public void ExitToLevelSelect()
    {
        SceneManager.LoadScene("World_1");
    }

    // Might be worth considering a coroutine instead of coupling this with flags but this works
    private void WinLvl()
    {
        moveDisabled = true;
        int panel = PlayerPrefs.GetInt("levelEndPanel") * 10;
        Debug.Log("You won!");

        logger.LogLevelAction(panel + 2, "" + moveCount); // 2 is for move count on win
        logger.LogLevelAction(panel + 8, "" + LoggerController.numRestarts); // Log number of restarts on this level before win
        Debug.Log(LoggerController.numRestarts);
        LevelSelector.levelChosen++;
        Debug.Log("next Level : " + LevelSelector.levelChosen);

        if (LevelSelector.levelChosen > Unlocker.GetHighestUnlockedLevel())
            Unlocker.Unlocked();

        print("NEXT LEVEL: " + LevelSelector.levelChosen);
        LoggerController.LOGGER.LogLevelAction(6, "" + LevelSelector.levelChosen); // Log level beat
        logger.LogLevelAction(panel + 4, LoggerController.deathCount + ""); // death count

        logger.LogLevelAction(panel + 9, startPos + " " + movePath + " W"); // Log path of player on win

        logger.LogLevelEnd((LoggerController.numRestarts + LoggerController.deathCount) + " W"); // Log end of level || Details: total retries including restarts and deaths
        LoggerController.ResetFields();

        int curRetries = LevelSelector.currentRetries;
        int lvlChosen = LevelSelector.levelChosen;
        Debug.Log(curRetries);
        // If retries this time surpasses your max retries on this level
        if ( curRetries > LevelSelector.maxRetries[lvlChosen - 1])
        {
            LevelSelector.maxRetries[lvlChosen - 1] = curRetries;
        }

        if (PlayerPrefs.GetInt("levelEndPanel") == 1 && LevelSelector.levelChosen != Levels.LAST_LEVEL + 1 && LevelSelector.levelChosen != 2)
        {
            GameOver = true;
            //NextLevel();
            elPanel.GetComponent<endLevelPanel>().showPanel(starRange.x, starRange.y, numFollowers, followersDead, followersUp);
        } else
        {
            NextLevel();
        }
        
    }

    private void LoseLvl()
    {
        int panel = PlayerPrefs.GetInt("levelEndPanel") * 10;
        Debug.Log("You died");
        LoggerController.deathCount++;
        LevelSelector.currentRetries++;
        logger.LogLevelAction(1 + panel, "" + moveCount); // 1 is for move count on losses
        logger.LogLevelAction(9 + panel, startPos + " " + movePath + " L"); // Log path of player on loss

        FinishLvl();
        SceneManager.LoadScene("Level");
    }

    // This is kinda bad but I need to differentiate restart with loss so I created a method thats basically the same as lose lvl
    private void RestartLvl()
    {
        LevelSelector.currentRetries++;
        int panel = PlayerPrefs.GetInt("levelEndPanel") * 10;
        LoggerController.numRestarts++;
        logger.LogLevelAction(7, "(" + grid.player.position.x + ", " + grid.player.position.y + ")"); // Logs where the user restarts
        logger.LogLevelAction(9, startPos + " " + (movePath.Equals("") ? "O" : movePath) + " R"); // Log path of player on restart

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
        Vector2Int oldPos = grid.player.position;
        grid.MoveLiving(grid.player, newPos);
       
        // Ugly, but makes the grid animations much smoother
        PlaySnappyAnimations(grid.GetTile(newPos));

        //audio.PlaySound("woosh");
        // Changing warning signs to stop signs if Jay is on zebra tile



        yield return StartCoroutine(MoveChain(oldPos, 0));
    }

    // Plays animations on tileStepped
    private void PlaySnappyAnimations(TileObject tileStepped)
    {
        if (grid.IsElement(tileStepped, GameElement.ElementType.FanHole))
        {
            audio.PlaySound("close");
            render.PlayAnimation(tileStepped, "New Animation");
        }
        else if (grid.IsElement(tileStepped, GameElement.ElementType.ManHole))
        {
            audio.PlaySound("close");
            Transform transMan = render.GetGameObject(tileStepped).transform;
            transMan.GetChild(1).GetComponent<Animator>().Play("ManholeTopClose");
            transMan.GetChild(2).gameObject.SetActive(false);
        }

    }

    // Moves a portion chain of followers to the given coords, starting from index *head*
    private IEnumerator MoveChain(Vector2Int newPos, int head)
    {
        //print("NEW MOVE");
        Vector2Int oldPos;
        for (int i = head; i < grid.followers.Count; i++)
        {
            oldPos = grid.followers[i].position;
            grid.MoveLiving(grid.followers[i], newPos);
            newPos = oldPos;
            
        }
        render.MoveSprites();
        render.UpdateSpriteDirection(playerDirections, grid.followers, head);
        yield return new WaitUntil(() => !render.IsInAnimation());


        // then check if any tiles were triggered by movement
        yield return StartCoroutine(UpdateTiles());
    }

    private IEnumerator KillFollower(Follower f) // Deletes follower *i* from the chain
    {
        int i = grid.followers.IndexOf(f); // if this is slow, we can pass in int directly 

        Vector2Int dest = grid.followers[i].position;
        grid.DeleteLiving(grid.followers[i]);
        grid.followers.RemoveAt(i);
        render.SyncSprites(grid, copsGoal, copsDefeated); // sync, since we just deleted it

        yield return StartCoroutine(MoveChain(dest, i));
    }

    // Animates cars running over living objects, populates killed with runover living
    private IEnumerator SendCars()
    {
        List<LivingObject> killed = new List<LivingObject>();
        List<int> carColumns = new List<int>();

        List<Car> cars = grid.cars;

        for (int i = cars.Count - 1; i >= 0; i--){
            Car car = cars[i];

            if (car.triggerTurn == grid.turnCount)
            {
                carColumns.Add(car.xPos);
                // kinda jank way of doing this... can be cleaned up
                foreach (Follower f in grid.followers)
                {
                    if (f.position.x == car.xPos){
                        killed.Add(f);
                        if (f.eid == GameElement.ElementType.Cop)
                        {
                            copsDefeated++;
                        }
                        else
                        {
                            followersDead++;
                        }
                    }
                }
                if (grid.player.position.x == car.xPos)
                    killed.Add(grid.player);
                cars.RemoveAt(i); // then consume that car
            }
        }

        /*--Animate those changes--*/
        render.MoveCars(killed, carColumns, grid);
        yield return new WaitUntil(() => !render.IsInAnimation());

        // Check if the car killed the player
        if (killed.Contains(grid.player)){
            LoseLvl();
            yield return null;
        } else if (PlayerPrefs.GetInt("levelEndPanel") == 0) {
            foreach (Follower f in killed)
            {
                if (f.eid == GameElement.ElementType.Fan){
                    LoseLvl();
                    yield return null;
                }

                // Then delete followers who were killed
                yield return StartCoroutine(KillFollower(f));
            }

        } else
        {
            foreach (Follower f in killed)
            {
                // Then delete followers who were killed
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

        // Changing warning signs to stop signs if Jay is on zebra tile
        TileObject tileOccupied = grid.GetTile(grid.player.position);
        bool onZebra = grid.IsElement(tileOccupied, GameElement.ElementType.Zebra);
        render.ChangeCarWarningSprite(onZebra);


        // render changes if any living were moved by tiles
        render.SyncSprites(grid, copsGoal, copsDefeated);
        render.MoveSprites();
        yield return new WaitUntil(() => !render.IsInAnimation());
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
    ~Level Loader~:
    */
    public void LoadLevel(Vector2Int JayPos,
                        GameElement.ElementType?[, ] objects,  
                        List<Vector2Int> cars, // misuse of Vector2Int
                        List<(Vector2Int, Vector2Int)> portals,
                        int copsGoal, Vector2Int star){
        int height, width;
        
        width = objects.GetLength(0);
        height = objects.GetLength(1);

        height++; // make rooom for an invisible wall on the top

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
                            numFollowers++;
                            break;
                        case GameElement.ElementType.Flagpole:
                            world.SpawnTile(new TileObject(x, y, GameElement.ElementType.Flagpole));
                            break;
                        case GameElement.ElementType.ConeWalk:
                            world.SpawnTile(CreateSidewalk(x, y));
                            world.SpawnLiving(new Cone(x, y));
                            break;
                        default: // Any other ID's don't really make sense
                            Debug.Log("Invalid Level");
                            return;
                    }
                }
            }
        }

        world.SpawnTile(CreateSidewalk(0, height - 1));
        world.SpawnTile(CreateSidewalk(width - 1, height - 1));
        for (int x = 0; x < objects.GetLength(0); x++){
            world.SpawnLiving(new InvisibleWall(x, height - 1)); 
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

        world.player = new Jay(JayPos.x, JayPos.y);

        startPos = JayPos.x + " " + JayPos.y; // Store start position of Jay for purpose of logging

        world.followers = new List<Follower>();
        world.SpawnLiving(world.player);

        grid = world;
        this.copsGoal = copsGoal;
        this.copsDefeated = 0;

        prevStates = new List<Overworld>();
        prevStates.Add(grid);

        render = tilemap.GetComponent<OverworldRenderer>();
        render.ScaleCamera(height, width);
        render.SyncSprites(grid, copsGoal, copsDefeated);

        moveDisabled = false;
        alive = true;

        starRange = star;
    }


    /*

    I really don't think this should be in manager class, but uh these functions generate tiles

    */
    Func<TileObject, LivingObject, bool> TileNoop = (TileObject _1, LivingObject _2) => {return true;};

    private PressurePlate CreateSidewalk(int x, int y)
    {
        return new PressurePlate(x, y, TileNoop, TileNoop,
            GameElement.ElementType.Sidewalk);
    }

    private PressurePlate CreateManhole(int x, int y)
    {

        Func<TileObject, LivingObject, bool> CopSpawner = (TileObject tile, LivingObject _) => {
            Follower cop = new Cop(tile.position.x, tile.position.y);
            grid.DeleteTile(tile);
            grid.SpawnLiving(cop);
            grid.followers.Add(cop);
            return true;
        };

        return new PressurePlate(x, y, CopSpawner, TileNoop, 
            GameElement.ElementType.ManHole);
    }

    private PressurePlate CreateFanHole(int x, int y)
    {

        Func<TileObject, LivingObject, bool> FanSpanwer = (TileObject tile, LivingObject _) => {
            Follower fan = new Fan(tile.position.x, tile.position.y);
            grid.DeleteTile(tile);
            grid.SpawnLiving(fan);
            grid.followers.Add(fan);
            followersUp++;
            return true;
        };

        return new PressurePlate(x, y, FanSpanwer, TileNoop, 
            GameElement.ElementType.FanHole);
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

    private (PressurePlate, PressurePlate) CreatePortals(int x1, int y1, int x2, int y2)
    {
        int lastTurnUsed = -1; // prevents infinite loops
        Vector2Int entrance = new Vector2Int(x1, y1);
        Vector2Int exit = new Vector2Int(x2, y2);

        Func<TileObject, LivingObject, bool> SendToExit = (TileObject _, LivingObject occupant) => {
            if (lastTurnUsed == grid.turnCount || !grid.TileIsEmpty(exit) || occupant != grid.player){
                return true;
            }
            lastTurnUsed = grid.turnCount;
            grid.MoveLiving(occupant, exit);
            audio.PlaySound("woosh");
            return true;
        };

        Func<TileObject, LivingObject, bool> SendToEntrance = (TileObject _, LivingObject occupant) => {
            if (lastTurnUsed == grid.turnCount || !grid.TileIsEmpty(entrance) || occupant != grid.player){
                return true;
            }
            lastTurnUsed = grid.turnCount;
            grid.MoveLiving(occupant, entrance);
            audio.PlaySound("woosh");
            return true;
        };

        return (new PressurePlate(x1, y1, TileNoop, SendToExit, GameElement.ElementType.Portal),
                new PressurePlate(x2, y2, TileNoop, SendToEntrance, GameElement.ElementType.Portal));
    }

    private string ConvertDirToStr(Direction dir)
    {
        switch (dir)
        {
            case Direction.South:
                return "S";
            case Direction.North:
                return "N";
            case Direction.West:
                return "W";
            case Direction.East:
                return "E";
            case Direction.None:
                return "O";
            default: // This should never occur
                return "Z";
        }
    }
}
