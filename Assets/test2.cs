using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : StateMachineBehaviour
{
    private int atcpower = 1;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // animator.gameObject�� ���� �ִϸ����Ͱ� ����� GameObject�� �����ɴϴ�.
        // �׸��� �� ������ �ڽĵ��� �˻��� �� �ֽ��ϴ�.
        foreach (Transform child in animator.gameObject.transform)
        {
            // �ڽ� ��ü �߿��� attackarea�� ã�Ƽ� �� Ʈ���� �̺�Ʈ�� �˻��� �� �ֽ��ϴ�.
            if (child.CompareTag("attackarea"))
            {
                // attackarea�� Collider ������Ʈ�� �ִ��� Ȯ���մϴ�.
                Collider attackCollider = child.GetComponent<Collider>();
                if (attackCollider != null && attackCollider.isTrigger)
                {
                    // attackarea�� �浹�ϴ� ��� Collider�� �迭�� �����ɴϴ�.
                    Collider[] colliders = Physics.OverlapBox(attackCollider.bounds.center, attackCollider.bounds.extents, Quaternion.identity);

                    foreach (Collider collider in colliders)
                    {
                        
                        if (collider.CompareTag("Enemy"))
                        {
                            // player �±׸� ���� ��ü�� ��ũ��Ʈ�� �����Ͽ� �޼��带 ������ �� �ֽ��ϴ�.
                            Monster MobScript = collider.GetComponent<Monster>();
                            if (MobScript != null)
                            {
                                // PlayerScript�� Ư�� �޼��带 �����մϴ�.
                                MobScript.TakeDamage(atcpower);
                            }
                        }
                    }
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
