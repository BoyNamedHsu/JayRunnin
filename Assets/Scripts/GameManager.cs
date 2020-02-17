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

        render = tilemap.GetComponent<ObjectSpawner>();
        render.SetMap(followers, tiles);

        for (int i = followers.Count - 2; i >= 0; i--) {
            directions.Add(followers[i].position);
        }
        cars = new List<CarTile> {
            //new CarTile(1, 5),
            //new CarTile(2, 5),
            //new CarTile(3, 5),
            //new CarTile(5, 7)
        };
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

        // Move sprites
        render.MoveSprites();

        // yield return new WaitUntil(() => !render.IsInAnimation());

        bool killed = false;

        for (int i = 0; i < cars.Count; i++)
        {
            CarTile car = cars[i];
            car.countDown();
            if (car.gone && car.countdown == 0) // countdown hits 0 when iterates count times
            {
                killed = true;
                grid.kill(car.yPos);

                for (int j = 0; j < followers.Count; j++)
                {
                    if (followers[j].position.y == car.yPos)
                    {
                        followers.RemoveAt(j);
                    }
                }
            }
        }

        if (killed)
        {
            for (int i = 1; i < followers.Count; i++)
            {
                //print(followers[i].position + " yee " + directions[directions.Count - i - 1]);
                grid.Move(followers[i].position, directions[directions.Count - i - 1], GameElement.ElementType.Follower);
                followers[i].position = directions[directions.Count - i - 1];
            }
            while (directions.Count != followers.Count - 1)
                directions.RemoveAt(0);
        }

        moveDisabled = false;

        yield return null;
    }

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

    /*// moves part of a list of people, starting with the given person in line
    private void MoveList(Direction dir, List<Living> people, int startPerson)
    {
        Vector2Int dest = getDest(dir, people[startPerson].position); // position must be a Vector2Int, change living obj
        for (int i = startPerson; i < people.Count; i++)
        {
            Living curr = people[i];
            Vector2Int src = curr.position; // current position of living obj

            GameObject currSprite = spawnedSprites[curr];
            destinations[currSprite] = convertCellLoc(dest); // move to destination
            dest = src; // next obj moves to this object's previous position
        }

        ObjectSpawner.MoveSprites();
    }*/


}
