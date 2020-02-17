using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
  private enum Animation { MoveSprites, MoveCars, SpawnCopSprites, None }; // All animation "states" our renderer can be in

  // I need prefabs for each object type, IE cones, manholes, jay, etc
  public GameObject jay_sprite, cone_sprite, zebra_sprite, follower_sprite,
        car_warning_sprite;  // and prefabs for other game ObjectSpawner

  public GameObject car_sprite; // and prefabs for other game ObjectSpawner

  // Coordinates (x, y) of the bottom left and top right cells
  private Vector2Int blCell;
  private Vector2Int trCell;
  Tilemap tilemap; // And the tilemap those cells exist on
  Overworld overworld; // And the overworld to get grid height 

  // References to each GameObject we instantiate
  private Dictionary<GameElement, GameObject> spawnedSprites;

  // fields for specific animations, we need to hold onto these variables between calls to Update()
  private Animation currAnimation;

  private Dictionary<Animation, Func<bool>> animationUpdates;

  // returns true if the renderer is in an animation, otherwise false
  public bool IsInAnimation()
  {
    return this.currAnimation != Animation.None;
  }

  // Snaps all current GameObjects to their proper locations
  public void MoveSprites()
  {
    if (this.IsInAnimation()) // THIS SHOULD NEVER HAPPEN
    {
      Debug.Log("MoveSprites animation was cancelled");
      return;
    }

    currAnimation = Animation.MoveSprites;

    // when called, this method moves each GameObject closer to its given destination
    Func<bool> MoveSpritesUpdate = () =>
    {
      foreach (GameElement obj in spawnedSprites.Keys)
      {
        Vector3 destination = ConvertCellLoc(obj.position);
        GameObject sprite = spawnedSprites[obj];
        Vector3 currPos = sprite.transform.position;

        Vector3 newPos = Vector3.Lerp(currPos, destination, 10.0f * Time.deltaTime);
        sprite.transform.position = newPos;
      }

      // now check if this animation is completed and update AnimationState if so
      bool animationIsComplete = true;
      foreach (GameElement obj in spawnedSprites.Keys)
      {
        animationIsComplete = animationIsComplete &&
                (Vector3.Distance(spawnedSprites[obj].transform.position, ConvertCellLoc(obj.position)) < 0.01f);
      }
      if (animationIsComplete)
      {
        this.currAnimation = Animation.None; // if so, our animation is set back to None
      }
      return true;
    };

    animationUpdates[Animation.MoveSprites] = MoveSpritesUpdate;
  }

  public void MoveCars(List<GameElement> killedOrig, List<int> carColumns)
  {
    if (this.IsInAnimation()) // THIS SHOULD NEVER HAPPEN
    {
      Debug.Log("MoveCars animation was cancelled");
      return;
    }

    currAnimation = Animation.MoveCars;

    List<GameElement> killed = new List<GameElement>(killedOrig);

    // each car spawned is mapped to its destination, IE a y-pos off-camera
    Dictionary<GameObject, Vector3> carDestinations = new Dictionary<GameObject, Vector3>();
    foreach (int xPos in carColumns)
    {
        GameObject carSprite = Instantiate(car_sprite) as GameObject;
        carSprite.transform.position = ConvertCellLoc(new Vector2Int(xPos, trCell.y - blCell.y + 1));
        carDestinations[carSprite] = ConvertCellLoc(new Vector2Int(xPos, -2)); // add that car's destination
    }

    // when called, this method moves each car down the map, destroy objects in killed they touch along the way
    Func<bool> MoveCarsUpdate = () =>
        {
          foreach (GameObject car in carDestinations.Keys)
          {
            Vector3 currPos = car.transform.position;
            // if a car is near any followers, destroy them
            for (int i = 0; i < killed.Count; i++)
            {
                if (Vector3.Distance(currPos, spawnedSprites[killed[i]].transform.position) < 2.0f)
                {
                    DestroySprite(killed[i]);
                    killed.RemoveAt(i);
                    i--;
                }
            }

            Vector3 newPos = Vector2.Lerp(currPos, carDestinations[car], 10.0f * Time.deltaTime);
            car.transform.position = newPos;
          }

          // now check if this animation is completed and update AnimationState if so
          bool animationIsComplete = true;
          foreach (GameObject car in carDestinations.Keys)
          {
            animationIsComplete = animationIsComplete &&
                    (Vector3.Distance(car.transform.position, carDestinations[car]) < 1.0f);
          }

          if (animationIsComplete)
          {
                this.currAnimation = Animation.None; // if so, our animation is set back to None
                // cleanup GameElements destroyed by the car (if they didn't get cleaned up already)
                foreach (GameElement runOver in killed)
                {
                    DestroySprite(runOver);
                }

                // and destroy car sprites now that they're offscreen
                foreach (GameObject car in carDestinations.Keys)
                {
                   Destroy(car);
                }
          }
          return true;
        };

    animationUpdates[Animation.MoveCars] = MoveCarsUpdate;
  }

  // SpawnCopSprites (List<Manholes> holes)

  public void SetMap(List<GameElement> people, List<TileObject> tiles, List<CarTile> cars)
  {
        foreach (GameElement person in people)
        {
          SpawnSprite(person);
        }
        foreach (TileObject tile in tiles)
        {
          SpawnSprite(tile);
        }
        foreach (CarTile car in cars)
        {
            SpawnSprite(car);
        }
  }

  void Update()
  {
        Debug.Log(this.currAnimation);
        animationUpdates[this.currAnimation]();
  }

  // Start is called before the first frame update
  void Awake()
  {
    overworld = GameObject.Find("Overworld").GetComponent<Overworld>();
    tilemap = transform.GetComponent<Tilemap>();
    spawnedSprites = new Dictionary<GameElement, GameObject>();

    currAnimation = Animation.None;
    animationUpdates = new Dictionary<Animation, Func<bool>>();
    animationUpdates[Animation.None] = (() => { return true; });

    // bl stands for bottom left not "boys love"
    Vector3Int blLoc = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Vector3.zero));
    blCell.x = blLoc.x;
    blCell.y = blLoc.y;
    Debug.Log("blcell " + blCell.x + ", " + blCell.y);

    // tr stands for top right
    Vector3Int trLoc = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(
        new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0)));
    trCell.x = trLoc.x;
    trCell.y = trLoc.y;
    Debug.Log("trcell " + trCell.x + ", " + trCell.y);
  }

  // converts a given Vector2Int into a location in the world space
  private Vector3 ConvertCellLoc(Vector2Int coords)
  {
    Vector2Int fixedCoords = coords; // ConvertGridIndexToCoordSpace(coords);

    // adjust coords over bottom left cell
    Vector3Int adjustedCoords =
        new Vector3Int(fixedCoords.x + blCell.x, fixedCoords.y + blCell.y, 0);
    Debug.Log("adjusted to " + adjustedCoords);

    Vector3 res = tilemap.GetCellCenterLocal(adjustedCoords);

    return new Vector3(res.x + 1, res.y + 1, 0); // 1 cell of padding
  }

  private void SpawnSprite(GameElement character)
  {
    Debug.Log("Cell size: " + tilemap.cellSize);

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
      case GameElement.ElementType.Car:
        newObj = Instantiate(car_warning_sprite) as GameObject;
        break;
      default:
        print("Spawn failed!");
        return; // This should never occur
    }

    Debug.Log("new location: " + ConvertCellLoc(loc));
    newObj.transform.position = ConvertCellLoc(loc);

    SpriteRenderer newObjBounds = newObj.GetComponent<SpriteRenderer>();

    Vector3 tilesize = tilemap.cellSize;
    Vector3 spritesize = newObjBounds.bounds.size;

    // scale sprite to size of grid
    newObj.transform.localScale = new Vector3(tilesize.x / spritesize.x, tilesize.y / spritesize.y, 1);
    spawnedSprites[character] = newObj;
  }

  private void DestroySprite(GameElement character)
  {
    Destroy(spawnedSprites[character]);
    spawnedSprites.Remove(character);
  }
}