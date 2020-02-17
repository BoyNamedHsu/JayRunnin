using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public enum Direction {North, East, South, West, None};
    private Overworld grid;
    private ObjectSpawner render;
    public static Vector2Int playerStart = new Vector2Int(0, 0);
    public static List<Vector2Int> directions;
    public static List<GameElement> followers;
    public static List<CarTile> cars;
    public static List<TileObject> tiles;
    public Tilemap tilemap;

    public bool moveDisabled; // disable movements while renderer is playing. This is hacky :(

    // Start is called before the first frame update
    void Awake()
    {
        moveDisabled = false;
        directions = new List<Vector2Int>();
        followers = new List<GameElement> {
            new Jay(playerStart.x, playerStart.y),
            new Follower(1, 0, false),
            new Follower(2, 0, false),
            new Follower(3, 0, false),
            };
        // Initialize the gridworld and spawn a tile object in it
        grid = GameObject.Find("Overworld").GetComponent<Overworld>();

        // Kinda sketch here, to have two separate spawns, find a way to work around this.
        grid.spawnTile(new Vector2Int(2, 2), GameElement.ElementType.Cone);
        tiles = new List<TileObject>
        {
            new ConeTile(2, 2)
        };
        cars = new List<CarTile>
        {
            new CarTile(1, 5)
        };

        render = tilemap.GetComponent<ObjectSpawner>();
        render.SetMap(followers, tiles, cars);

        for (int i = followers.Count - 2; i >= 0; i--) {
            directions.Add(followers[i].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveDisabled)
        {
            return; // don't listen for key inputs while renderer is animating
        }

        Direction moved = Move();
        if (moved != Direction.None)
        {
            StartCoroutine(UpdateGameState(moved));
        }
    }

    // updates state of board and waits for animations to play
    IEnumerator UpdateGameState(Direction moved)
    {
        moveDisabled = true; // players can't input additional controls while we're processing this one

        // Entering this method assumes that move is valid.

        // Test if valid move
        for (int i = 1; i < followers.Count; i++)
        {
            grid.Move(followers[i].position, directions[directions.Count - i], GameElement.ElementType.Follower);
            followers[i].position = directions[directions.Count - i];
        }
        directions.Add(followers[0].position);
        directions.RemoveAt(0);

        /*--Animate those changes--*/
        render.MoveSprites();
        yield return new WaitUntil(() => !render.IsInAnimation());

        List<int> carColumns = new List<int>();
        List<GameElement> killed = new List<GameElement>();

        foreach (CarTile car in cars)
        {
            car.countDown();
            if (car.gone && car.countdown == 0) // countdown hits 0 when iterates count times
            {
                carColumns.Add(car.xPos);
                foreach (GameElement follower in followers)
                {
                    if (follower.position.x == car.xPos)
                        killed.Add(follower);
                }
            }
        }

        /*--Animate those changes--*/
        render.MoveCars(killed, carColumns); // This animation gets hung.... yikes
        yield return new WaitUntil(() => !render.IsInAnimation());

        // if any were killed we need to tighten
        if (killed.Count > 0)
        {
            for (int i = followers.Count - 1; i > 0; i--) // We can't elim Jay!
            {
                if (killed.Contains(followers[i]))
                {
                    for (int j = followers.Count - 1; j > i; j--)
                    {
                        followers[j].position = followers[j - 1].position;
                    }
                    followers.RemoveAt(i);
                }

                /*--Animate those changes--*/
                render.MoveSprites(); // ugh, I don't like de-sync potential here
                yield return new WaitUntil(() => !render.IsInAnimation());
            }
        }

        moveDisabled = false;
        yield return null;
    }

    private 

    // Returns the direction Jay has moved, else returns null if an invalid move occurs
    Direction Move()
    {
        Direction dir = Direction.None;
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2Int temp = followers[0].position;  // Jay's position
        if (Input.GetButtonDown("Vertical"))
        {
            if (moveVertical < 0 && temp.y > 0)  // South
            {
                temp.y--;
                if (grid.TileOccupied(temp)) dir = Direction.South;
            }
            else if (moveVertical > 0 && temp.y < grid.height - 1)  // North
            {
                temp.y++;
                if (grid.TileOccupied(temp)) dir = Direction.North;
            }
        }
        else if (Input.GetButtonDown("Horizontal"))
        {
            if (moveHorizontal < 0 && temp.x > 0)  // West
            {
                temp.x--;
                if (grid.TileOccupied(temp)) dir = Direction.West;
            }
            else if (moveHorizontal > 0 && temp.x < grid.width - 1)  // East
            {
                temp.x++;
                if (grid.TileOccupied(temp)) dir = Direction.East;
            }
        }

        if (grid.Move(followers[0].position, temp, GameElement.ElementType.Jay))
        {
            followers[0].position = temp;
        }
        return dir;
    }
}
