using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraCtrl : MonoBehaviour
{
    Player player;
    public float smoothSpeed = 0.125f;  // 카메라 이동의 부드러움
    public Vector3 offset;  // 카메라의 오프셋

    // 카메라의 x축 이동 범위 제한값
    public float minX = -9.57f;
    public float maxX = 9.57f;

    // 카메라의 고정된 y축 및 z축 값
    private float fixedY;
    private float fixedZ;

    void Start()
    {
        player = FindAnyObjectByType<Player>();

        // 카메라의 y축과 z축 값을 고정
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        // 플레이어 위치에 오프셋을 더한 카메라의 목표 위치
        Vector3 targetPosition = player.transform.position + offset;

        // x축 이동 범위를 제한
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

        // y축과 z축을 고정된 값으로 유지
        targetPosition.y = fixedY;
        targetPosition.z = fixedZ;

        // 카메라 위치를 부드럽게 업데이트
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}