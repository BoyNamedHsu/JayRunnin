﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameElement;
using cse481.logging;

public class Levels : MonoBehaviour
{
    public LevelManager manager;
    public CapstoneLogger logger;
    public static int LAST_LEVEL = 16; // for triggering ending cutscene. Please edit this

    // Start is called before the first frame update
    void Awake()
    {
        logger = LoggerController.LOGGER;
        Invoke("LoadLevel" + LevelSelector.levelChosen, 0f);
    }

    private void LoadLevel1() // first level of the game, simple
    {
        int width, height;
        width = 7;
        height = 6;

        Vector2Int jayPos = new Vector2Int(0, 3);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 2));
        cars.Add(new Vector2Int(5, 5));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();

        Vector2Int stars = new Vector2Int(0, 0);

        manager.LoadLevel(jayPos, objects, cars, portals, 0, stars);
        //logger.LogLevelEnd("ELevel_1_1");
    }

    private void LoadLevel2() // teaches mole-cops
    {
        int width, height;
        width = 8;
        height = 5;

        Vector2Int jayPos = new Vector2Int(0, 2);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[0, 1] = GameElement.ElementType.ConeWalk;
        objects[0, 3] = GameElement.ElementType.ConeWalk;
        for (int i = 1; i < 4; i++)
        {
            objects[i, 1] = GameElement.ElementType.Cone;
            objects[i, 3] = GameElement.ElementType.Cone;
        }
        objects[2, 2] = GameElement.ElementType.ManHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(4, 5));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();

        Vector2Int stars = new Vector2Int(0, 0);

        manager.LoadLevel(jayPos, objects, cars, portals, 1, stars);
    }

    private void LoadLevel3() // explores a level with 2 mole-cops
    {
        int width, height;
        width = 7;
        height = 6;

        Vector2Int jayPos = new Vector2Int(0, 3);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[2, 4] = GameElement.ElementType.ManHole;
        objects[4, 1] = GameElement.ElementType.ManHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 5));
        cars.Add(new Vector2Int(5, 10));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 2, stars);
    }

    private void LoadLevel4() // explores a level with a ton of mole cops (only need to kill 4)
    {
        int width, height;
        width = 8;
        height = 4;

        Vector2Int jayPos = new Vector2Int(0, 2);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }


        objects[1, 1] = GameElement.ElementType.Cone;
        objects[3, 2] = GameElement.ElementType.Cone;

        objects[2, 2] = GameElement.ElementType.ManHole;
        objects[3, 1] = GameElement.ElementType.ManHole;
        objects[1, 2] = GameElement.ElementType.ManHole;
        objects[2, 1] = GameElement.ElementType.ManHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(4, 7));
        cars.Add(new Vector2Int(5, 9));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 4, stars);
    }

    private void LoadLevel5() // teaches followers
    {
        int width, height;
        width = 9;
        height = 6;

        Vector2Int jayPos = new Vector2Int(0, 3);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }
        objects[0, 0] = GameElement.ElementType.ConeWalk;
        objects[0, 1] = GameElement.ElementType.ConeWalk;
        objects[0, 2] = GameElement.ElementType.ConeWalk;
        objects[0, 4] = GameElement.ElementType.ConeWalk;
        objects[1, 2] = GameElement.ElementType.Cone;
        objects[1, 4] = GameElement.ElementType.Cone;
        objects[2, 5] = GameElement.ElementType.Cone;

        objects[1, 3] = GameElement.ElementType.FanHole;
        objects[2, 1] = GameElement.ElementType.FanHole;
        objects[2, 3] = GameElement.ElementType.FanHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 6));
        cars.Add(new Vector2Int(4, 7));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(3, 2);
        manager.LoadLevel(jayPos, objects, cars, portals, 0, stars);
    }

    private void LoadLevel6() // teaches followers
    {
        int width, height;
        width = 9;
        height = 6;

        Vector2Int jayPos = new Vector2Int(0, 3);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }
        objects[0, 0] = GameElement.ElementType.ConeWalk;
        objects[0, 1] = GameElement.ElementType.ConeWalk;
        objects[0, 2] = GameElement.ElementType.ConeWalk;
        objects[0, 4] = GameElement.ElementType.ConeWalk;
        objects[1, 2] = GameElement.ElementType.Cone;
        objects[1, 4] = GameElement.ElementType.Cone;
        objects[2, 5] = GameElement.ElementType.Cone;

        objects[1, 3] = GameElement.ElementType.FanHole;
        objects[2, 1] = GameElement.ElementType.FanHole;
        objects[2, 3] = GameElement.ElementType.FanHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 6));
        cars.Add(new Vector2Int(4, 7));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(3, 2);
        manager.LoadLevel(jayPos, objects, cars, portals, 0, stars);
    }

    private void LoadLevel7() // teaches followers w/ cops (just movement)
    {
        int width, height;
        width = 8;
        height = 4;

        Vector2Int jayPos = new Vector2Int(0, 1);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }
        for (int i = 1; i < 4; i++)
        {
            objects[i, 0] = GameElement.ElementType.Cone;
            objects[i, 2] = GameElement.ElementType.Cone;
        }

        objects[5, 1] = GameElement.ElementType.Cone;

        objects[1, 1] = GameElement.ElementType.ManHole;
        objects[2, 1] = GameElement.ElementType.ManHole;
        objects[3, 1] = GameElement.ElementType.FanHole;
        objects[4, 1] = GameElement.ElementType.ManHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(4, 13));
        cars.Add(new Vector2Int(6, 13));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 3, stars);
    }

    private void LoadLevel8() // teaches followers w/ cops (make a single choice)
    {
        int width, height;
        width = 9;
        height = 5;

        Vector2Int jayPos = new Vector2Int(0, 2);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }
        objects[0, 1] = GameElement.ElementType.ConeWalk;
        objects[0, 3] = GameElement.ElementType.ConeWalk;
        objects[1, 2] = GameElement.ElementType.ManHole;

        objects[3, 0] = GameElement.ElementType.FanHole;
        objects[4, 0] = GameElement.ElementType.ManHole;
        objects[3, 1] = GameElement.ElementType.Cone;
        objects[4, 1] = GameElement.ElementType.Cone;

        objects[3, 4] = GameElement.ElementType.ManHole;
        objects[4, 4] = GameElement.ElementType.FanHole;
        objects[3, 3] = GameElement.ElementType.Cone;
        objects[4, 3] = GameElement.ElementType.Cone;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(6, 10));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 2, stars);
    }

    private void LoadLevel9() // many choices on this level
    {
        Vector2Int jayPos = new Vector2Int(0, 2);

        int width, height;
        width = 10;
        height = 5;

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[0, 1] = GameElement.ElementType.ConeWalk;
        objects[0, 3] = GameElement.ElementType.ConeWalk;

        objects[1, 1] = GameElement.ElementType.FanHole;
        objects[1, 3] = GameElement.ElementType.ManHole;

        objects[2, 2] = GameElement.ElementType.Cone;

        objects[3, 2] = GameElement.ElementType.ManHole;
        objects[3, 1] = GameElement.ElementType.ManHole;
        objects[3, 3] = GameElement.ElementType.FanHole;
        objects[2, 1] = GameElement.ElementType.Cone;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(4, 11));
        cars.Add(new Vector2Int(5, 11));
        cars.Add(new Vector2Int(7, 11));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 3, stars);
    }

    private void LoadLevel10() // teaches portals
    {
        int width, height;
        width = 9;
        height = 6;

        Vector2Int jayPos = new Vector2Int(0, 3);

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        for (int i = 0; i < height; i++)
        {
            objects[4, i] = GameElement.ElementType.Cone;
        }
        objects[1, 3] = GameElement.ElementType.ManHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(6, 7));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        portals.Add((new Vector2Int(2, 1), new Vector2Int(6, 4)));
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 1, stars);
    }

    private void LoadLevel11() // leap of faith into a portal.
    {
        Vector2Int jayPos = new Vector2Int(0, 1);

        int width, height;
        width = 6;
        height = 7;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[1, 1] = GameElement.ElementType.ManHole;
        objects[2, 1] = GameElement.ElementType.ManHole;
        objects[1, 2] = GameElement.ElementType.ManHole;
        objects[2, 2] = GameElement.ElementType.ManHole;

        objects[2, 4] = GameElement.ElementType.Cone;
        objects[4, 6] = GameElement.ElementType.Cone;
        objects[4, 4] = GameElement.ElementType.Cone;
        objects[2, 6] = GameElement.ElementType.Cone;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 9));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        portals.Add((new Vector2Int(1, 0), new Vector2Int(3, 5)));
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 4, stars);
    }

    private void LoadLevel12) // this one is cool, multiple solutions too.
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

        objects[2, 5] = GameElement.ElementType.Cone;
        objects[2, 1] = GameElement.ElementType.Cone;
        objects[6, 3] = GameElement.ElementType.Cone;

        objects[1, 2] = GameElement.ElementType.ManHole;
        objects[1, 3] = GameElement.ElementType.ManHole;
        objects[1, 4] = GameElement.ElementType.ManHole;
        objects[2, 3] = GameElement.ElementType.ManHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(3, 10));
        cars.Add(new Vector2Int(5, 11));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        portals.Add((new Vector2Int(3, 0), new Vector2Int(5, 4)));
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 4, stars);
    }

    private void LoadLevel13() // this level is a bit too hard lmao
    {
        Vector2Int jayPos = new Vector2Int(0, 3);

        int width, height;
        width = 8;
        height = 7;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[0, 2] = GameElement.ElementType.ConeWalk;
        objects[0, 4] = GameElement.ElementType.ConeWalk;
        objects[1, 2] = GameElement.ElementType.Cone;
        objects[1, 4] = GameElement.ElementType.Cone;
        objects[3, 1] = GameElement.ElementType.Cone;
        objects[5, 1] = GameElement.ElementType.Cone;
        objects[5, 3] = GameElement.ElementType.Cone;
        objects[5, 5] = GameElement.ElementType.Cone;
        objects[6, 1] = GameElement.ElementType.Cone;

        objects[2, 2] = GameElement.ElementType.FanHole;
        objects[2, 3] = GameElement.ElementType.FanHole;
        objects[2, 4] = GameElement.ElementType.FanHole;

        objects[1, 3] = GameElement.ElementType.ManHole;
        objects[3, 5] = GameElement.ElementType.ManHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(4, 16));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        portals.Add((new Vector2Int(6, 0), new Vector2Int(6, 4)));
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 2, stars);
    }

    private void LoadLevel20() // this level is a bit too hard lmao
    {
        Vector2Int jayPos = new Vector2Int(0, 2);

        int width, height;
        width = 11;
        height = 7;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[0, 3] = GameElement.ElementType.ConeWalk;
        objects[0, 1] = GameElement.ElementType.ConeWalk;
        objects[0, 0] = GameElement.ElementType.ConeWalk;

        objects[1, 6] = GameElement.ElementType.ManHole;
        objects[1, 4] = GameElement.ElementType.FanHole;
        objects[1, 3] = GameElement.ElementType.Cone;
        objects[1, 1] = GameElement.ElementType.Cone;

        objects[2, 3] = GameElement.ElementType.Cone;
        objects[2, 1] = GameElement.ElementType.Cone;

        objects[3, 6] = GameElement.ElementType.Cone;
        objects[3, 4] = GameElement.ElementType.Cone;
        objects[3, 2] = GameElement.ElementType.Cone;
        objects[3, 1] = GameElement.ElementType.Cone;

        objects[4, 6] = GameElement.ElementType.Cone;
        objects[4, 4] = GameElement.ElementType.Cone;
        objects[4, 2] = GameElement.ElementType.Cone;

        objects[5, 6] = GameElement.ElementType.Cone;
        objects[5, 4] = GameElement.ElementType.Cone;
        objects[5, 0] = GameElement.ElementType.FanHole;
        objects[5, 1] = GameElement.ElementType.ManHole;
        objects[5, 2] = GameElement.ElementType.Cone;

        objects[6, 6] = GameElement.ElementType.Cone;
        objects[6, 4] = GameElement.ElementType.Cone;
        objects[6, 2] = GameElement.ElementType.Cone;

        objects[7, 6] = GameElement.ElementType.Cone;
        objects[7, 4] = GameElement.ElementType.Cone;
        objects[7, 2] = GameElement.ElementType.Cone;
        objects[7, 1] = GameElement.ElementType.FanHole;
        objects[7, 0] = GameElement.ElementType.ManHole;
    
        objects[8, 6] = GameElement.ElementType.Cone;
        objects[8, 4] = GameElement.ElementType.Cone;
        objects[8, 2] = GameElement.ElementType.Cone;


        objects[9, 4] = GameElement.ElementType.Cone;
        objects[9, 2] = GameElement.ElementType.Cone;
        objects[9, 6] = GameElement.ElementType.Cone;


        objects[10, 5] = GameElement.ElementType.ConeWalk;
        objects[10, 6] = GameElement.ElementType.ConeWalk ;

        objects[10, 1] = GameElement.ElementType.ConeWalk;
        objects[10, 0] = GameElement.ElementType.ConeWalk;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(4, 35));
        cars.Add(new Vector2Int(7, 35));
        cars.Add(new Vector2Int(9, 35));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        portals.Add((new Vector2Int(2, 2), new Vector2Int(1, 0)));
        portals.Add((new Vector2Int(9, 0), new Vector2Int(9, 5)));
        portals.Add((new Vector2Int(1, 5), new Vector2Int(3, 3)));
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 3, stars);
    }

    private void LoadLevel21() // this level is a bit too hard lmao
    {
        Vector2Int jayPos = new Vector2Int(0, 0);

        int width, height;
        width = 11;
        height = 7;

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[1, 6] = GameElement.ElementType.ManHole;
        objects[1, 5] = GameElement.ElementType.Cone;
        objects[1, 4] = GameElement.ElementType.Cone;
        objects[1, 3] = GameElement.ElementType.Cone;
        objects[1, 2] = GameElement.ElementType.Cone;
        objects[1, 1] = GameElement.ElementType.Cone;
        objects[1, 0] = GameElement.ElementType.Cone;

        objects[2, 6] = GameElement.ElementType.ManHole;
        objects[2, 0] = GameElement.ElementType.Cone;
        objects[2, 4] = GameElement.ElementType.Cone;

        objects[3, 6] = GameElement.ElementType.FanHole;
        objects[3, 5] = GameElement.ElementType.ManHole;
        objects[3, 4] = GameElement.ElementType.Cone;
        objects[3, 2] = GameElement.ElementType.Cone;

        objects[4, 0] = GameElement.ElementType.ManHole;
        objects[4, 1] = GameElement.ElementType.FanHole;
        objects[4, 2] = GameElement.ElementType.Cone;
        objects[4, 4] = GameElement.ElementType.ManHole;

        objects[5, 6] = GameElement.ElementType.Cone;
        objects[5, 5] = GameElement.ElementType.Cone;
        objects[5, 4] = GameElement.ElementType.Cone;
        objects[5, 3] = GameElement.ElementType.Cone;
        objects[5, 2] = GameElement.ElementType.Cone;


        objects[6, 2] = GameElement.ElementType.Cone;
        objects[6, 0] = GameElement.ElementType.Cone;

        objects[7, 2] = GameElement.ElementType.Cone;

        objects[8, 1] = GameElement.ElementType.Cone;

        objects[9, 1] = GameElement.ElementType.Cone;
        objects[10, 1] = GameElement.ElementType.ConeWalk;


        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(2, 20));
        cars.Add(new Vector2Int(7, 29));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 5, stars);
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


    private void LoadLevel102() // teaches crosswalks
    {
        int width, height;
        width = 8;
        height = 8;

        Vector2Int jayPos = new Vector2Int(1, 4);

        // NOTE: This array is rotated 90d counterclockwise before being loaded in
        // This has to do with how arrays are indexed, and uh can be fixed later
        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[1, 4] = GameElement.ElementType.Zebra;
        objects[2, 4] = GameElement.ElementType.Zebra;
        objects[2, 3] = GameElement.ElementType.Zebra;
        objects[3, 3] = GameElement.ElementType.Zebra;
        objects[4, 3] = GameElement.ElementType.Zebra;
        objects[5, 3] = GameElement.ElementType.Zebra;
        objects[5, 4] = GameElement.ElementType.Zebra;
        objects[6, 4] = GameElement.ElementType.Zebra;

        objects[0, 5] = GameElement.ElementType.ConeWalk;
        objects[0, 4] = GameElement.ElementType.ConeWalk;
        objects[0, 3] = GameElement.ElementType.ConeWalk;

        List<Vector2Int> cars = new List<Vector2Int>();
        for (int i = 1; i < 7; i++)
        {
            cars.Add(new Vector2Int(i, 2));
        }

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 0, stars);
    }

    private void LoadLevel108() // many choices on this level
    {
        Vector2Int jayPos = new Vector2Int(0, 2);

        int width, height;
        width = 10;
        height = 5;

        GameElement.ElementType?[,] objects = new GameElement.ElementType?[width, height];
        for (int i = 0; i < height; i++)
        {
            objects[0, i] = GameElement.ElementType.Sidewalk;
            objects[width - 1, i] = GameElement.ElementType.Flagpole;
        }

        objects[0, 1] = GameElement.ElementType.ConeWalk;
        objects[0, 3] = GameElement.ElementType.ConeWalk;

        objects[1, 1] = GameElement.ElementType.FanHole;
        objects[1, 2] = GameElement.ElementType.ManHole;
        objects[1, 3] = GameElement.ElementType.FanHole;
        objects[2, 1] = GameElement.ElementType.ManHole;
        objects[2, 2] = GameElement.ElementType.FanHole;
        objects[2, 3] = GameElement.ElementType.ManHole;

        List<Vector2Int> cars = new List<Vector2Int>();
        cars.Add(new Vector2Int(4, 14));
        cars.Add(new Vector2Int(5, 14));
        cars.Add(new Vector2Int(7, 14));

        List<(Vector2Int, Vector2Int)> portals = new List<(Vector2Int, Vector2Int)>();
        Vector2Int stars = new Vector2Int(0, 0);
        manager.LoadLevel(jayPos, objects, cars, portals, 3, stars);
    }
}