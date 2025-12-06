using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PalHealth : MonoBehaviour
{
    public int maxHp = 5;
    [HideInInspector]
    public int hp;
    public Color flashColor = Color.red;

    public Image hpImage;
    Rigidbody2D rigid;
    SpriteRenderer render;

    void Start()
    {
        hp = maxHp;
        rigid = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();

        if (hpImage)
        {
            hpImage.gameObject.SetActive(false);
        }
        RefreshHpImage();
    }

    public void BeHit(float[] pars)
    {
        int damage = (int)pars[0];
        Vector2 hitDir = new Vector2(pars[1], pars[2]);

        hp -= damage;
        if (hp < 0) { hp = 0; }
        RefreshHpImage();
        Flash(0.1f);

        Enemy enemy = GetComponent<Enemy>();
        if (enemy)
        {
            enemy.HitBack(hitDir);
            if (hp == 0)
            {
                if (hpImage)
                {
                    hpImage.transform.parent.gameObject.SetActive(false);
                }
                
                enemy.Die();
            }
        }

        Member member = GetComponent<Member>();
        if (member)
        {
            if (hp == 0)
            {
                member.HitBack(hitDir);
                if (hpImage)
                {
                    hpImage.transform.parent.gameObject.SetActive(false);
                }
                member.Die();
            }
        }
    }


    void RefreshHpImage()
    {
        if (!hpImage)
        {
            return;
        }
        Image fillImage = hpImage.transform.GetChild(0).GetComponent<Image>();
        fillImage.fillAmount = (float)hp / maxHp;
    }

    public void SetHpVisible(bool visible)
    {
        if (hpImage)
        {
            hpImage.gameObject.SetActive(visible);
        }
    }


    public void Flash(float time)
    {
        StopAllCoroutines();
        StartCoroutine(CoFlash(time));
    }

    IEnumerator CoFlash(float time)
    {
        float beg = Time.time;
        while (Time.time < beg + time)
        {
            float percent = (Time.time - beg) / time;
            Color c = Color.Lerp(Color.white, flashColor, percent);

            render.color = c;
            yield return null;
        }

        beg = Time.time;
        time /= 2;
        while (Time.time < beg + time)
        {
            float percent = (Time.time - beg) / time;
            Color c = Color.Lerp(flashColor, Color.white, percent);

            render.color = c;
            yield return null;
        }
    }
}
