using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTimeCtrl : MonoBehaviour
{
    public Image SkillCoolImg;

    float SkillTime = 0.0f;
    float fillAmountTimer = 0.0f;

    void Update()
    {
        if(fillAmountTimer > 0.0f)
        {
            fillAmountTimer -= Time.deltaTime;
            CoolTimeUI_On();

            if (fillAmountTimer <= 0.0f)
            {
                SkillCoolImg.gameObject.SetActive(false);
                SkillCoolImg.fillAmount = 0.0f;
                fillAmountTimer = 0.0f;
            }
        }
    }

    void CoolTimeUI_On()
    {
        SkillCoolImg.gameObject.SetActive(true);
        SkillCoolImg.fillAmount += Time.deltaTime / SkillTime;
    }

    public void SkillTimeCtrl(float time)
    {
        if (fillAmountTimer > 0.0f)
            return;

        fillAmountTimer = time;
        SkillTime = time;
    }
}
