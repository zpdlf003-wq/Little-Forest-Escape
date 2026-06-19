using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // 🎯 실시간 외형 가로채기를 위한 컴포넌트 추가
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // 컴포넌트 자동 가져오기
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;

        if (animator != null)
        {
            if (moveInput.sqrMagnitude > 0.001f)
            {
                animator.SetFloat("InputX", moveX);
                animator.SetFloat("InputY", moveY);
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    // 🎯 [핵심 추가] 애니메이션이 프레임마다 원래 옷을 그리려고 할 때, 새 옷 스프라이트로 실시간 가로채기 연산을 수행합니다.
    void LateUpdate()
    {
        // 옷장에서 새 옷을 장착한 상태이고, 필요한 컴포넌트들이 정상 작동 중일 때만 실행
        if (ClosetManager.Instance != null && ClosetManager.Instance.hasCustomOutfit && ClosetManager.Instance.currentOutfitSprites != null && spriteRenderer != null)
        {
            Sprite currentBaseSprite = spriteRenderer.sprite;

            if (currentBaseSprite != null)
            {
                // 기본 캐릭터 스프라이트 이름의 맨 뒤 숫자(예: Player_0 -> 0)를 추출합니다.
                // 💡 만약 본인의 기본 캐릭터 스프라이트 이미지 이름 구조가 다르다면 규칙에 맞게 숫자를 파싱합니다.
                string spriteName = currentBaseSprite.name;
                int spriteIndex = GetSpriteIndexFromName(spriteName);

                // 새 옷의 16장 배열 범위 안에 안전하게 들어맞는다면 외형을 갈아끼웁니다.
                if (spriteIndex >= 0 && spriteIndex < ClosetManager.Instance.currentOutfitSprites.Length)
                {
                    if (ClosetManager.Instance.currentOutfitSprites[spriteIndex] != null)
                    {
                        spriteRenderer.sprite = ClosetManager.Instance.currentOutfitSprites[spriteIndex];
                    }
                }
            }
        }
    }

    // 스프라이트 이름에서 에셋의 순번 숫자만 안전하게 추출하는 보조 함수
    private int GetSpriteIndexFromName(string name)
    {
        string numberString = string.Empty;
        for (int i = name.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(name[i]))
            {
                numberString = name[i] + numberString;
            }
            else
            {
                if (numberString != string.Empty) break;
            }
        }

        if (int.TryParse(numberString, out int index))
        {
            // 💡 만약 기본 캐릭터 시트 파일 조각 번호가 0번부터 시작하지 않고 다른 번호(예: 120번 등)라면 
            // 현재 애니메이션이 도는 상대적 번호(0~15)로 맞춰주기 위해 인덱스 보정 처리를 합니다.
            // 올려주신 이미지상 기본 번호가 120번 기준 주변이라면 index % 16 처리를 하여 안전하게 0~15 사이로 압축합니다.
            return index % 16;
        }
        return -1;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
