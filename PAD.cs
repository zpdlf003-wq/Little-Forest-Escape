using UnityEngine;
using UnityEngine.InputSystem; // New Input System 사용을 위해 필수

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Rigidbody 2D 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 매 프레임 입력값 처리 (원하는 경우 이곳에서 애니메이션 방향 설정)
    }

    void FixedUpdate()
    {
        // 물리 기반 캐릭터 이동 처리
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);
    }

    // Input System 메시지 시스템에 의해 자동으로 호출되는 메서드
    // 컴포넌트에 Player Input이 추가되어 있어야 작동합니다.
    public void OnMove(InputValue value)
    {
        // 게임패드 스틱이나 D-Pad의 입력값을 Vector2(X, Y)로 받아옴
        moveInput = value.Get<Vector2>();
    }
}
