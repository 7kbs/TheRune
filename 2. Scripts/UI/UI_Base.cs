using UnityEngine;

public class UI_Base : MonoBehaviour
{

    void OnEnable()
    {
        PlayerMove.inst.ChangeState(new InteractingState());
    }

    void OnDisable()
    {
        PlayerMove.inst.ChangeState(new DefaultState());
    }
}
