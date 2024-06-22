using AquariusMax.Ancient;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public enum CurrentState { idle, trace, attack, dead };
    public CurrentState curState = CurrentState.idle;

    private Transform _transform;
    private Transform enemyTransform;
    private Transform playerTransform; // 플레이어의 Transform
    private NavMeshAgent nvAgent;
    private Animator _animator;

    public float traceDist = 15.0f;
    public float attackDist = 1.2f;
    public int hp = 10;
    private bool isDead = false;
    public int attackPower = 1;

    private float attackInterval = 3f; // 공격 간격 설정
    private float attackTimer = 0f; // 공격 타이머

    void Start()
    {
        _transform = transform;
        enemyTransform = FindNearestEnemy();
        nvAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        // 플레이어의 Transform 찾기 (예: "Player" 태그를 가진 오브젝트를 플레이어로 가정)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        StartCoroutine(CheckState());
        StartCoroutine(CheckStateForAction());
    }

    void Update()
    {
        // Update 함수는 사용하지 않음
    }

    IEnumerator CheckState()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.2f);

            enemyTransform = FindNearestEnemy();
            if (enemyTransform == null)
            {
                // 플레이어를 추적할 수 있는지 여부를 결정하기 위한 추가 로직
                if (playerTransform != null)
                {
                    float playerDist = Vector3.Distance(playerTransform.position, _transform.position);
                    if (playerDist <= traceDist)
                    {
                        enemyTransform = playerTransform;
                        curState = CurrentState.trace;
                    }
                    else
                    {
                        curState = CurrentState.idle;
                    }
                }
                else
                {
                    curState = CurrentState.idle;
                }
            }
            else
            {
                float dist = Vector3.Distance(enemyTransform.position, _transform.position);

                if (dist <= attackDist)
                {
                    curState = CurrentState.attack;
                }
                else if (dist <= traceDist)
                {
                    curState = CurrentState.trace;
                }
                else
                {
                    curState = CurrentState.idle;
                }
            }
        }
    }

    IEnumerator CheckStateForAction()
    {
        while (!isDead)
        {
            switch (curState)
            {
                case CurrentState.idle:
                    nvAgent.isStopped = true;
                    _animator.SetBool("IsTrace", false);
                    _animator.SetBool("IsAttack", false);
                    break;

                case CurrentState.trace:
                    if (enemyTransform != null)
                    {
                        nvAgent.destination = enemyTransform.position;
                        nvAgent.isStopped = false;
                        _animator.SetBool("IsTrace", true);
                        _animator.SetBool("IsAttack", false);
                    }
                    break;

                case CurrentState.attack:
                    if (enemyTransform != null)
                    {
                        nvAgent.isStopped = true;
                        _animator.SetBool("IsTrace", false);
                        _animator.SetBool("IsAttack", true);

                        // Attack with interval
                        if (Time.time >= attackTimer)
                        {
                            AttackEnemy();
                            attackTimer = Time.time + attackInterval;
                        }
                    }
                    break;
            }
            yield return null;
        }
    }

    void Die()
    {
        isDead = true;
        curState = CurrentState.dead;
        nvAgent.isStopped = true;
        _animator.SetBool("IsTrace", false);
        _animator.SetBool("IsAttack", false);
        _animator.SetTrigger("IsDead");

        // Additional death logic if needed
    }

    void AttackEnemy()
    {
        // Implement attack logic here, such as dealing damage to the enemy
        if (enemyTransform != null)
        {
            Monster enemy = enemyTransform.GetComponent<Monster>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackPower);
            }
        }
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearestEnemy = null;
        float minDist = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.transform.position, _transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            hp -= damage;
            StartCoroutine(DelayedHitAnimation());
            if (hp <= 0)
            {
                Die();
            }
        }
    }

    IEnumerator DelayedHitAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.SetTrigger("IsHit");
    }

    public int GetHP()
    {
        return hp;
    }
}
