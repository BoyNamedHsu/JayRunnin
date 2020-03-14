using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector
{
    public static int levelChosen;
    public static int[] maxRetries = new int[Levels.LAST_LEVEL + 1];
    public static int currentRetries;
}
