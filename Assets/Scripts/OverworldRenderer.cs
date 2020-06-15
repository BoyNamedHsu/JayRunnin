using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using UnityEngine.Rendering;

public class OverworldRenderer : MonoBehaviour
{
    // I need prefabs for each object type, IE cones, manholes, jay, etc
    public GameObject Jay_Sprite, Cone_Sprite, Cop_Sprite, ManHole_Sprite, Fan_Sprite, FanHole_Sprite,
      Zebra_Sprite, Flagpole_Sprite, Sidewalk_Sprite, Invisible_Sprite, Portal_Sprite;

    public GameObject Car_Sprite, rToRestartTutorial; // and prefabs for other game ObjectSpawner
    public GameObject Warning, Cop_Counter_Sprite; // prefab for warning object

    Tilemap tilemap; // And the tilemap those cells exist on

    // References to each GameObject we instantiate
    private Dictionary<GameElement, GameObject> spawnedSprites;

    // List of the car warning UI elements with the timer countdown
    private Dictionary<Car, GameObject> CarWarnings;
    private GameObject CopCounter;
    private GameObject rToRestart;
    public SoundPlayer GameAudio; // And our sound player

    private List<Func<bool>> animations; // All the movement animations we'll between during updating
    private Func<bool> update; // allows us to change our FixedUpdate tic

    const float DELTA = 0.5f; // Delta for distance calculations 
    private Func<bool> NOOP = () => true; // treat this as a const

    private void Clear() // Deletes everything onscreen!
    {
        update = NOOP;
        animations = new List<Func<bool>>(); // Cancel our animations
        if (spawnedSprites != null)
        {
            foreach (GameElement ge in spawnedSprites.Keys)
                Destroy(spawnedSprites[ge]);
        }
        spawnedSprites = new Dictionary<GameElement, GameObject>(); // All sprites gone

        if (CarWarnings != null)
        {
            foreach (Car car in CarWarnings.Keys)
                Destroy(CarWarnings[car]);
        }
        CarWarnings = new Dictionary<Car, GameObject>();

        if (CopCounter != null)
            Destroy(CopCounter);
        CopCounter = null;
        if (rToRestart != null)
            Destroy(rToRestart);
        rToRestart = null;
    }

    // Start is called before the first frame update
    void Awake()
    {
        tilemap = transform.GetComponent<Tilemap>();
        this.Clear();
    }

    void FixedUpdate()
    {
        update();
    }

    // Deletes renders the given grid
    public void SyncSprites(Overworld grid)
    {
        HashSet<GameElement> gridObjects = grid.GetAllObjects();
        HashSet<GameElement> toRemove = new HashSet<GameElement>();
        foreach (GameElement obj in spawnedSprites.Keys)
        {
            if (!gridObjects.Contains(obj))
                toRemove.Add(obj);
        }
        foreach (GameElement removed in toRemove)
        {
            Destroy(spawnedSprites[removed]);
            spawnedSprites.Remove(removed);
        } // Deleted all extra sprites

        foreach (GameElement obj in gridObjects)
        {
            if (!spawnedSprites.ContainsKey(obj))
            {
                SpawnSprite(obj);
            }
            spawnedSprites[obj].transform.position = ConvertCellLoc(obj.position); // (Don't really need this lol)
        } // All sprites synced
        foreach (TileObject tile in grid.GetAllTiles())
        {
            if ((tile.eid == GameElement.ElementType.FanHole || tile.eid == GameElement.ElementType.ManHole)
                && grid.GetOccupant(tile) != null)
                ShutManhole(tile);
        }  // Finally close all occupied manholes

        GameObject canvas = GameObject.Find("CanvasUI");
        if (grid.copsGoal > 0)
        {
            if (CopCounter == null)
            {
                // these transformations are sus lmao
                CopCounter = GameObject.Instantiate(Cop_Counter_Sprite);
                CopCounter.transform.localScale = new Vector3(tilemap.cellSize.x / 100f, tilemap.cellSize.y / 100f, 1);
                CopCounter.transform.SetParent(canvas.transform);
                CopCounter.transform.position = ConvertCellLoc(new Vector2Int(grid.width - 1, 0));
            }
            TMPro.TextMeshProUGUI counter = CopCounter.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            counter.text = grid.copsDefeated + "/" + grid.copsGoal;
            if (grid.copsDefeated >= grid.copsGoal)
                counter.color = new Color32(0, 255, 0, 255);
        } // Cop counter synced

        foreach (Car car in grid.cars)
        {
            int countdown = car.triggerTurn - grid.turnCount;
            if (countdown > 0)
            {
                if (!CarWarnings.ContainsKey(car))
                {
                    GameObject newWarning = GameObject.Instantiate(Warning);
                    newWarning.transform.localScale = new Vector3(tilemap.cellSize.x / 100f, tilemap.cellSize.y / 100f, 1);
                    newWarning.transform.SetParent(canvas.transform);
                    newWarning.transform.position = ConvertCellLoc(new Vector2Int(car.xPos, (grid.height - 1)));
                    CarWarnings[car] = newWarning;
                }
                CarWarnings[car].GetComponentInChildren<TextMeshProUGUI>().text = "" + countdown;
                if (countdown == 1)
                    CarWarnings[car].GetComponentInChildren<Animator>().Play("shaking");
            }
            else if (CarWarnings.ContainsKey(car))
            {
                Destroy(CarWarnings[car]);
                CarWarnings[car] = null;
            }
        } // All car warnings Synced

        if (rToRestart != null)
        {
            Destroy(rToRestart);
            rToRestart = null;
        }
        if (grid.IsStuck())
            rToRestart = GameObject.Instantiate(rToRestartTutorial);
    }

    // This method erases the grid, so there's no possilibity of desyncs.
    public void SyncSpritesHard(Overworld grid)
    {
        this.Clear();
        this.SyncSprites(grid);
    }

    public void UpdateSpriteDirection(List<LevelManager.Direction> jayDirections, List<Follower> followers, int head)
    {
        for (int i = head; i < followers.Count; i++)
        {
            var anim = spawnedSprites[followers[i]].GetComponent<Animator>();
            if (jayDirections[i] == LevelManager.Direction.West)
                anim.Play("Idle West");
            else if (jayDirections[i] == LevelManager.Direction.East)
                anim.Play("Idle East");
            else if (jayDirections[i] == LevelManager.Direction.South)
                anim.Play("Idle South");
            else
                anim.Play("Idle North");
        }
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
            case GameElement.ElementType.Fan:
                newObj = Instantiate(Fan_Sprite) as GameObject;
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
            case GameElement.ElementType.InvisibleWall:
                newObj = Instantiate(Invisible_Sprite) as GameObject;
                break;
            case GameElement.ElementType.FanHole:
                newObj = Instantiate(FanHole_Sprite) as GameObject;
                break;
            case GameElement.ElementType.Portal:
                newObj = Instantiate(Portal_Sprite) as GameObject;
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

    private void ScaleSprite(GameObject obj)
    {
        SpriteRenderer objBounds = obj.GetComponent<SpriteRenderer>();
        Vector3 tilesize = tilemap.cellSize;
        Vector3 spritesize = objBounds.bounds.size;

        // scale sprite to size of grid
        obj.transform.localScale = new Vector3(tilesize.x / spritesize.x, tilesize.y / spritesize.y, 1);
    }

    // scales the camera to fit the given width/height of cells
    public void ScaleCamera(int height, int width)
    {
        Vector3 cellSize = tilemap.cellSize;
        Camera.main.orthographicSize = cellSize.y * height / 2f;
        Transform tmp = Camera.main.GetComponent<Transform>();
        tmp.position = new Vector3(cellSize.y * width / 2f, cellSize.y * height / 2f, -10);
    }

    // Changes the car warning sprites depending on zebra.
    // zebra represents whether tile Jay is standing on is a zebra tile.
    public void ChangeCarWarningSprite(bool zebra)
    {
        foreach (var warning in CarWarnings)
        {
            GameObject stopSign = warning.Value.transform.GetChild(1).gameObject;
            stopSign.SetActive(zebra);
            GameObject warningSign = warning.Value.transform.GetChild(0).gameObject;
            warningSign.SetActive(!zebra);
        }
    }

    /*
     * All of these public methods enqueue animations for the renderer to perform
     */
    public IEnumerator PlayAnimations() // Plays every enqueued animation
    {
        Func<bool> PlayAnimations =
            () =>
            {
                for (int i = animations.Count - 1; i >= 0; i--)
                {
                    if (animations[i]())
                        animations.RemoveAt(i); // If that animation finished, remove it
                }
                return true;
            };
        update = PlayAnimations;
        yield return new WaitUntil(() => animations.Count == 0); // when empty, no more animations to play
        update = NOOP;
        yield return null;

        // This can hang! If there are two opposing move commands for single sprite, this stalls
        // A map is likely a better way to represent this
    }

    /*
     * Note: None of these animations have side effects *until* they're executed
     */
    public void MoveLiving(LivingObject obj, Vector2Int dest) // Moves a sprite to the given location
    {
        Vector3 real_destination = ConvertCellLoc(dest);
        Func<bool> MoveToDest =
            () =>
            {
                if (!spawnedSprites.ContainsKey(obj)) return true; // If the sprite has been deleted, don't move it!
                GameObject sprite = spawnedSprites[obj];
                float dist = Vector3.Distance(sprite.transform.position, real_destination);
                if (dist < 0.01f)
                    return true; // Our sprite is at our destination

                Vector3 newPos = (dist > 0.1f) ?
                    Vector3.Lerp(sprite.transform.position, real_destination, 10.0f * Time.deltaTime) :
                    Vector3.Lerp(sprite.transform.position, real_destination, 20.0f * Time.deltaTime);
                sprite.transform.position = newPos;
                return false;
            }; // This function moves sprite (obj) gradually to the destination position, returns true when done
        animations.Add(MoveToDest); // Add this function to our list of animation updates
    }

    public void SpawnObject(GameElement obj)
    {
        // This one will probably play those pop-out animations
        Func<bool> Spawn = () =>
        {
            this.SpawnSprite(obj);
            if (obj.eid == GameElement.ElementType.Cop || obj.eid == GameElement.ElementType.Fan)
            {
                // GameAudio.PlaySound("plop");
            }
            return true;
        };
        animations.Add(Spawn);
    }

    public void DeleteObject(GameElement obj)
    {
        Func<bool> Delete = () =>
        {
            if (spawnedSprites.ContainsKey(obj))
            {
                Destroy(spawnedSprites[obj]);
                spawnedSprites.Remove(obj);
            }
            return true;
        };
        animations.Add(Delete);
    }
    public void SendCar(Car car, Overworld grid, List<LivingObject> killed) // Sends a car down the given column
    {
        GameObject carSprite = null;
        bool firstExecution = true;
        Vector3 dest = ConvertCellLoc(new Vector2Int(car.xPos, -1));

        Func<bool> MoveCar =
            () =>
            {
                if (firstExecution) // On our first call to this method we need to create a car!
                {
                    // GameAudio.PlaySound("car");
                    carSprite = Instantiate(Car_Sprite) as GameObject;
                    ScaleSprite(carSprite);
                    carSprite.transform.position = ConvertCellLoc(new Vector2Int(car.xPos, grid.height + 1));
                    if (CarWarnings.ContainsKey(car))
                    {
                        Destroy(CarWarnings[car]); /// And destroy its associated warning
                        CarWarnings.Remove(car);
                    }
                    firstExecution = false;
                }

                Vector3 currPos = carSprite.transform.position;
                if (Vector3.Distance(currPos, dest) < 1.0f)
                {
                    Destroy(carSprite);
                    return true; // Car has driven to bottom of screen!
                }

                foreach (GameElement key in killed)
                {
                    if (key.position.x == car.xPos &&
                        spawnedSprites[key].GetComponent<Renderer>().enabled &&
                        Vector3.Distance(currPos, spawnedSprites[key].transform.position) < 0.5f)
                    {
                        // GameAudio.PlaySound("thud");
                        CameraShake.Shake(0.05f, tilemap.cellSize.y / 20.0f);
                        // hide sprites that get runover
                        spawnedSprites[key].GetComponent<Renderer>().enabled = false;
                    }
                }

                // Cone hit animation:
                foreach (TileObject tile in grid.GetAllTiles())
                {
                    if (tile.eid == GameElement.ElementType.Cone && 
                        Vector3.Distance(currPos, spawnedSprites[tile].transform.position) < 0.5f)
                    {
                        PlaySpriteAnimation(tile, "cone hit");
                    }
                }


                Vector3 newPos = Vector2.Lerp(currPos, dest, 10.0f * Time.deltaTime);
                carSprite.transform.position = newPos;

                return false;
            };
        animations.Add(MoveCar);
    }

    public void PlayManholeClose(TileObject tile)
    {
        Func<bool> CloseHole = () =>
        {
            ShutManhole(tile);
            return true;
        };
        animations.Add(CloseHole);
    }

    private void PlaySpriteAnimation(GameElement el, String animationName)
    {
        spawnedSprites[el].GetComponent<Animator>().Play(animationName);
    }

    private void ShutManhole(TileObject tile)
    {
        if (tile.eid != GameElement.ElementType.FanHole && tile.eid != GameElement.ElementType.ManHole)
            return;
        if (tile.eid == GameElement.ElementType.FanHole)
        {
            PlaySpriteAnimation(tile, "New Animation");
        }
        else if (tile.eid == GameElement.ElementType.ManHole)
        {
            Transform transMan = spawnedSprites[tile].transform;
            transMan.GetChild(1).GetComponent<Animator>().Play("ManholeTopClose");
            transMan.GetChild(2).gameObject.SetActive(false);
        }
    } // I really want a method that insta-closes a manhole too
}