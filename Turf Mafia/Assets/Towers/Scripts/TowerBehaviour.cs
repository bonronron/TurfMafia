using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    [SerializeField] TowerData towerData;
    [SerializeField] List<GameObject> currentTargets;

    private void OnEnable()
    {
        StartCoroutine(DetectEnemiesInRange());
        currentTargets = new List<GameObject>();
    }

    private void OnDisable()
    {
        StopCoroutine(DetectEnemiesInRange());
    }
    private IEnumerator DetectEnemiesInRange()
    {
        while (true)
        {
            yield return new WaitForSeconds(towerData.fireInterval);
            currentTargets.Clear();
            Collider[] colliders = Physics.OverlapSphere(transform.position, towerData.range);
            foreach (Collider collider in colliders)
            {
                Debug.Log(collider.tag);
                if (collider.CompareTag("Enemy"))
                {
                    if (collider.gameObject.GetComponent<EnemyBehaviour>().isEnemy) currentTargets.Add(collider.gameObject);
                }
            }
            switch (towerData.type)
            {
                case TowerType.General:
                    GeneralShooting();
                    break;
                case TowerType.BribeMaster:
                    ChangeTeam();
                    break;
                case TowerType.SpeakeasyBar:
                    GetDrunkAtBar();
                    break;
                case TowerType.TommyGunTurret:
                    RandomShooting();
                    break;
                default:
                    Debug.LogWarning("TowerType not matching");
                    break;
            }
        }
    }

    void GeneralShooting()
    {
        foreach(GameObject enemy in currentTargets)
        {
            enemy.GetComponent<EnemyBehaviour>().takeDamage(10);
        }
    }
    void RandomShooting()
    {
        if (currentTargets.Count == 0) return;

            var enemy = currentTargets[Random.Range(0, currentTargets.Count)];
            if(enemy) enemy.GetComponent<EnemyBehaviour>().takeDamage(10);
    }

    void GetDrunkAtBar()
    {
        foreach (GameObject enemy in currentTargets)
        {
            enemy.GetComponent<EnemyBehaviour>().getSlowed(towerData.fireInterval);
        }
    }

    void ChangeTeam()
    {
        if (currentTargets.Count == 0) return;
        var enemy = currentTargets[Random.Range(0, currentTargets.Count)];
        enemy.GetComponent<EnemyBehaviour>().changeTeam();
    }



    #region Debugging
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, towerData.range);
    }
    #endregion
}
