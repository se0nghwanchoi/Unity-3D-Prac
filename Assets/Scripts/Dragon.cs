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
    public int hp = 10; // ������ ü��
    private bool isHit = false; // ���Ͱ� ������ �޾Ҵ��� ����

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
                    nvAgent.isStopped = true; // ������Ʈ�� ���߱� ���� isStopped �Ӽ��� true�� ����
                    _animator.SetBool("IsTrace", false);
                    _animator.SetBool("IsAttack", false);
                    break;

                case CurrentState.trace:
                    nvAgent.destination = playerTransform.position;
                    nvAgent.isStopped = false; // ������Ʈ�� �簳�ϱ� ���� isStopped �Ӽ��� false�� ����
                    _animator.SetBool("IsTrace", true);
                    _animator.SetBool("IsAttack", false);
                    break;

                case CurrentState.attack:
                    nvAgent.isStopped = true; // ���� ���¿����� ������Ʈ�� ���߱� ���� isStopped �Ӽ��� true�� ����
                    _animator.SetBool("IsTrace", false);
                    _animator.SetBool("IsAttack", true);
                    break;

            }
            yield return null;
        }
    }

    void Die()
    {
        isDead = true; // ���Ͱ� �׾����� ǥ��
        curState = CurrentState.dead; // ���¸� �������� ����
        nvAgent.isStopped = true; // �׺�޽� ������Ʈ ����
        _animator.SetBool("IsTrace", false);
        _animator.SetBool("IsAttack", false);
        _animator.SetTrigger("IsDead"); // ���� �ִϸ��̼� Ʈ����

     

        // �߰����� ���� ó�� ����
    }


    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            hp -= damage; // �������� �޾� ü���� ���ҽ�Ŵ
            isHit = true; // ������ �޾����� ǥ��
            StartCoroutine(DelayedHitAnimation()); // 0.1�� ������ �� �ִϸ��̼� Ʈ����
            /*if (hp <= 0)
            {
                Die();
            }*/
        }
    }

    IEnumerator DelayedHitAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.SetTrigger("IsHit"); // ���� ���� �ִϸ��̼� Ʈ����
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