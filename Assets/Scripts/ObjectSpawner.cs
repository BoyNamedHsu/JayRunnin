using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    // I need prefabs for each object type, IE cones, manholes, jay, etc
    public GameObject jay, cone, zebra, follower, car_obj;  // and prefabs for other game objects

    private enum TurnPhase
    {
        moves,
        cars,
        update, 
        wait
    }

    // I also need references to each GameObject we instantiate
    private Dictionary<Living, GameObject> spawnedSprites;

    // Positions each sprite should "drift" towards
    private Dictionary<GameObject, Vector2> destinations;

    // Coordinates (x, y) of the bottom left and top right cells
    private Vector2Int blCell;
    private Vector2Int trCell;

    // Tilemap
    Tilemap tilemap;

    // we shouldn't need this but uh, Thomas didn't use Vector2Int
    private Vector2Int convertThomTuple((int x, int y) coords)
    {
        return new Vector2Int(coords.x, coords.y);
    }

  private Vector2Int getDest(GameManager.Direction dir, Vector2Int src)
  {
    Vector2Int dest = new Vector2Int(src.x, src.y);
    switch (dir)
    {
      case GameManager.Direction.North:
        dest.y++;
        break;
      case GameManager.Direction.South:
        dest.y--;
        break;
      case GameManager.Direction.East:
        dest.x++;
        break;
      case GameManager.Direction.West:
        dest.x--;
        break;
      default:
        return dest; // This should never occur
    }
    return dest;
  }

    public void setMap(List<Living> people)
    {
        foreach (Living person in people)
        {
            spawnObj(person);
        }
    }

    // moves part of a list of people, starting with the given person in line
    public void MoveList(GameManager.Direction dir, List<Living> people, int startPerson)
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
    }

    // moves an entire list of people, starting with Jay
    public void MoveFullChain(GameManager.Direction dir, List<Living> people)
    {
        MoveList(dir, people, 0);
    }

    // converts a given Vector2Int into a location in the world space
    private Vector2 convertCellLoc(Vector2Int coords)
    {
        // adjust coords over bottom left cell
        Vector3Int adjustedCoords = new Vector3Int(coords.x + blCell.x, coords.y + blCell.y, 0);
        Debug.Log(coords.x + ", " + coords.y);
        Debug.Log(adjustedCoords.x + ", " + adjustedCoords.y);
        Vector3 res = tilemap.GetCellCenterLocal(adjustedCoords);

        return new Vector2(res.x + 1, res.y + 1); // 1 cell of padding
    }

    public void runCar(List<CarTile> cars)
    {
        foreach (CarTile car in cars)
        {
            if (car.countdown == 0 && !car.gone) // we should just remove cars from the list instead
            {
                GameObject carSprite = Instantiate(car_obj) as GameObject;
                carSprite.transform.position = convertCellLoc(new Vector2Int(car.yPos, trCell.y - blCell.y + 1));
                destinations[carSprite] = convertCellLoc(new Vector2Int(car.yPos, -10));
            }
        }
    }

    private void spawnObj(Living character)
    {
        Vector2Int loc = character.position;
        Debug.Log(loc);
        GameElement.ElementType characterType = character.eid;
        Debug.Log(characterType);
        GameObject newObj;

        switch (character.eid)
        {
            case GameElement.ElementType.Jay:
                newObj = Instantiate(jay) as GameObject;
                Debug.Log("j");
                break;
            case GameElement.ElementType.Cone:
                newObj = Instantiate(cone) as GameObject;
                break;
            case GameElement.ElementType.Zebra:
                newObj = Instantiate(zebra) as GameObject;
                break;
            case GameElement.ElementType.Follower:
                newObj = Instantiate(follower) as GameObject;
                Debug.Log("f");
                break;
            default:
                print("Spawn failed!");
                return; // This should never occur
        }

        Vector2 worldLoc = convertCellLoc(loc);

        newObj.transform.position = worldLoc;
        spawnedSprites[character] = newObj;
        destinations[newObj] = worldLoc;
    }

    // Start is called before the first frame update
    void Awake()
    {
        tilemap = transform.GetComponent<Tilemap>();
        spawnedSprites = new Dictionary<Living, GameObject>();
        destinations = new Dictionary<GameObject, Vector2>();

        // bl stands for bottom left not "boys love"
        Vector3Int blLoc = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Vector3.zero));
        blCell.x = blLoc.x;
        blCell.y = blLoc.y;
        Debug.Log(blCell.x + ", " + blCell.y);
        //
        // tr stands for top right
        Vector3Int trLoc = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(
            new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0)));
        trCell.x = trLoc.x;
        trCell.y = trLoc.y;
        Debug.Log(trCell.x + ", " + trCell.y);
        /*
        // Testing chain movement
        
        List<Living> testChain = new List<Living>();
        Jay player = new Jay(0, 0);
        
        testChain.Add(player);
        for (int i = 1; i < 5; i++)
        {
            testChain.Add(new Follower(0, i, true));
        }

        // now spawn each "chess piece"
        foreach (Living character in testChain)
        {
            spawnObj(character);
        }

        // finally try moving the chain around
        MoveFullChain(GameManager.Direction.East, testChain);
        

        // Testing car movement
        List<CarTile> cars = new List<CarTile>();
        cars.Add(new CarTile(1, 0));
        cars.Add(new CarTile(4, 0));
        runCar(cars);
        */
    }

    void Update()
    {
        // drift each GameObject closer to its destination
        foreach (GameObject obj in destinations.Keys)
        {
            Vector2 destination = destinations[obj];
            Vector2 currPos = obj.transform.position;

            Vector2 newPos = Vector2.Lerp(currPos, destination, 0.5f * Time.deltaTime);
            obj.transform.position = newPos;
        }
    }
}