// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class ObjectSpawner : MonoBehaviour
// {
//     // For Thomas and Kelvin:
//     enum SpriteType
//     {
//         Jay,
//         Cone,
//         Manhole
//     }

//     // This stems from Kelvin/Thomas's implementation
//     private class TileObj
//     {
//         public SpriteType sType;

//         public TileObj (SpriteType sType) {
//             this.sType = sType;
//         }
//     }

//     /*
//      * 
//      * MY CODE STARTS HERE:
//      * 
//      */
//     // I need prefabs for each object type, IE cones, manholes, jay, etc
//     public GameObject jay, cone, zebra, follower;

//     // I also need references to each GameObject we instantiate
//     private Dictionary<TileObj, GameObject> spawnedSprites;

//     // Coordinates (x, y) of the bottom left 
//     private Vector2Int blCell;
//     // Tilemap
//     Tilemap tilemap;

//     // Given a start grid state and end grid state, generates a list of diffs for updating sprite locations
//     // returns each TileObj mapped to a tuple of its old location (or null if it did not previously exist), and its new location
//     // (or null if that object no longer exists, IE was destroyed)
//     private Dictionary<TileObj, Tuple<Vector2Int?, Vector2Int?>> getStateDiff(TileObj[,] startGrid, TileObj[,] endGrid)
//     {
//         Dictionary<TileObj, Tuple<Vector2Int?, Vector2Int?>> diffs = new Dictionary<TileObj, Tuple<Vector2Int?, Vector2Int?>>();

//         // get all old locations
//         for (int y = 0; y < startGrid.GetLength(0); y++)
//         {
//             for (int x = 0; x < startGrid.GetLength(1); x++)
//             {
//                 TileObj curr = startGrid[y, x];
//                 if (curr != null)
//                 {
//                     Vector2Int? oldLoc = new Vector2Int(y, x);
//                     Vector2Int? newLoc = null;

//                     diffs[curr] = new Tuple<Vector2Int?, Vector2Int?>(oldLoc, newLoc);
//                 }
//             }
//         }

//         // get all new locations
//         for (int y = 0; y < endGrid.GetLength(0); y++)
//         {
//             for (int x = 0; x < endGrid.GetLength(1); x++)
//             {
//                 TileObj curr = endGrid[y, x];

//                 if (curr != null)
//                 {
//                     Vector2Int? oldLoc = diffs.ContainsKey(curr) ? diffs[curr].Item1 : null;
//                     Vector2Int? newLoc = new Vector2Int(y, x);

//                     diffs[curr] = new Tuple<Vector2Int?, Vector2Int?>(oldLoc, newLoc);
//                 }
//             }
//         }

//         return diffs;
//     }

//     // converts a given Vector2Int into a location in the world space
//     private Vector2 convertCellLoc(Vector2Int coords)
//     {
//         // adjust coords over bottom left cell
//         Vector3Int adjustedCoords = new Vector3Int(coords.x + blCell.x, coords.y + blCell.y, 0);
//         Vector3 res = tilemap.GetCellCenterLocal(adjustedCoords);

//         return new Vector2(res.y, res.x); // this is flipped to fix a bug
//     }

//     private GameObject spawnObj (TileObj obj, Vector2Int loc)
//     {
//         SpriteType objType = obj.sType;
//         GameObject newObj;

//         switch (obj.sType)
//         {
//             case SpriteType.Cone:
//                 newObj = Instantiate(cone) as GameObject;
//                 break;
//             case SpriteType.Jay:
//                 newObj = Instantiate(jay) as GameObject;
//                 break;
//             case SpriteType.Manhole:
//                 newObj = Instantiate(manhole) as GameObject;
//                 break;
//             default:
//                 return null; // This should never occur
//         }
//         newObj.transform.position = convertCellLoc(loc);
//         return newObj;
//     }

//     private void renderTileObjDiff(TileObj obj, Vector2Int? oldLoc, Vector2Int? newLoc)
//     {
//         // Assert(oldLoc != null || newLoc != null);
//         if (oldLoc == null && newLoc != null) // our first time seeing this object!, we need to instantiate it
//         {
//             spawnedSprites[obj] = spawnObj(obj, newLoc.Value);
//         } else if (oldLoc != null && newLoc == null) // we need to destroy an object that's currently rendered
//         {
//             GameObject removed = spawnedSprites[obj];
//             spawnedSprites.Remove(obj);
//             Destroy(removed);
//         } else if (oldLoc != null && newLoc != null) // otherwise we just need to transform thie object
//         {
//             spawnedSprites[obj].transform.position = convertCellLoc(newLoc.Value);
//         } else
//         {
//             return; // this should be unreachable?
//         }
//     }

//     private void renderDiffs(Dictionary<TileObj, Tuple<Vector2Int?, Vector2Int?>> stateDiffs)
//     {
//         foreach (TileObj obj in stateDiffs.Keys)
//         {
//             renderTileObjDiff(obj, stateDiffs[obj].Item1, stateDiffs[obj].Item2);
//         }
//     }

//     // Start is called before the first frame update
//     void Start()
//     {
//         tilemap = transform.GetComponent<Tilemap>();
//         spawnedSprites = new Dictionary<TileObj, GameObject>();

//         // bl stands for bottom left not "boys love"
//         Vector3Int blLoc = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Vector3.zero));
//         blCell.x = blLoc.x;
//         blCell.y = blLoc.y;

//         /*
//          * Testing with grids:
//          */

//         TileObj coneObj = new TileObj(SpriteType.Cone);
//         TileObj jayObj = new TileObj(SpriteType.Jay);
//         TileObj copObj = new TileObj(SpriteType.Manhole); // should be a manhole, just a cop rn

//         TileObj[,] emptyG = {{null, null, null},
//                              {null, null, null},
//                              {null, null, null}};

//         TileObj[,] grid1 = {{null, null, null},
//                             {null, coneObj, copObj},
//                             {null, null, jayObj}};

//         TileObj[,] grid2 = {{null, null, copObj},
//                             {null, null, coneObj},
//                             {null, jayObj, null}};

//         TileObj[,] grid3 = {{null, null, null},
//                             {null, null, null},
//                             {null, null, coneObj}};

//         Dictionary<TileObj, Tuple<Vector2Int?, Vector2Int?>> diffs = getStateDiff(emptyG, grid1);
//         renderDiffs(diffs);

//         renderDiffs(getStateDiff(grid1, grid2));
//         //renderDiffs(getStateDiff(grid2, grid3));
//     }
// }