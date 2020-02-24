using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameElement;

public class Levels : MonoBehaviour
{
    public LevelManager manager;

    // Start is called before the first frame update
    void Awake()
    {
        Invoke("LoadLevel" + LevelSelector.levelChosen, 0f);
    }

    private void LoadLevel1()
    {
        Vector2Int jayPos = new Vector2Int(0, 3);

        int width, height;
        width = 10;
        height = 6;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        List<Vector2Int> cars = new List<Vector2Int>();
        for (int i = 2; i < width - 1; i++)
            cars.Add(new Vector2Int(i, i - 1));
        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();

        manager.LoadLevel(jayPos, objects, cars, portals, 0);
    }

    private void LoadLevel2()
    {
        Vector2Int jayPos = new Vector2Int(0, 3);

        int width, height;
        width = 8;
        height = 6;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(2, 2));
        cars.Add(new Vector2Int(3, 2));
        cars.Add(new Vector2Int(5, 6));
        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();

        manager.LoadLevel(jayPos, objects, cars, portals, 0);
    }

    private void LoadLevel3()
    {
        Vector2Int jayPos = new Vector2Int(0, 3);

        int width, height;
        width = 10;
        height = 6;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }
        objects[2, 3] = GameElement.ElementType.Cone;
        objects[2, 2] = GameElement.ElementType.Cone;
        objects[4, 5] = GameElement.ElementType.Cone;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 4));
        cars.Add(new Vector2Int(7, 9));
        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();

        manager.LoadLevel(jayPos, objects, cars, portals, 0);
    }

    private void LoadLevel9() 
    {
        Vector2Int jayPos = new Vector2Int(0, 4);

        int width, height;
        width = 10;
        height = 8;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++){
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[2,3] = GameElement.ElementType.FanHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 2));
        cars.Add(new Vector2Int(6, 6));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        portals.Add((new Vector2Int(2, 1), new Vector2Int(4, 5)));

        manager.LoadLevel(jayPos, objects, cars, portals, 0);
    }

    private void LoadLevel10()
    {
        Vector2Int jayPos = new Vector2Int(0, 3);

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[7, 10];
        for (int i = 0; i < 7; i++)
        {
            objects[i, 9] = GameElement.ElementType.Flagpole;
            objects[i, 0] = GameElement.ElementType.Sidewalk;
        }
        List<Vector2Int> cars = new List<Vector2Int>();
        for (int i = 1; i < 9; i++)
        {
            objects[3, i] = GameElement.ElementType.Zebra;
            cars.Add(new Vector2Int(i, 1));
        }
        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();

        manager.LoadLevel(jayPos, Transpose(objects), cars, portals, 0);
    }

    // A quick, inefficent way to solve the rotated 90d rotation bugs
    public static GameElement.ElementType?[,] Transpose(GameElement.ElementType?[,] arr)
    {
        int rowCount = arr.GetLength(0);
        int columnCount = arr.GetLength(1);
        GameElement.ElementType?[,] transposed = new GameElement.ElementType?[columnCount, rowCount];
        if (rowCount == columnCount)
        {
            transposed = (GameElement.ElementType?[,])arr.Clone();
            for (int i = 1; i < rowCount; i++)
        {
                for (int j = 0; j < i; j++)
            {
                    GameElement.ElementType? temp = transposed[i, j];
                    transposed[i, j] = transposed[j, i];
                    transposed[j, i] = temp;
                }
            }
        }
        else
        {
            for (int column = 0; column < columnCount; column++)
            {
                for (int row = 0; row < rowCount; row++)
                {
                    transposed[column, row] = arr[row, column];
                }
            }
        }
        return transposed;
    }
}
