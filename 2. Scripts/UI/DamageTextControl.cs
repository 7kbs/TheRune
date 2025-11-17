using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextControl : MonoBehaviour
{
    float time;
    Vector3 dir;
    float EffTime = 0.0f;         //연출 시간 계산용 변수
    public Text DamageText = null;  //Text UI 접근용 변수

    //속도 = 거리 / 시간
    float MvVelocity = 1.1f / 1.05f;  //1.05초 동안에 1.1m 간다는 ... 속도
    float ApVelocity = 1.0f / (1.0f - 0.4f);
    //alpha 0.4초부터 1.0초까지 (0.6초동안) : 0.0 -> 1.0 변화하는 속도

    Vector3 CurPos;   //위치 계산용 변수
    Color Color;    //색깔 계산용 변수


    void Start()
    {
        dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1).normalized;
    }


    void Update()
    {
        EffTime += Time.deltaTime;

        if (EffTime < 1.05f)
        {
            DamageText.transform.Translate(dir * Time.deltaTime);
            transform.localScale = Vector3.one * (1 + time);
            time += Time.deltaTime;
        }

        if (0.4f < EffTime)
        {
            Color = DamageText.color;
            Color.a -= (Time.deltaTime * ApVelocity);
            if (Color.a < 0.0f)
                Color.a = 0.0f;
            DamageText.color = Color;
        }

        if (1.05f < EffTime)
        {
            Destroy(gameObject);
        }
    }

    public void InitDamage(float dmg, Color color)
    {
        DamageText = this.GetComponentInChildren<Text>();

        DamageText.text = "-" + dmg;

        color.a = 1.0f;
        DamageText.color = color;
    }
}
