using UnityEngine;

[RequireComponent(typeof(Animator))]
public class move_test : MonoBehaviour
{
    // 1) 애니메이터 컴포넌트를 담을 변수
    private Animator animator;

    // 2) 컴포넌트 초기화
    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("move_test: Animator 컴포넌트를 찾을 수 없습니다!");
    }

    // 3) 매 프레임마다 호출되는 로직
    void Update()
    {
        // — 입력값 읽기
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // — 이동 벡터 계산 및 이동
        Vector3 moveVector = new Vector3(moveX, moveY, 0f);
        transform.Translate(moveVector.normalized * Time.deltaTime * 5f);

        // — 이동 애니메이션 토글
        bool isMoving = (moveX != 0f || moveY != 0f);
        animator.SetBool("1_Move", isMoving);

        // — 스페이스바 입력 시 공격 트리거
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("2_Attack");
            Debug.Log("스페이스바 눌림");
        }
    }
}
