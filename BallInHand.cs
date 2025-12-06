using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BallInHand : MonoBehaviour
{
    Player player;
    bool active = false;

    public Rigidbody2D prefabPalBall;

    void Start()
    {
        player = GetComponentInParent<Player>();
        SetActive(false);
    }

    void Update()
    {
        if (GameMode.Instance)
        {
            if (GameMode.Instance.numPalBall == 0)
            {
                return;
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            SetActive(true);
            UpdateBall();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (GameMode.Instance)
            {
                GameMode.Instance.RemovePalBall();
            }

            Rigidbody2D ball = Instantiate(prefabPalBall, transform.position, Quaternion.identity);

            Vector3 to = transform.up;

            ball.velocity = to * 10;

            PalBall palBall = ball.GetComponent<PalBall>();
            palBall.player = GetComponentInParent<Player>();

            SetActive(false);
        }
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

    void UpdateBall()
    {
        Vector3 screenPos = Input.mousePosition;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(screenPos);

        Vector2 toBall = mousePos - transform.position;
        toBall = toBall.normalized;

        transform.up = toBall.normalized;
    }
}
