using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    private enum Animation{MoveSprites, MoveCars, SpawnCopSprites, None}; // All animation "states" our renderer can be in

    // I need prefabs for each object type, IE cones, manholes, jay, etc
    public GameObject jay_sprite, cone_sprite, zebra_sprite, follower_sprite, car_sprite;  // and prefabs for other game ObjectSpawner

    // Coordinates (x, y) of the bottom left and top right cells
    private Vector2Int blCell;
    private Vector2Int trCell;
    Tilemap tilemap; // And the tilemap those cells exist on

    // References to each GameObject we instantiate
    private Dictionary<Living, GameObject> spawnedSprites;

    // fields for specific animations, we need to hold onto these variables between calls to Update()
    private Animation currAnimation;

    private Dictionary<Animation, Func<bool>> animationUpdates;

    // returns true if the renderer is not in an animation, otherwise false
    public bool IsNotInAnimation() 
    {
        return this.currAnimation == Animation.None;
    }

    public void MoveSprites(Dictionary<Living, Vector2Int> destinations)
    {
        currAnimation = Animation.MoveSprites;

        // when called, this method moves each GameObject closer to its given destination
        Func<bool> MoveSpritesUpdate = () =>
            {
                foreach (Living obj in destinations.Keys)
                {
                    Vector3 destination = ConvertCellLoc(destinations[obj]);
                    GameObject sprite = spawnedSprites[obj];
                    Vector3 currPos = sprite.transform.position;

                    Vector3 newPos = Vector3.Lerp(currPos, destination, 0.5f * Time.deltaTime);
                    sprite.transform.position = newPos;
                }

                // now check if this animation is completed and update AnimationState if so
                bool animationIsComplete = true;
                foreach (Living obj in destinations.Keys)
                {
                    animationIsComplete = animationIsComplete && 
                        (Vector3.Distance(spawnedSprites[obj].transform.position, ConvertCellLoc(destinations[obj])) < 0.01f);
                }
                if (animationIsComplete)
                {
                    this.currAnimation = Animation.None; // if so, our animation is set back to None
                }
                return true;
            };
        
        animationUpdates[Animation.MoveSprites] = MoveSpritesUpdate;
    }

    public void MoveCars(List<Living> killed, List<CarTile> cars)
    {
        currAnimation = Animation.MoveSprites;

        // each car spawned is mapped to its destination, IE a y-pos off-camera
        Dictionary<GameObject, Vector3> carDestinations = new Dictionary<GameObject, Vector3>();
        foreach (CarTile car in cars)
        {
            if (car.countdown == 0 && !car.gone) // we should just remove cars from the list instead
            {
                GameObject carSprite = Instantiate(car_sprite) as GameObject;
                carSprite.transform.position = ConvertCellLoc(new Vector2Int(car.yPos, trCell.y - blCell.y + 1));
                carDestinations[carSprite] = ConvertCellLoc(new Vector2Int(car.yPos, -5)); // add that car's destination
            }
        }

        // when called, this method moves each car down the map, destroy objects in killed they touch along the way
        Func<bool> MoveCarsUpdate = () =>
            {
                foreach (GameObject car in carDestinations.Keys)
                {
                    Vector3 destination = carDestinations[car];
                    Vector3 currPos = car.transform.position;

                    Vector3 newPos = Vector2.Lerp(currPos, destination, 0.5f * Time.deltaTime);
                    car.transform.position = newPos;
                }

                // now check if this animation is completed and update AnimationState if so
                bool animationIsComplete = true;
                foreach (GameObject car in carDestinations.Keys)
                {
                    animationIsComplete = animationIsComplete && 
                        (Vector3.Distance(car.transform.position, carDestinations[car]) < 0.01f);
                }
                if (animationIsComplete)
                {
                    // cleanup sprites destroyed by the car
                    foreach (Living runOver in killed)
                    {
                        DestroySprite(runOver);
                    }
                    // and destroy car sprites now that they're offscreen
                    foreach (GameObject car in carDestinations.Keys)
                    {
                        Destroy(car);
                    }

                    // and cleanup the car sprites
                    this.currAnimation = Animation.None; // if so, our animation is set back to None
                }
                return true;
            };
        
        animationUpdates[Animation.MoveCars] = MoveCarsUpdate;
    }

    // SpawnCopSprites (List<Manholes> holes)

    public void SetMap(List<Living> people)
    {
        foreach (Living person in people)
        {
            SpawnSprite(person);
        }
    }

    void Update()
    {
        animationUpdates[this.currAnimation]();
    }

    // Start is called before the first frame update
    void Awake()
    {
        tilemap = transform.GetComponent<Tilemap>();
        spawnedSprites = new Dictionary<Living, GameObject>();

        currAnimation = Animation.None;
        animationUpdates = new Dictionary<Animation, Func<bool>>();
        animationUpdates[Animation.None] = (() => { return true; });

        // bl stands for bottom left not "boys love"
        Vector3Int blLoc = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Vector3.zero));
        blCell.x = blLoc.x;
        blCell.y = blLoc.y;
        Debug.Log(blCell.x + ", " + blCell.y);
        
        // tr stands for top right
        Vector3Int trLoc = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(
            new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0)));
        trCell.x = trLoc.x;
        trCell.y = trLoc.y;
        Debug.Log(trCell.x + ", " + trCell.y);
        
        // Testing chain movement
        
        List<Living> testChain = new List<Living>();
        Jay player = new Jay(0, 0);
        
        testChain.Add(player);
        for (int i = 1; i < 5; i++)
        {
            testChain.Add(new Follower(0, i, true));
        }

        SetMap(testChain);

        // Testing car movement
        /*
        List<CarTile> cars = new List<CarTile>();
        cars.Add(new CarTile(1, 0));
        cars.Add(new CarTile(4, 0));
        runCar(cars);
        */
    }

    // converts a given Vector2Int into a location in the world space
    private Vector3 ConvertCellLoc(Vector2Int coords)
    {
        // adjust coords over bottom left cell
        Vector3Int adjustedCoords = new Vector3Int(coords.x + blCell.x, coords.y + blCell.y, 0);
        Debug.Log(coords.x + ", " + coords.y);
        Debug.Log(adjustedCoords.x + ", " + adjustedCoords.y);
        Vector3 res = tilemap.GetCellCenterLocal(adjustedCoords);

        return new Vector3(res.x + 1, res.y + 1, 0); // 1 cell of padding
    }

    private void SpawnSprite(Living character)
    {
        Vector2Int loc = character.position;
        Debug.Log(loc);
        GameElement.ElementType characterType = character.eid;
        Debug.Log(characterType);
        GameObject newObj;

        switch (character.eid)
        {
            case GameElement.ElementType.Jay:
                newObj = Instantiate(jay_sprite) as GameObject;
                Debug.Log("j");
                break;
            case GameElement.ElementType.Cone:
                newObj = Instantiate(cone_sprite) as GameObject;
                break;
            case GameElement.ElementType.Zebra:
                newObj = Instantiate(zebra_sprite) as GameObject;
                break;
            case GameElement.ElementType.Follower:
                newObj = Instantiate(follower_sprite) as GameObject;
                break;
            default:
                print("Spawn failed!");
                return; // This should never occur
        }
        newObj.transform.position = ConvertCellLoc(loc);
        Debug.Log(loc.x + " " + loc.y);

        spawnedSprites[character] = newObj;
    }

    private void DestroySprite(Living character)
    {
        Destroy(spawnedSprites[character]);
        spawnedSprites.Remove(character);
    }
}