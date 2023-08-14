using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawning : MonoBehaviour
{
    private bool isSpawnSet;
    private Vector3 spawnPointOnNavMesh;
    [SerializeField] float spawnRange;
    [SerializeField] GameObject enemyToSpawn;
    [SerializeField] GameManager gameManager;
    [SerializeField] private bool _isSpawning;
    [SerializeField] List<GameObject> enemies;
    bool isSpawning { 
        get => _isSpawning; 
        set {
            _isSpawning = value;
            SpawningChanged(value);
        } 
    }



    private void Start()
    {
        isSpawning = false;
        isSpawnSet = false;
        gameManager = FindAnyObjectByType<GameManager>();
        //Debug.Log("Startfunction checking for spawnpoint");
    }

    private void OnEnable()
    {
        isSpawning = true;
    }
    private void OnDisable()
    {
        isSpawning = false;
    }
    private void SpawningChanged(bool value)
    {
        if (value)
        {
            StartCoroutine(spawnEnemy());
        }
        else
            StopCoroutine(spawnEnemy());
    }

    public void TurnOnSpawns()
    {
        StartCoroutine(checkForSpawnPoint());
        isSpawning = true;
    }
    public void SwitchEnemySpawns()
    {
        isSpawning = !gameManager.isMaxSpawnLimitReach();
    }
    IEnumerator checkForSpawnPoint()
    {
        //Debug.Log("Started checking for spawnpoint");
        if (isSpawnSet) yield break;
        //Debug.Log("didn't break out");
        while (true)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(transform.position, out hit, spawnRange, NavMesh.AllAreas);
            //Debug.Log(hit.position);
            if (Vector3.Distance(transform.position, hit.position) < 50f)
            {
                spawnPointOnNavMesh = hit.position;
                isSpawnSet = true;
                StopCoroutine(checkForSpawnPoint());
            }
            yield return new WaitForSeconds(3f);
        }
    }
    private IEnumerator spawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            if (isSpawnSet && isSpawning && !gameManager.isMaxSpawnLimitReach() && gameManager.hasPlacedHomeTurf)
            {
                enemyToSpawn = selectEnemy(enemies);
                Instantiate(enemyToSpawn, spawnPointOnNavMesh, Quaternion.identity, transform);
                gameManager.EnemiesInScene++;
            }
        }
    }

    public GameObject selectEnemy(List<GameObject> enemies)
    {
        float rand = Random.value;
        for(var i = 0; i < enemies.Count; i++)
        {
            //Debug.Log("Random No. :" + rand + "<=" + gameManager.gameData.enemyFrequency[i] + "  Selected: " + (rand <= gameManager.gameData.enemyFrequency[i])+ enemies[i].name);
            if (rand <= gameManager.gameData.enemyFrequency[i]) return enemies[i];
        }
        return enemies[enemies.Count - 1];
    }
}
