using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    public LevelManager manager;

    // Start is called before the first frame update
    void Start()
    {
        LoadLevel1();
    }

    private void LoadLevel1() 
    {
        Vector2Int jayPos = new Vector2Int(0, 0);

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = {
            {null, null, GameElement.ElementType.ManHole, null, null},
            {null, null, null, null, null},
            // {null, null, null, null},
            // {null, null, null, null},
            {null, null, null, null, null}
        };
        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(1, 4));
        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();

        manager.LoadLevel(jayPos, objects, cars, portals);
    }
}
