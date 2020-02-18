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
        };

        // Initialize the gridworld and spawn a tile object in it
        cars = new List<Car>();
        cars.Add(new Car(2, 5)); // one car

        grid = new Overworld(height, width);

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

        Vector2Int newPos = Move();
        if (newPos.x == player.position.x && newPos.y == player.position.y){
            return;
        }
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

        // move jay
        Vector2Int oldPos = player.position;
        grid.MoveLiving(player, newPos);

        // and each follower
        newPos = oldPos;
        foreach (Follower f in followers)
        {
            oldPos = f.position;
            grid.MoveLiving(f, newPos);
            newPos = oldPos;
        }

        /*--Animate those changes--*/
        render.MoveSprites();
        yield return new WaitUntil(() => !render.IsInAnimation());
        
        List<int> carColumns = new List<int>();
        List<LivingObject> killed = new List<LivingObject>();

        foreach (Car car in cars)
        {
            if (car.countDown()) // if countdown returns true
            {
                carColumns.Add(car.xPos);
                foreach (Follower follower in followers)
                {
                    if (follower.position.x == car.xPos)
                        killed.Add(follower);
                }
            }
        }

        /*--Animate those changes--*/
        render.MoveCars(killed, carColumns);
        yield return new WaitUntil(() => !render.IsInAnimation());
        

        // if any were killed we need to tighten
        if (killed.Count > 0)
        {
            for (int i = followers.Count - 1; i >= 0; i--)
            {
                if (killed.Contains(followers[i]))
                {
                    newPos = followers[i].position;
                    grid.DeleteLiving(followers[i]);

                    for (int j = i + 1; j < followers.Count; j++)
                    {
                        oldPos = followers[j].position;
                        grid.MoveLiving(followers[j], newPos);
                        newPos = oldPos;
                    }
                    followers.RemoveAt(i);
                }
                
                // Animate those changes
                render.MoveSprites(); // ugh, I don't like de-sync potential here
                yield return new WaitUntil(() => !render.IsInAnimation());
            }
        }

        moveDisabled = false;
        yield return null;
    }

    // Returns the tile Jay will move to, else returns the same position as Jay
    Vector2Int Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Jay's position
        Vector2Int newPos = new Vector2Int(player.position.x, player.position.y);
        
        if (Input.GetButtonDown("Vertical"))
        {
            if (moveVertical < 0 && newPos.y > 0)  // South
            {
                newPos.y--;
            }
            else if (moveVertical > 0 && newPos.y < height - 1)  // North
            {
                newPos.y++;
            }
        }
        else if (Input.GetButtonDown("Horizontal"))
        {
            if (moveHorizontal < 0 && newPos.x > 0)  // West
            {
                newPos.x--;
            }
            else if (moveHorizontal > 0 && newPos.x < width - 1)  // East
            {
                newPos.x++;
            }
        }
        return newPos;
    }
}
