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
    public static List<Living> followers;
    public static List<CarTile> cars;
    // Add enum for object and elements


    // Start is called before the first frame update
    void Awake()
    {
        directions = new List<Vector2Int>();
        followers = new List<Living> {
            new Jay(playerStart.x, playerStart.y),
            new Follower(1, 0, false),
            new Follower(2, 0, false),
            new Follower(3, 0, false),
            };
        grid = GameObject.Find("Overworld").GetComponent<Overworld>();
        render = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
        render.setMap(followers);
        for (int i = followers.Count - 2; i >= 0; i--) {
            directions.Add(followers[i].position);
        }
        cars = new List<CarTile> {
            new CarTile(1, 5),
            new CarTile(2, 5),
            new CarTile(3, 5),
            new CarTile(5, 7)
        };
    }

    // Update is called once per frame
    void Update()
    {
        Direction moved = Move();
        if (moved != Direction.None)
        {
            // Entering this branch assumes that move is valid.
            
            // Test if valid move
            for (int i = 1; i < followers.Count; i++)
            {
                /* (int x, int y) next = followers[i].Move(directions[directions.Count - i - 1]);*/
                // don't need to subtract index by 1 because start i at 1
                grid.Move(followers[i].position, directions[directions.Count - i], GameElement.ElementType.Follower);
                followers[i].position = directions[directions.Count - i];

            }
            directions.Add(followers[0].position);
            directions.RemoveAt(0);
            render.MoveFullChain(moved, followers);

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
                    print(followers[i].position + " yee " + directions[directions.Count - i - 1]);
                    grid.Move(followers[i].position, directions[directions.Count - i - 1], GameElement.ElementType.Follower);
                    followers[i].position = directions[directions.Count - i - 1];
                }
                while (directions.Count != followers.Count - 1)
                    directions.RemoveAt(0);
            }

        }
/*        print("Directions " + string.Join(",", directions));
        print("Followers " + string.Join(",", followers));*/
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
            if (moveVertical < 0 && temp.x < grid.height - 1)  // South
            {
                temp.x = temp.x + 1;
                if (grid.TileOccupied(temp)) dir = Direction.South;
            }
            else if (moveVertical > 0 && temp.x > 0)  // North
            {
                temp.x = temp.x - 1;
                if (grid.TileOccupied(temp)) dir = Direction.North;
            }
        }
        else if (Input.GetButtonDown("Horizontal"))
        {
            if (moveHorizontal > 0 && temp.y < grid.width - 1)  // East
            {
                temp.y = temp.y + 1;
                if (grid.TileOccupied(temp)) dir = Direction.East;
            }
            else if (moveHorizontal < 0 && temp.y > 0)  // West
            {
                temp.y = temp.y - 1;
                if (grid.TileOccupied(temp)) dir = Direction.West;
            }
        }

        if (grid.Move(followers[0].position, temp, GameElement.ElementType.Jay))
        {
            followers[0].position = temp;
        }
        return dir;
    }


}
