using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AquariusMax.Ancient
{
    [RequireComponent(typeof(CharacterController))]
    public class DemoCharacter : MonoBehaviour
    {
        [SerializeField] Camera cam;
        [SerializeField] float gravityModifier = 2f;
        [SerializeField] float walkSpeed = 5f;
        [SerializeField] float runSpeed = 10f;
        [SerializeField] float jumpSpeed = 10f;
        [SerializeField] float landingForce = 10f;
        [SerializeField] float mouseXSensitivity = 2f;
        [SerializeField] float mouseYSensitivity = 2f;

        [SerializeField] Image healthBar; // 체력바 이미지 추가

        CharacterController charControl;
        Animator animator;
        Quaternion characterTargetRot;
        Quaternion cameraTargetRot;
        bool isGrounded = true;
        bool isWalking = false;
        bool isRunning = false;
        bool isStrafingLeft = false;
        bool isStrafingRight = false;
        bool isJumping = false;
        bool isWalkingBack = false;
        bool isRollingForward = false;
        bool isRollingBackward = false;
        bool isRollingLeft = false;
        bool isRollingRight = false;
        bool isAttacking = false;
        bool isGathering = false; // Gathering 상태를 나타내는 변수

        public int hp = 20;
        public int attackPower = 1; // 플레이어의 공격력 변수
        bool isHit = false;
        private int deathCount = 0;

        private cshAttackArea m_attackArea = null;

        void Start()
        {
            if (cam == null)
                cam = Camera.main;

            charControl = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterTargetRot = transform.localRotation;
            cameraTargetRot = cam.transform.localRotation;
            m_attackArea = GetComponentInChildren<cshAttackArea>();
        }

        void Update()
        {
            CameraLook();
            UpdateMovement();
            UpdateJump();
            UpdateAttack();
            UpdateGathering(); // 추가된 메서드 호출
            UpdateAnimator();
            CheckWalkingBack();
            CheckRollingForward();
            CheckRollingBackward();
            CheckRollingLeft();
            CheckRollingRight();
        }

        void FixedUpdate()
        {
            ApplyMovement();
        }

        void CameraLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseXSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseYSensitivity;

            characterTargetRot *= Quaternion.Euler(0f, mouseX, 0f);
            cameraTargetRot *= Quaternion.Euler(-mouseY, 0f, 0f);

            cameraTargetRot = ClampRotationAroundXAxis(cameraTargetRot);

            transform.localRotation = characterTargetRot;
            cam.transform.localRotation = cameraTargetRot;
        }

        void UpdateMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector2 moveInput = new Vector2(horizontal, vertical).normalized;

            isRunning = Input.GetKey(KeyCode.LeftShift);
            isWalking = Input.GetKey(KeyCode.W);
            isStrafingLeft = Input.GetKey(KeyCode.A);
            isStrafingRight = Input.GetKey(KeyCode.D);
        }

        void UpdateJump()
        {
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
            }
        }

        void UpdateAttack()
        {
            if (Input.GetMouseButtonDown(0)) // 좌클릭을 누르면
            {
                if (this == null) { return; }

                isAttacking = true; // 공격 상태로 설정

                Vector3 center = Vector3.zero;
                int cnt = m_attackArea.colliders.Count;

                int cntBreak = 0;

                for (int i = 0; i < m_attackArea.colliders.Count; ++i)
                {
                    var collider = m_attackArea.colliders[i];
                    center += collider.transform.localPosition;

                    var enemy = collider.GetComponent<Monster>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(attackPower); // 공격력을 사용하여 데미지를 줌
                        if (enemy.GetHP() <= 0) m_attackArea.colliders.Clear();
                    }
                    else
                    {
                        Destroy(collider.gameObject);
                    }
                }
                if (cntBreak > 0) m_attackArea.colliders.Clear();

                center /= cnt;
                center.y = transform.localPosition.y;
                transform.LookAt(center);

            }
            else if (Input.GetMouseButtonUp(0)) // 좌클릭을 뗄 때
            {
                isAttacking = false; // 공격 상태 해제
            }
        }

        void UpdateGathering()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                isGathering = true; // Gathering 상태로 설정
            }
            else if (Input.GetKeyUp(KeyCode.G))
            {
                isGathering = false; // Gathering 상태 해제
            }
        }

        void ApplyMovement()
        {
            float speed = isRunning ? runSpeed : walkSpeed;

            Vector3 move = Vector3.zero;
            Vector3 desiredMove = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, charControl.radius, Vector3.down, out hitInfo, charControl.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            move.x = desiredMove.x * speed;
            move.z = desiredMove.z * speed;

            if (charControl.isGrounded)
            {
                move.y = -landingForce;

                if (isJumping)
                {
                    move.y = jumpSpeed;
                    isJumping = false;
                }
            }
            else
            {
                move += Physics.gravity * gravityModifier * Time.fixedDeltaTime;
            }

            charControl.Move(move * Time.fixedDeltaTime);
        }

        public void TakeDamage()
        {
            hp--; // 데미지를 받아 체력을 감소시킴
            isHit = true; // 공격을 받았음을 표시
            Debug.Log("피해를 받음");
            animator.SetTrigger("IsHit"); // 공격 받음 애니메이션 트리거

            // 체력바 업데이트
            if (healthBar != null)
            {
                healthBar.fillAmount = (float)hp / 20; // 현재 체력에 따라 체력바 fillAmount 설정
            }

            if (hp <= 0)
            {
                Reborn();
            }
        }

        public int GetHP()
        {
            return hp;
        }

        void Reborn()
        {
            deathCount++; // 죽은 횟수를 증가시킴

            if (deathCount == 1)
            {
                hp = 16;
            }
            else if (deathCount == 2)
            {
                hp = 12;
            }
            else // deathCount가 2보다 큰 경우
            {
                hp = 8;
            }

            animator.SetTrigger("Reborn"); // 죽음 애니메이션 트리거

            // 체력바 업데이트
            if (healthBar != null)
            {
                healthBar.fillAmount = (float)hp / 20; // 현재 체력에 따라 체력바 fillAmount 설정
            }

        }

        void UpdateAnimator()
        {
            if (animator != null)
            {
                animator.SetBool("IsWalk", isWalking);
                animator.SetBool("IsRun", isRunning);
                animator.SetBool("IsStrafeLeft", isStrafingLeft);
                animator.SetBool("IsStrafeRight", isStrafingRight);
                animator.SetBool("IsJump", isJumping);
                animator.SetBool("IsWalkBack", isWalkingBack);
                animator.SetBool("IsRollForward", isRollingForward);
                animator.SetBool("IsRollBackward", isRollingBackward);
                animator.SetBool("IsRollLeft", isRollingLeft);
                animator.SetBool("IsRollRight", isRollingRight);
                animator.SetBool("IsAttack", isAttacking);
                animator.SetBool("IsGathering", isGathering); // Gathering 상태 설정
            }
        }

        void CheckWalkingBack()
        {
            isWalkingBack = Input.GetKey(KeyCode.S);
        }

        void CheckRollingForward()
        {
            isRollingForward = Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftControl);
        }

        void CheckRollingBackward()
        {
            isRollingBackward = Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftControl);
        }

        void CheckRollingLeft()
        {
            isRollingLeft = Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftControl);
        }

        void CheckRollingRight()
        {
            isRollingRight = Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftControl);
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, -90f, 90f);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}