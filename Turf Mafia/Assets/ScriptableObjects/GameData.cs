using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="GameData")]
public class GameData : ScriptableObject
{
    public int maxTowers;
    public int maxEnemies;
    public int towersToDestroyForWin;
    public int moneyGeneratedByHome;
    public int moneyGeneratedBySpawnerDestroy;
    public List<int> moneyGeneratedByKill;
    public List<float> enemyFrequency;
    public List<int> towerCosts;
}
