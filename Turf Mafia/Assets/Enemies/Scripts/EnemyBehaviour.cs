using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyData enemyData;
    [SerializeReference] EnemyMovement movement;
    [SerializeField] private float health;
    NavMeshAgent agentNavigation;
    [SerializeField] private EnemyState _behaviourState;
    [SerializeField] GameManager gameManager;
    [SerializeField] Animator _animator;
    public EnemyState behaviourState
    {
        get => _behaviourState;
        set
        {
            _behaviourState = value;
            OnBehaviourStateChanged(value);
        }
    }
    private Transform player;
    private Transform homeTurf;
    [SerializeField] private GameObject nearestSpawner;
    public bool isEnemy;
    void Start()
    {
        agentNavigation = GetComponent<NavMeshAgent>();
        health = enemyData.maxHealth;
        isEnemy = true;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        behaviourState = EnemyState.MovingToHomeTurf;
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (!isEnemy)
        {
            nearestSpawner = GameObject.FindGameObjectsWithTag("EnemySpawner")
                .Select(t => t.transform)
                .OrderBy(t => (t.position - transform.position).sqrMagnitude).FirstOrDefault().gameObject;
        }
    }

    private void Update()
    {
        switch (behaviourState)
        {
            case EnemyState.MovingToHomeTurf :
                _animator.CrossFade("Walk", 0.2f);
                break;
            case EnemyState.ShootingHomeTurf:
                _animator.CrossFade("Kick", 0.2f);
                break;
            case EnemyState.MovingToPlayer:
                _animator.CrossFade("Walk", 0.2f);
                break;
            case EnemyState.ShootingEnemySpawners:
                _animator.CrossFade("Kick", 0.2f);
                break;
            default:
                break;
        }
    }
    void OnBehaviourStateChanged(EnemyState value)
    {
        switch (value)
        {
            case EnemyState.MovingToHomeTurf:
                goToHomeTurf();
                break;
            case EnemyState.ShootingHomeTurf:
                shootAtTarget();
                break;
            case EnemyState.MovingToPlayer:
                followPlayer();
                break;
            case EnemyState.ShootingEnemySpawners:
                shootAtTarget();
                break;
            default:
                break;
        }
    }

    public void takeDamage(float dmg) 
    {
        health -= dmg;
        if (health < 0)
        {
            gameManager.EnemiesInScene--;
            gameManager.money += gameManager.gameData.moneyGeneratedByKill[(int)enemyData.type];
            Destroy(gameObject);
        }
    }

    public void changeTeam()
    {
        isEnemy = false;
        behaviourState = EnemyState.MovingToPlayer;
    }

    public void getSlowed(float time)
    {
        StartCoroutine(returnSpeed(time, agentNavigation.speed));
        agentNavigation.speed -= agentNavigation.speed * 0.6f;
    }
    IEnumerator returnSpeed(float time, float speed)
    {
        yield return new WaitForSeconds(time);
        if (gameObject) agentNavigation.speed = speed; 
    }
    

    void goToHomeTurf()
    {

        homeTurf = FindAnyObjectByType<HomeTurf>().transform;
        movement.WalkToLocation(homeTurf);
    }
    
    void followPlayer()
    {
        StartCoroutine(walkWithPlayer());
    }

    void shootAtTarget()
    {
        if (isEnemy)
        {
            StartCoroutine(shootAtHomeTurf());
        }
        else
        {
            StartCoroutine(shootAtSpawner());
        }
    }

    IEnumerator shootAtSpawner()
    {
        while (true)
        {
            if (behaviourState != EnemyState.ShootingEnemySpawners) break;
            yield return new WaitForSeconds(enemyData.attackSpeed);
            nearestSpawner.GetComponent<EnemySpawner>().takeDamage(enemyData.attack);
            if (Vector3.Distance(transform.position, player.position) > agentNavigation.stoppingDistance) behaviourState = EnemyState.MovingToPlayer;
        }
    }

    IEnumerator shootAtHomeTurf()
    {
        while (true)
        {
            if (behaviourState != EnemyState.ShootingHomeTurf) break;
            yield return new WaitForSeconds(enemyData.attackSpeed);
            if(homeTurf) homeTurf.GetComponent<HomeTurf>().takeDamage(enemyData.attack);
        }
    }
    IEnumerator walkWithPlayer()
    {
        while (true)
        {
            if (behaviourState != EnemyState.MovingToPlayer) break;
            yield return new WaitForSeconds(2f);
            movement.WalkToLocation(player);
        }
    }
}

public enum EnemyState
{
    MovingToHomeTurf,
    ShootingHomeTurf,
    MovingToPlayer,
    ShootingEnemySpawners
}