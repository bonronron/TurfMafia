using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTurf : MonoBehaviour
{
    [SerializeField] private TowerData homeTurfData;
    [SerializeReference] GameManager gameManager;
    [SerializeField] private int health;

    [SerializeField] GameEvent LoseGame;

    private void Start()
    {
        health = homeTurfData.health;
    }


    public void takeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0) EndGame();
    }


    public void EndGame()
    {
        LoseGame.Notify();
        Destroy(gameObject);
    }
}
