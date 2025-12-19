using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Puzzle : UI_Base
{
    public GameObject puzzlePosSet;
    public GameObject puzzlePieceSet;

    // 각 퍼즐 위치의 자식 여부를 나타내는 bool 배열
    bool[] hasChild;

    public bool IsClear()
    {
        for (int i = 0; i < puzzlePosSet.transform.childCount; i++)
        {
            //퍼즐위치의 자식이 없으면 모든 퍼즐조각이 놓여지지 않은것입니다.
            if (puzzlePosSet.transform.GetChild(i).childCount == 0)
            {
                return false;
            }
            //퍼즐조각의 번호와 퍼즐 위치 번호가 일치하지 않으면 퍼즐은 완성되지 않은것입니다.
            if (puzzlePosSet.transform.GetChild(i).GetChild(0).GetComponent<PuzzlePiece>().piece_no != i)
            {
                return false;
            }
        }

        Invoke("Close", 1.0f);
        return true;
    }

    void Start()
    {
        // 각 퍼즐 위치마다 자식 유무를 체크할 배열 초기화
        hasChild = new bool[puzzlePosSet.transform.childCount];
    }

    void Update()
    {
        // 각 퍼즐 위치를 순회하면서 자식 오브젝트의 유무를 확인
        for (int i = 0; i < puzzlePosSet.transform.childCount; i++)
        {
            bool childExists = puzzlePosSet.transform.GetChild(i).childCount > 0;

            // 자식이 생겼고, 이전에 없었던 경우 이벤트 발생
            if (childExists && !hasChild[i])
            {
                hasChild[i] = true; // 자식 존재 여부 업데이트
            }
        }
    }
}
