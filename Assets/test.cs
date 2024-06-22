using AquariusMax.Ancient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : StateMachineBehaviour
{

    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // animator.gameObject는 현재 애니메이터가 연결된 GameObject를 가져옵니다.
        // 그리고 그 하위의 자식들을 검색할 수 있습니다.
        foreach (Transform child in animator.gameObject.transform)
        {
            // 자식 객체 중에서 attackarea를 찾아서 그 트리거 이벤트를 검사할 수 있습니다.
            if (child.CompareTag("attackarea"))
            {
                // attackarea에 Collider 컴포넌트가 있는지 확인합니다.
                Collider attackCollider = child.GetComponent<Collider>();
                if (attackCollider != null && attackCollider.isTrigger)
                {
                    // attackarea와 충돌하는 모든 Collider를 배열로 가져옵니다.
                    Collider[] colliders = Physics.OverlapBox(attackCollider.bounds.center, attackCollider.bounds.extents, Quaternion.identity);

                    foreach (Collider collider in colliders)
                    {
                        // 충돌한 Collider가 player 태그를 가진 객체인지 확인합니다.
                        if (collider.CompareTag("Player"))
                        {
                            // player 태그를 가진 객체의 스크립트에 접근하여 메서드를 실행할 수 있습니다.
                            DemoCharacter playerScript = collider.GetComponent<DemoCharacter>();
                            if (playerScript != null)
                            {
                                // PlayerScript의 특정 메서드를 실행합니다.
                                playerScript.TakeDamage(); 
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