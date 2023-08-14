using System.Collections;
using UnityEngine;
public static class Constants
{
    public static int GetRandomDamage()
    {
        return Random.Range(0, 10);
    }

    public static float GetRandomWait()
    {
        return Random.Range(0, 5);
    }
}