using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleBoxCtrl : MonoBehaviour
{
    public bool isOn = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = this.transform.position
        };

        var result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, result);

        if (result.Count > 0)
        {
            //RaycastResult Hit = result[0];
            foreach (var Hit in result)
            {
                if (Hit.gameObject.name.Contains("ItemGroup"))
                {                    
                    OnPuzzle(Hit.gameObject);
                }

                if (Hit.gameObject.name.Contains("ItemGroup") || Hit.gameObject.name.Contains("ItemChild"))
                {
                    isOn = true;
                    GetComponent<Image>().color = Color.green;
                }
            }
        }
        else
        {
            isOn = false;
            GetComponent<Image>().color = Color.white;
        }
    }

    void OnPuzzle(GameObject puzzle)
    {
        puzzle.transform.position = this.transform.position;
    }

}
