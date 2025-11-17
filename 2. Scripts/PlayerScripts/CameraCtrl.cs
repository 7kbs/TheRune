using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    Player player;

    float x_Offset = 0.0f;
    float y_Offset = 2.5f;
    float Update_y = 3.5f;
    float SmoothTime = 0.3f;

    Vector3 initPosition;
    Vector3 velocity = Vector3.zero;

    //Test
    public GameObject BackGround;
    SpriteRenderer[] BG_Rend = null;
    Color OriginColor = Color.white;
    Color ChangeColor = new Color32(173,95,198,255);
    public bool isChange = false;
    float OriginTimer = 0.0f;
    float ChangeTimer = 0.0f;

    public GameObject LastBossProduct;

    void Start()
    {
        player = FindObjectOfType<Player>();
        initPosition = transform.position;
        BG_Rend = BackGround.GetComponentsInChildren<SpriteRenderer>();

        if (GlobalValue.sceneType == GlobalValue.SceneType.Boss)
        {
            BackGround.SetActive(false);
            LastBossProduct.SetActive(true);
        }
        else
        {
            BackGround.SetActive(true);
            LastBossProduct.SetActive(false);
        }
    }


    void Update()
    {
        Vector3 playerPos = player.transform.position;

        float X_pos = playerPos.x + x_Offset;

        if(Mathf.Abs(playerPos.y - initPosition.y) > Update_y)
        {
            float Y_pos = playerPos.y + y_Offset;
            initPosition = new Vector3(X_pos, Y_pos, transform.position.z);
        }
        else
        {
            initPosition = new Vector3(X_pos, initPosition.y, transform.position.z);
        }

        transform.position = Vector3.SmoothDamp
            (transform.position, initPosition, ref velocity, SmoothTime);

        if (GlobalValue.sceneType == GlobalValue.SceneType.Game)
        {
            if (player.transform.position.y <= -21.0f
                && BG_Rend[0].color == OriginColor)
            {
                OriginTimer = 1.0f;
            }

            if (player.transform.position.y >= -21.0f
                && BG_Rend[0].color == ChangeColor)
            {
                ChangeTimer = 1.0f;
            }

        }

        if(OriginTimer > 0.0f)
        {
            OriginTimer -= Time.deltaTime * 0.7f;
            ChangeBackGround();
            if (OriginTimer <= 0.0f)
                OriginTimer = 0.0f;
        }


        if (ChangeTimer > 0.0f)
        {
            ChangeTimer -= Time.deltaTime * 0.7f;

            ChangeBackGroundOrigin();
            if (ChangeTimer <= 0.0f)
                ChangeTimer = 0.0f;
        }
    }

    void ChangeBackGroundOrigin() //배경을 보라색에서 기존색으로
    {

        BG_Rend[0].color = Color.Lerp(OriginColor, ChangeColor, ChangeTimer);
        BG_Rend[1].color = Color.Lerp(OriginColor, ChangeColor, ChangeTimer);
    }

    void ChangeBackGround() //배경을 기존색상에서 보라색으로
    {
        BG_Rend[0].color = Color.Lerp(ChangeColor, OriginColor, OriginTimer);
        BG_Rend[1].color = Color.Lerp(ChangeColor, OriginColor, OriginTimer);
    }
}
