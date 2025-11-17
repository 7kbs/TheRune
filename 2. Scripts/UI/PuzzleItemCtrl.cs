using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleItemCtrl : MonoBehaviour, IDragHandler, IPointerClickHandler
{

    void Start()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundMgr.inst.UI_Play((int)SoundMgr.UI_Sound.Button);
    }
}
