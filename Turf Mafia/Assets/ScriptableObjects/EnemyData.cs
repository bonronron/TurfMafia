using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float speed;
    public float attackSpeed;
    public int attack;
    public EnemyType type;

}

    /// <summary>
    /// 1) Thug: Walk avg, avg damage, low health
    /// 2) Hitman: Walk fast, high damage, Avg health
    /// 3) Soldier: Walk slow, very high damage, high health
    /// </summary>
public enum EnemyType
{
    Thug, // Walk avg, avg damage, low health
    Hitman, // Walk fast, high damage, Avg health
    Soldier, // Walk slow, very high damage, high health
    General // Does nothing
}