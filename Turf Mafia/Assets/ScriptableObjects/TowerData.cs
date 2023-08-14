using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerData : ScriptableObject
{
    public float range = 10f;
    public int health = 50;
    public float fireInterval = 5f;  //Time between firing
    public TowerType type = TowerType.General;
}

/**
 * <summary>
   1) TommyGunTurret : Shoots enemies which come in its range.
   2) SpeakeasyBar : Stuns enemies in place for a while.
   3) BribeMaster : Makes enemies switch sides and join the player.
  </summary>
 */
public enum TowerType
{
    TommyGunTurret,// Shoots enemies which come in its range
    SpeakeasyBar, // Stuns enemies in place for a while.
    BribeMaster, // Makes enemies switch sides and join the player.
    General // Does nothing
}