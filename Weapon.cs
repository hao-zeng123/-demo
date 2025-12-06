using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    float origAngle;
    bool swinging = false;
    float startSwingTime;

    public float totalAngle = 100;
    public float totalTime = 0.2f;

    Player player;
    Collider2D hitBox;

    public AnimationCurve curve;

    bool active;

    void Start()
    {
        player = GetComponentInParent<Player>();
        hitBox = GetComponentInChildren<Collider2D>();
        hitBox.enabled = false;
        origAngle = transform.localEulerAngles.z;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            SetActive(false);
        }
        else
        {
            SetActive(true);
        }

        if (!active)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && !swinging)
        {
            StartCoroutine(Swing());
            swinging = true;
        }


        Vector3 dir;
        if (player.move.magnitude > 0.01f && !swinging)
        {
            dir = player.move.normalized;
            transform.up = dir;
            origAngle = transform.localEulerAngles.z;
        }

        // 挥舞过程中，开启/关闭 攻击判定框
        if (swinging)
        {
            float t = (Time.time - startSwingTime) / totalTime;

            if (t > 0.9f)
            {
                // 关闭判定框
                hitBox.enabled = false;
            }
            else if (t > 0.1f)
            {
                // 开启判定框
                hitBox.enabled = true;
            }
        }
    }

    IEnumerator Swing()
    {
        startSwingTime = Time.time;
        float t0 = startSwingTime;

        Quaternion q1 = transform.rotation;
        Quaternion q2 = transform.rotation * Quaternion.Euler(0, 0, totalAngle);

        while (Time.time < t0 + totalTime)
        {
            float t = (Time.time - t0) / totalTime;
            t = curve.Evaluate(t);
            Quaternion q = Quaternion.SlerpUnclamped(q1, q2, t);
            transform.rotation = q;
            yield return null;
        }

        float bt = 0.1f;
        while (bt > 0)
        {
            float t = 1 - (bt / 0.1f);
            Quaternion q = Quaternion.SlerpUnclamped(q2, q1, t);
            transform.rotation = q;
            bt -= Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0, 0, origAngle);

        swinging = false;
    }

    void SetActive(bool active)
    {
        this.active = active;
        SpriteRenderer[] renders = GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renders)
        {
            r.enabled = active;
        }
    }
}
