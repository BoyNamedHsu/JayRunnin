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

    private Dictionary<Animation, Func<void, void>> animationUpdates;

    // returns true if the renderer is not in an animation, otherwise false
    public bool IsNotInAnimation() 
    {
        return this.currAnimation = Animation.None;
    }

    public void MoveSprites(Dictionary<Living, Vector2Int> destinations)
    {
        currAnimation = Animation.MoveSprites;
        this.destinations = destinations;

        // when called, this method moves each GameObject closer to its given destination
        Func<void, void> MoveSpritesUpdate = () =>
            {
                foreach (GameObject obj in destinations.Keys)
                {
                    Vector2 destination = destinations[obj];
                    Vector2 currPos = obj.transform.position;

                    Vector2 newPos = Vector2.Lerp(currPos, destination, 0.5f * Time.deltaTime);
                    obj.transform.position = newPos;
                }

                // now check if this animation is completed and update AnimationState if so
                bool animationIsComplete = true;
                foreach (GameObject obj in destinations.Keys)
                {
                    animationIsComplete = animationIsComplete && (obj.transform.position == destinations[obj]);
                }
                if (animationIsComplete){
                    this.currAnimation = Animation.None; // if so, our animation is set back to None
                }
            }
        
        animationUpdates[Animation.MoveSprites] = MoveSpritesUpdate;
    }

    public void MoveCars(List<Living> killed, List<CarTile> cars)
    {
        currAnimation = Animation.MoveSprites;
        this.destinations = destinations;

        // each car spawned is mapped to its destination, IE a y-pos off-camera
        Dictionary<GameObject, Vector2Int> carDestinations = new List<GameObject>();
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
        Func<void, void> MoveCarsUpdate = () =>
            {
                foreach (GameObject car in carSprites.Keys)
                {
                    Vector2 destination = carDestinations[car];
                    Vector2 currPos = car.transform.position;

                    Vector2 newPos = Vector2.Lerp(currPos, destination, 0.5f * Time.deltaTime);
                    car.transform.position = newPos;
                }

                // now check if this animation is completed and update AnimationState if so
                bool animationIsComplete = true;
                foreach (GameObject car in carSprites.Keys)
                {
                    animationIsComplete = animationIsComplete && (car.transform.position == carDestinations[car]);
                }
                if (animationIsComplete)
                {
                    // cleanup sprites destroyed by the car
                    foreach (Living runOver in killed)
                    {
                        DestroySprite(runOver);
                    }
                    // and destroy car sprites now that they're offscreen
                    foreach (GameObject car in carSprites.Keys)
                    {
                        Destroy(car);
                    }

                    // and cleanup the car sprites
                    this.currAnimation = Animation.None; // if so, our animation is set back to None
                }
            }
        
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
        animationUpdates = new Dictionary<Animation, Func<void, void>>();
        AnimationUpdates[Animation.None] = (() => { return; });

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

    // converts a given Vector2Int into a location in the world space
    private Vector2 ConvertCellLoc(Vector2Int coords)
    {
        // adjust coords over bottom left cell
        Vector3Int adjustedCoords = new Vector3Int(coords.x + blCell.x, coords.y + blCell.y, 0);
        Debug.Log(coords.x + ", " + coords.y);
        Debug.Log(adjustedCoords.x + ", " + adjustedCoords.y);
        Vector3 res = tilemap.GetCellCenterLocal(adjustedCoords);

        return new Vector2(res.x + 1, res.y + 1); // 1 cell of padding
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
                Debug.Log("f");
                break;
            default:
                print("Spawn failed!");
                return; // This should never occur
        }

        Vector2 worldLoc = ConvertCellLoc(loc);

        newObj.transform.position = worldLoc;
        spawnedSprites[character] = newObj;
        destinations[newObj] = worldLoc;
    }

    private void DestroySprite(Living character)
    {
        Destroy(spawnedSprites[character]);
        spawnedSprites.Remove(character);
    }
}