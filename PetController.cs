using UnityEngine;

public class PetController : MonoBehaviour
{
    public Transform playerTransform;
    public float followSpeed = 3.0f;
    public float stopDistance = 1.0f;

    [Header("Pet 4-Direction Sprites")]
    public Sprite petUp;
    public Sprite petDown;
    public Sprite petLeft;
    public Sprite petRight;

    private SpriteRenderer spriteRenderer;
    private Vector3 lastPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calculate distance to player
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > stopDistance)
        {
            // Move towards player
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, followSpeed * Time.deltaTime);
        }

        // Calculate movement direction for sprite swapping
        Vector3 movementDelta = transform.position - lastPosition;
        UpdatePetDirectionSprite(movementDelta);

        lastPosition = transform.position;
    }

    private void UpdatePetDirectionSprite(Vector3 delta)
    {
        if (spriteRenderer == null || delta.sqrMagnitude < 0.0001f) return;

        // Check if moving more horizontally or vertically
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0) spriteRenderer.sprite = petRight;
            else spriteRenderer.sprite = petLeft;
        }
        else
        {
            if (delta.y > 0) spriteRenderer.sprite = petUp;
            else spriteRenderer.sprite = petDown;
        }
    }
}
