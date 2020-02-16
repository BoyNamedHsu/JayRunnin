using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Overworld : MonoBehaviour
{
    private Tile[,] gridworld;
    public int height;
    public int width;
    public TextMeshProUGUI ian;

    // Start is called before the first frame update
    void Start()
    {
        gridworld = new Tile[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                gridworld[i, j] = new Tile();

        gridworld[GameManager.playerStart.x, GameManager.playerStart.y].character = 'c';
        for (int i = 1; i < GameManager.followers.Count(); i++)
        {
            gridworld[GameManager.followers[i].position.x, 
                        GameManager.followers[i].position.y].character = 'f';
        }
        ian.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        ian.text = toString();
    }

    public string toString()
    {
        string tess = "";
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tess += gridworld[i, j].character;
            }
            tess += Environment.NewLine;
        }
        return tess;
    }

    public bool Move((int x, int y) prev, (int x, int y) current, char character)
    {
        if (TileOccupied(current))
        {
            gridworld[prev.x, prev.y].character = '0';
            gridworld[current.x, current.y].character = character;
            return true;
        }
        return false;
    }

    public void kill(int col) {
        for (int x = 0; x < height; x++) {
            if (gridworld[x, col].character != '0') {
                gridworld[x, col].character = '0';
                for (int i = 0; i < GameManager.followers.Count; i++)
                {
                    if (GameManager.followers[i].position == (x, col)) {
                        GameManager.followers.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    public Tile[,] getGrid() {
        return gridworld;
    }

    // Returns whether or not coords is occupied
    public bool TileOccupied((int x, int y) coords)
    {
        return gridworld[coords.x, coords.y].character == '0';
    }
}
