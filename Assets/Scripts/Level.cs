using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Overworld grid;
    private Jay player;
    private static List<Follower> followers;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Overworld(8, 10);
        player = new Jay(0, 4);
        followers = new List<Follower> {
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
