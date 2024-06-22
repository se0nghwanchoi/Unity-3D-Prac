using AquariusMax.Ancient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    public enum CurrentState { idle, trace, attack, dead };
    public CurrentState curState = CurrentState.idle;

    private Transform _transform;
    private Transform playerTransform;
    private NavMeshAgent nvAgent;
    private Animator _animator;

    public float traceDist = 15.0f;
    public float attackDist = 1.2f;
    public int hp = 10; // 몬스터의 체력
    private bool isHit = false; // 몬스터가 공격을 받았는지 여부

    private bool isDead = false;
    private MobAttack m_attackArea = null;
    public int attackPower = 1;

    

    void Start()
    {
        _transform = this.gameObject.GetComponent<Transform>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        _animator = this.gameObject.GetComponent<Animator>();
        m_attackArea = GetComponentInChildren<MobAttack>();

        StartCoroutine(this.CheckState());
        StartCoroutine(this.CheckStateForAction());

    }

    void Update()
    {
        /*if (hp <= 0 && !isDead)
        {
            Die();
        }*/
    }

    IEnumerator CheckState()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTransform.position, _transform.position);

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

    IEnumerator CheckStateForAction()
    {
        while (!isDead)
        {
            switch (curState)
            {
                case CurrentState.idle:
                    nvAgent.isStopped = true; // 에이전트를 멈추기 위해 isStopped 속성을 true로 설정
                    _animator.SetBool("IsTrace", false);
                    _animator.SetBool("IsAttack", false);
                    break;

                case CurrentState.trace:
                    nvAgent.destination = playerTransform.position;
                    nvAgent.isStopped = false; // 에이전트를 재개하기 위해 isStopped 속성을 false로 설정
                    _animator.SetBool("IsTrace", true);
                    _animator.SetBool("IsAttack", false);
                    break;

                case CurrentState.attack:
                    nvAgent.isStopped = true; // 공격 상태에서는 에이전트를 멈추기 위해 isStopped 속성을 true로 설정
                    _animator.SetBool("IsTrace", false);
                    _animator.SetBool("IsAttack", true);
                    break;

            }
            yield return null;
        }
    }

    void Die()
    {
        isDead = true; // 몬스터가 죽었음을 표시
        curState = CurrentState.dead; // 상태를 죽음으로 변경
        nvAgent.isStopped = true; // 네브메쉬 에이전트 정지
        _animator.SetBool("IsTrace", false);
        _animator.SetBool("IsAttack", false);
        _animator.SetTrigger("IsDead"); // 죽음 애니메이션 트리거

     

        // 추가적인 죽음 처리 로직
    }


    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            hp -= damage; // 데미지를 받아 체력을 감소시킴
            isHit = true; // 공격을 받았음을 표시
            StartCoroutine(DelayedHitAnimation()); // 0.1초 딜레이 후 애니메이션 트리거
            /*if (hp <= 0)
            {
                Die();
            }*/
        }
    }

    IEnumerator DelayedHitAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.SetTrigger("IsHit"); // 공격 받음 애니메이션 트리거
        if (hp <= 0)
        {
            Die();
        }
    }

    public int GetHP()
    {
        return hp;
    }
}