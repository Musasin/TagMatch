using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;

public class GaugeUI : MonoBehaviour
{
    const float SCALE_CHANGE_SPEED = 200.0f;
    const float WAIT_TIME = 0.5f;

    public enum GaugeType { YukariHP, YukariMP, MakiHP, MakiMP, BossHP }
    public GaugeType gaugeType;
    public bool isInstantly;
    public bool isOutline;

    RectTransform rt;

    float waitTime;
    float width;
    float defaultWidth;
    int gaugePoint;
    int gaugePointMax;
    bool isBossStage;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        defaultWidth = rt.sizeDelta.x;
        width = rt.sizeDelta.x;
        UpdateGaugePoint();
        isBossStage = (GameObject.Find("Enemies").GetComponentInChildren<Boss>() != null);
    }

    // Update is called once per frame
    void Update()
    {
        float step = SCALE_CHANGE_SPEED * Time.deltaTime;
        waitTime -= Time.deltaTime;

        int beforePoint = gaugePoint;
        UpdateGaugePoint();

        if (!isBossStage && gaugeType == GaugeType.BossHP)
        {
            gameObject.SetActive(false);
        }

        // 即時反映じゃない方のゲージは少し待ってから増減を行う
        // 増減の最中(= sizeDelta.xとwidthが不一致)の時は待機しない
        if (gaugePoint != beforePoint && !isInstantly && rt.sizeDelta.x == width)
        {
            waitTime = WAIT_TIME;
        }

        if (gaugeType == GaugeType.BossHP)
        {
            width = defaultWidth * gaugePoint / gaugePointMax;
        }
        else
        {
            // プレイヤーのゲージは拡大されうる & 内側のゲージ部分はアウトラインの端の分の幅を削る必要があるため、それを加味した計算を行う
            float outlineDefaultWidth = defaultWidth + 20;
            float outlineMaxWidth = outlineDefaultWidth * (float)gaugePointMax / 100 - 20;
            width = outlineMaxWidth * gaugePoint / gaugePointMax;
        }

        if (isInstantly)
        {
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
        } else if (waitTime < 0)
        {
            rt.sizeDelta = Vector2.MoveTowards(rt.sizeDelta, new Vector2(width, rt.sizeDelta.y), step);
        }
        
        if (isOutline)
        {
            if (gaugeType == GaugeType.BossHP)
            {
                rt.sizeDelta = new Vector2(defaultWidth, rt.sizeDelta.y);
                rt.localScale = new Vector2(0.8f, 0.8f);
            }
            else
            {
                rt.sizeDelta = new Vector2(defaultWidth * ((float)gaugePointMax / 100), rt.sizeDelta.y);
            }
        }
    }

    void UpdateGaugePoint()
    {
        switch (gaugeType)
        {
            case GaugeType.YukariHP:
                gaugePoint = StaticValues.yukariHP;
                gaugePointMax = StaticValues.yukariMaxHP;
                break;
            case GaugeType.YukariMP:
                gaugePoint = StaticValues.yukariMP;
                gaugePointMax = StaticValues.yukariMaxMP;
                break;
            case GaugeType.MakiHP:
                gaugePoint = StaticValues.makiHP;
                gaugePointMax = StaticValues.makiMaxHP;
                break;
            case GaugeType.MakiMP:
                gaugePoint = StaticValues.makiMP;
                gaugePointMax = StaticValues.makiMaxMP;
                break;
            case GaugeType.BossHP:
                gaugePoint = StaticValues.bossHP.Sum();
                gaugePointMax = StaticValues.bossMaxHP.Sum();
                break;
        }
    }
}
