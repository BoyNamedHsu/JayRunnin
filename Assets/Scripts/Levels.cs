﻿using System.Collections;
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

    private void LoadLevel101() 
    {
        Vector2Int jayPos = new Vector2Int(0, 3);

        int width, height;
        width = 8;
        height = 6;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++){
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[2, 5] = GameElement.ElementType.Cone;
        objects[2, 1] = GameElement.ElementType.Cone;
        objects[6, 3] = GameElement.ElementType.Cone;

        objects[1,2] = GameElement.ElementType.ManHole;
        objects[1,3] = GameElement.ElementType.ManHole;
        objects[1,4] = GameElement.ElementType.ManHole;
        objects[2,3] = GameElement.ElementType.ManHole;

        objects[1,0] = GameElement.ElementType.Zebra;
        objects[2,0] = GameElement.ElementType.Zebra;
        objects[3,0] = GameElement.ElementType.Zebra;
        objects[4,0] = GameElement.ElementType.Zebra;
        objects[5,0] = GameElement.ElementType.Zebra;
        objects[6,0] = GameElement.ElementType.Zebra;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 10));
        cars.Add(new Vector2Int(5, 11));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        portals.Add((new Vector2Int(3, 1), new Vector2Int(5, 4)));

        manager.LoadLevel(jayPos, objects, cars, portals, 4);
    }

    private void LoadLevel102() // this level is a bit too hard lmao
    {
        Vector2Int jayPos = new Vector2Int(0, 3);

        int width, height;
        width = 8;
        height = 7;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++){
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[0, 2] = GameElement.ElementType.Cone;
        objects[0, 4] = GameElement.ElementType.Cone;
        objects[1, 2] = GameElement.ElementType.Cone;
        objects[1, 4] = GameElement.ElementType.Cone;
        objects[3, 1] = GameElement.ElementType.Cone;
        objects[5, 1] = GameElement.ElementType.Cone;
        objects[5, 3] = GameElement.ElementType.Cone;
        objects[5, 5] = GameElement.ElementType.Cone;

        objects[2,2] = GameElement.ElementType.FanHole;
        objects[2,3] = GameElement.ElementType.FanHole;
        objects[2,4] = GameElement.ElementType.FanHole;

        objects[1,3] = GameElement.ElementType.ManHole;
        objects[3,5] = GameElement.ElementType.ManHole;

        objects[3,2] = GameElement.ElementType.Zebra;
        objects[3,3] = GameElement.ElementType.Zebra;
        objects[3,4] = GameElement.ElementType.Zebra;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(4, 15));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        portals.Add((new Vector2Int(6, 0), new Vector2Int(6, 4)));

        manager.LoadLevel(jayPos, objects, cars, portals, 2);
    }

    private void LoadLevel2()
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

        List<Vector2Int> cars = new List<Vector2Int>();
        for (int i = 2; i < 9; i++)
            cars.Add(new Vector2Int(i, i - 1));
        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();

        manager.LoadLevel(jayPos, objects, cars, portals, 0);
    }

    private void LoadLevel3()
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
