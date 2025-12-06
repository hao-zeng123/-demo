using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PalItem : MonoBehaviour
{
    public new string name;
    public int hp;
    public int maxHp;

    private void Start()
    {
        // 刷新界面上的图标、文字等信息
        Image image = transform.Find("icon").GetComponent<Image>();
        Text textName = transform.Find("name").GetComponent<Text>();
        Text textHp = transform.Find("hp").GetComponent<Text>();

        PalInfo info = GameMode.Instance.GetPalInfo(name);
        if (info == null)
        {
            return;
        }

        image.sprite = info.icon;
        textName.text = info.name;
        textHp.text = $"{hp} / {maxHp}";
    }

    public void OnButtonClick()
    {
        // 派出帕鲁
        GameObject obj = GameMode.Instance.ReleasePal(this);

        // 删除UI项目
        Destroy(gameObject);
    }
}
