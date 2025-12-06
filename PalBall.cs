using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PalBall : MonoBehaviour
{
    Rigidbody2D rigid;

    public Transform prefabFx;

    public Player player { get; set; }

    bool hitEnemy = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("DestroySelf", 0.7f);
    }

    private void Update()
    {
        if (hitEnemy)
        {
            rigid.velocity = Vector2.zero;
            rigid.angularVelocity = 0;
        }
    }

    void DestroySelf()
    {
        if (hitEnemy)
        {
            return;
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (!enemy)
        {
            return;
        }

        enemy.OnPalBallHit(transform);

        GetComponent<Collider2D>().enabled = false;

        hitEnemy = true;

        StartCoroutine(CoDoCatching(enemy));
    }

    IEnumerator CoDoCatching(Enemy enemy)
    {
        // 晃一晃
        for (int k=0; k<3; k++)
        {
            for (int i = 0; i < 2; i++)
            {
                transform.Rotate(0, 0, -5);
                yield return new WaitForSeconds(0.05f);
            }
            for (int i = 0; i < 4; i++)
            {
                transform.Rotate(0, 0, 5);
                yield return new WaitForSeconds(0.05f);
            }
            for (int i = 0; i < 2; i++)
            {
                transform.Rotate(0, 0, -5);
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.15f);
        }

        int r = Random.Range(0, 100);

        PalHealth palHealth = enemy.GetComponent<PalHealth>();
        float hpPercent = (float)palHealth.hp / palHealth.maxHp;
        if (hpPercent <= 20)
        {
            r -= 30;
        }
        else if (hpPercent <= 50)
        {
            r -= 20;
        }

        if (r < 20)
        {
            // 成功
            float t = 0;
            while (t < 0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, player.transform.position, 0.03f);
                t += Time.deltaTime;
                yield return null;
            }

            if (GameMode.Instance)
            {
                GameMode.Instance.AddPal(enemy);
            }

            Destroy(enemy.gameObject);
            Destroy(gameObject);
        }
        else
        {
            // 失败
            yield return new WaitForSeconds(0.4f);
            enemy.OnCatchingFailed();
            Destroy(gameObject);
            if (prefabFx)
            {
                Instantiate(prefabFx, transform.position, Quaternion.identity);
            }
        }
    }
}
