using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public int snapOffset = 30;
    public UI_Puzzle puzzle;
    public int piece_no;
    Image image;

    // 원래 위치 저장 변수
    private Vector3 originalPosition;

    void Awake()
    {
        puzzle = GetComponentInParent<UI_Puzzle>();

        // 조각의 원래 위치를 저장
        originalPosition = transform.position;
        image = GetComponent<Image>();
    }

    bool CheckSnapPuzzle()
    {
        for (int i = 0; i < puzzle.puzzlePosSet.transform.childCount; i++)
        {
            // 빈 자리를 찾고 스냅 범위 안에 있는지 확인
            if (puzzle.puzzlePosSet.transform.GetChild(i).childCount == 0 &&
                Vector2.Distance(puzzle.puzzlePosSet.transform.GetChild(i).position, transform.position) < snapOffset)
            {
                // piece_no와 자리가 일치하는지 확인
                if (piece_no == i)
                {
                    // 올바른 위치에 놓였으므로 해당 위치의 자식으로 설정
                    transform.SetParent(puzzle.puzzlePosSet.transform.GetChild(i).transform);
                    transform.localPosition = Vector3.zero;

                    image.raycastTarget = false;
                    return true;
                }
                else
                {
                    // 잘못된 위치에 놓였을 경우 다른 조치 실행 (여기서는 예시로 원래 위치로 돌아가게 설정)
                    Debug.Log($"잘못된 위치에 놓였습니다: {piece_no}번 조각은 {i}번 자리가 아닙니다.");
                    transform.position = originalPosition;
                    return false;
                }
            }
        }
        return false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 일치하는 위치가 없거나 잘못된 위치에 놓였을 경우 원래 자리로 돌아가게 함
        if (!CheckSnapPuzzle())
        {
            transform.SetParent(puzzle.puzzlePieceSet.transform);
            transform.position = originalPosition; // 원래 위치로 이동
        }

        // 퍼즐이 클리어 되었는지 체크
        if (puzzle.IsClear())
        {
            GameMgr.inst.userData.PuzzleClear = true;
            Sanctom stoneCtrl = GameObject.Find("StoneObject").GetComponent<Sanctom>();
            stoneCtrl.BossPortalOpen();

            Debug.Log("Clear");
        }
    }
}
