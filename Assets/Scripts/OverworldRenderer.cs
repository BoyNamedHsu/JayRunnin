using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class OverworldRenderer : MonoBehaviour
{
  private enum Animation { MoveSprites, MoveCars, SpawnCopSprites, None }; // All animation "states" our renderer can be in

  // I need prefabs for each object type, IE cones, manholes, jay, etc
  public GameObject Jay_Sprite, Cone_Sprite, Cop_Sprite, ManHole_Sprite, 
    Zebra_Sprite, Flagpole_Sprite, Sidewalk_Sprite;

  public GameObject Car_Sprite; // and prefabs for other game ObjectSpawner
  public GameObject Warning; // prefab for warning object

  Tilemap tilemap; // And the tilemap those cells exist on

  // References to each GameObject we instantiate
  private Dictionary<GameElement, GameObject> spawnedSprites;

  // fields for specific animations, we need to hold onto these variables between calls to Update()
  private Animation currAnimation;

  // Kinda weird, but the dictionary of different animations we're in
  private Dictionary<Animation, Func<bool>> animationUpdates;

  // List of the car warning UI elements with the timer countdown
  private Dictionary<Car, GameObject> CarWarnings = new Dictionary<Car, GameObject>();

  // returns true if the renderer is in an animation, otherwise false
  public bool IsInAnimation()
  {
    return this.currAnimation != Animation.None;
  }

  // Snaps all current GameObjects to their proper locations
  // (Requires that the positions of GameElements have been modified)
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
                (Vector3.Distance(spawnedSprites[obj].transform.position, ConvertCellLoc(obj.position)) < 0.1f);
      }
      if (animationIsComplete)
      {
        this.currAnimation = Animation.None; // if so, our animation is set back to None
      }
      return true;
    };

    animationUpdates[Animation.MoveSprites] = MoveSpritesUpdate;
  }

  // Updates cars UI countdown on screen given the current turn the user is on
  // To do: remove the UI elements when countdown is zero, place them in the according column position
  public void UpdateCarCount(List<Car> cars, int turn, int height)
  {
    GameObject canvas = GameObject.Find("Canvas");
    foreach (Car car in cars)
    {
      if (car.triggerTurn <= turn){
        if (CarWarnings.ContainsKey(car)){
          Destroy(CarWarnings[car]);
          CarWarnings.Remove(car);
        }
      } else {
        if (!CarWarnings.ContainsKey(car))
        {
            // these transformations are sus lmao
            GameObject newWarning = GameObject.Instantiate(Warning);
            newWarning.transform.localScale = new Vector3(tilemap.cellSize.x / 100f, tilemap.cellSize.y / 100f, 1);
            newWarning.transform.SetParent(canvas.transform);

            newWarning.transform.position = ConvertCellLoc(new Vector2Int(car.xPos, height));
            CarWarnings[car] = newWarning;
        }
        int countdown = car.triggerTurn - turn;
        CarWarnings[car].GetComponentInChildren<Text>().text = "" + countdown;
      }
    }
  }

  public void MoveCars(List<Follower> killedOrig, List<int> carColumns, Overworld grid)
  {
    if (this.IsInAnimation()) // THIS SHOULD NEVER HAPPEN
    {
      Debug.Log("MoveCars animation was cancelled");
      return;
    }

    currAnimation = Animation.MoveCars;

    List<Follower> killed = new List<Follower>(killedOrig);

    // each car spawned is mapped to its destination, IE a y-pos off-camera
    Dictionary<GameObject, Vector3> carDestinations = new Dictionary<GameObject, Vector3>();
    foreach (int xPos in carColumns)
    {
        GameObject carSprite = Instantiate(Car_Sprite) as GameObject;
        ScaleSprite(carSprite);
        carSprite.transform.position = ConvertCellLoc(new Vector2Int(xPos, grid.height + 1));
        carDestinations[carSprite] = ConvertCellLoc(new Vector2Int(xPos, -1));
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
                    // shake is scaled to cellsize
                    CameraShake.Shake(0.05f, tilemap.cellSize.y / 20.0f);
                    // hide sprites that get runover
                    spawnedSprites[killed[i]].GetComponent<Renderer>().enabled = false;
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

  // Spawns in missing objects from a grid
  // Despawns objects that no longer exist in grid
  public void SyncSprites(Overworld grid){
    HashSet<GameElement> gridElements = grid.GetAllObjects();

    // despawn no longer existing sprites
    HashSet<GameElement> toRemove = new HashSet<GameElement>();
    foreach (GameElement obj in spawnedSprites.Keys){
      if (!gridElements.Contains(obj)){
        toRemove.Add(obj);
      }
    }
    foreach (GameElement obj in toRemove){
      DestroySprite(obj);
    }

    // spawn missing sprites
    foreach (GameElement obj in gridElements){
      if (!spawnedSprites.ContainsKey(obj)){
        SpawnSprite(obj);
      }
    }

    UpdateCarCount(grid.cars, grid.turnCount, grid.height - 1);
  }

  void Update()
  {
    animationUpdates[this.currAnimation]();
  }

  // Start is called before the first frame update
  void Awake()
  {
    tilemap = transform.GetComponent<Tilemap>();
    spawnedSprites = new Dictionary<GameElement, GameObject>();
    CarWarnings = new Dictionary<Car, GameObject>();

    currAnimation = Animation.None;
    animationUpdates = new Dictionary<Animation, Func<bool>>();
    animationUpdates[Animation.None] = (() => { return true; });
  }

  // converts a given Vector2Int into a location in the world space
  private Vector3 ConvertCellLoc(Vector2Int coords)
  {
    Vector3 res = tilemap.GetCellCenterLocal(new Vector3Int(coords.x, coords.y, 0));

    // then center on those tiles
    return new Vector3(res.x + (tilemap.cellSize.x / 2f), res.y + (tilemap.cellSize.y / 2f), 0);
  }

  private void SpawnSprite(GameElement character)
  {
    Vector2Int loc = character.position;
    GameElement.ElementType characterType = character.eid;
    GameObject newObj;

    switch (character.eid)
    {
      case GameElement.ElementType.Jay:
        newObj = Instantiate(Jay_Sprite) as GameObject;
        break;
      case GameElement.ElementType.Cone:
        newObj = Instantiate(Cone_Sprite) as GameObject;
        break;
      case GameElement.ElementType.Cop:
        newObj = Instantiate(Cop_Sprite) as GameObject;
        break;
      case GameElement.ElementType.ManHole:
        newObj = Instantiate(ManHole_Sprite) as GameObject;
        break;
      case GameElement.ElementType.Zebra:
        newObj = Instantiate(Zebra_Sprite) as GameObject;
        break;
      case GameElement.ElementType.Flagpole:
        newObj = Instantiate(Flagpole_Sprite) as GameObject;
        break;
      case GameElement.ElementType.Sidewalk:
        newObj = Instantiate(Sidewalk_Sprite) as GameObject;
        break;
      default:
        Debug.Log("Spawn failed!");
        return; // This should never occur
    }

    newObj.transform.position = ConvertCellLoc(loc);

    // scale sprite to size of grid
    ScaleSprite(newObj);
    spawnedSprites[character] = newObj;
  }

  private void ScaleSprite (GameObject obj){
    SpriteRenderer objBounds = obj.GetComponent<SpriteRenderer>();
    Vector3 tilesize = tilemap.cellSize;
    Vector3 spritesize = objBounds.bounds.size;

    // scale sprite to size of grid
    obj.transform.localScale = new Vector3(tilesize.x / spritesize.x, tilesize.y / spritesize.y, 1);
  }

  // scales the camera to fit the given width/height of cells
  public void ScaleCamera(int height, int width){
    Vector3 cellSize = tilemap.cellSize;
    Camera.main.orthographicSize = cellSize.y * height / 2f;
    Transform tmp = Camera.main.GetComponent<Transform>();
    tmp.position = new Vector3(cellSize.y * width / 2f, cellSize.y * height / 2f, -10);
  }

  private void DestroySprite(GameElement character)
  {
    Destroy(spawnedSprites[character]);
    spawnedSprites.Remove(character);
  }
}