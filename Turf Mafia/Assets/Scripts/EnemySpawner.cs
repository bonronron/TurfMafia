using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] GameEvent spawnerDestroyed;

    public void takeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0) Destroy();
    }

    public void Destroy()
    {
        var gameManager = FindAnyObjectByType<GameManager>();
        gameManager.money += gameManager.gameData.moneyGeneratedBySpawnerDestroy;
        spawnerDestroyed.Notify();
        Destroy(gameObject);
    }
}
