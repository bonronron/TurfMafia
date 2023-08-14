using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] public GameData gameData;
    [SerializeField] List<GameData> gameDatas;

    [Header("Variables")]
    [SerializeField] public bool hasPlacedHomeTurf;
    [SerializeField] public bool areEnemiesSpawning;
    [SerializeField] private int _EnemiesInScene; 
    [SerializeField] private int _TowersInScene;
    [SerializeField] private int _SpawnersDestroyed;
    [SerializeField] public int money;
    
    public int EnemiesInScene { get => _EnemiesInScene; set { _EnemiesInScene = value; EnemiesValueChanged(); } }
    public int TowersInScene { get => _TowersInScene; set { _TowersInScene = value; TowersValueChanged(); } }
    public int SpawnersDestroyed { get => _SpawnersDestroyed; set { _SpawnersDestroyed = value; SpawnersValueChanged(value); } }


    [Header("Events")]
    [SerializeField] GameEvent WinGame;
    [SerializeField] GameEvent EnemiesLimitReached;

    private void TowersValueChanged()
    {
        
    }

    private void SpawnersValueChanged(int value)
    {
        if(value >= gameData.towersToDestroyForWin)
        {
            WinGame.Notify();
        }
    }
    private void EnemiesValueChanged()
    {
        if (isMaxSpawnLimitReach() && areEnemiesSpawning)
        {
            areEnemiesSpawning = false;
            EnemiesLimitReached.Notify();
        }
        else if(!areEnemiesSpawning && !isMaxSpawnLimitReach())
        {
            areEnemiesSpawning = true;
            EnemiesLimitReached.Notify();
        }
    }

    private void OnEnable()
    {
        switch (MainMenuUI.selectedLevel)
        {
            case Difficulty.Easy:
                gameData = gameDatas[0];
                break;
            case Difficulty.Normal:
                gameData = gameDatas[1];
                break;
            case Difficulty.Hard:
                gameData = gameDatas[2];
                break;
            case Difficulty.Extreme:
                gameData = gameDatas[4];
                break;
            default:
                gameData = gameDatas[1];
                break;
        }
    }
    private void Start()
    {
        areEnemiesSpawning = false;
        money = 0;
    }

    public bool isMaxSpawnLimitReach() { return EnemiesInScene >= gameData.maxEnemies; }

    public void placedHomeTurf()
    {
        hasPlacedHomeTurf = true;
        areEnemiesSpawning = true;
        StartCoroutine(generateMoney());
    }

    public void AddTower() { 
        TowersInScene++; 
    }
    public void AddSpawnerDestroyed() { 
        SpawnersDestroyed++; 
        if(SpawnersDestroyed >= gameData.towersToDestroyForWin)
        {
            WinGame.Notify();
            StopAllCoroutines();
        }
    }

    public bool canBuy(TowerType tower)
    {
        var cost = gameData.towerCosts[(int)tower];
        return cost < money;
    }
    public void boughtTower(TowerType tower)
    {
        var cost = gameData.towerCosts[(int)tower];
        money-=cost;
    }
    IEnumerator generateMoney()
    {
        while (true)
        {
            yield return new WaitForSeconds(Constants.GetRandomWait());
            money += gameData.moneyGeneratedByHome;
        }
    }

}
