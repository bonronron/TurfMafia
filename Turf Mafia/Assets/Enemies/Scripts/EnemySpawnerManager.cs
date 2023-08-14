using System;
using System.Collections;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    EnemySpawner[] enemySpawners;
    [SerializeField]Transform Player;
    void Start()
    {
        Invoke("SpawnersCheck", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnersCheck()
    {
        enemySpawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        if(enemySpawners.Length > 15)
        {

            StartCoroutine(CalculateDistanceAndDestroy());

        }else if(enemySpawners.Length<2 && enemySpawners.Length > 0)
        {



        }
    }

    private IEnumerator CalculateDistanceAndDestroy()
    {
        yield return null;
        var distanceFromSpawner = new float[enemySpawners.Length];
        float averageDis= 0;
        for(var i=0; i < enemySpawners.Length; i++)
        {
            distanceFromSpawner[i] = Vector3.SqrMagnitude(Player.position - enemySpawners[i].transform.position);
            averageDis += distanceFromSpawner[i];
        }
        averageDis = averageDis / distanceFromSpawner.Length;
        for (var i = 0; i < distanceFromSpawner.Length; i++)
        {
            if (distanceFromSpawner[i] > averageDis) Destroy(enemySpawners[i].gameObject);
            Debug.Log("Destroyed 1");
        }
    }
}
