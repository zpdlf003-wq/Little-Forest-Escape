using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 캐릭터
    public float smoothSpeed = 0.125f; // 카메라가 따라가는 부드러운 속도

    // 카메라가 움직일 수 있는 최소/최대 좌표 (배경의 가장자리에 맞춰 설정)
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void LateUpdate()
    {
        if (target == null) return;

        // 캐릭터의 목표 위치 정의 (카메라 Z축은 고정)
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // 부드러운 이동 (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Mathf.Clamp를 사용하여 카메라 위치를 최소/최대 범위 내로 제한
        float clampedX = Mathf.Clamp(smoothedPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothedPosition.y, minY, maxY);

        // 제한된 좌표로 카메라 위치 적용
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
