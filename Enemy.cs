using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AIState
{
    Idle,       // 静止
    Chase,      // 追击
    Behit,      // 被击退
    Dead,       // 死亡
}

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;

    Vector2 move;

    public new string name;
    public float speed = 1;
    public float chaseDist = 5f;
    public float attackDist = 1f;

    public AIState state;
    Transform player;
    public float hitbackTime = 0;

    PalHealth palHealth;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        player = GameObject.FindWithTag("Player")?.transform;
        palHealth = GetComponent<PalHealth>();
    }


    void Update()
    {
        if (!player)
        {
            state = AIState.Idle;
            return;
        }

        // 目标位置减去自身位置，得到从自身到目标的向量
        Vector2 to = player.position - transform.position;

        // 如果有宠物离得比玩家更近，目标改为宠物
        Transform target = FindNearestTarget();
        if (target && Vector3.Distance(target.position, transform.position) < to.magnitude)
        {
            to = target.position - transform.position;
        }

        switch (state)
        {
            case AIState.Idle:
                {
                    move = Vector2.zero;
                    if (to.magnitude <= chaseDist)
                    {
                        state = AIState.Chase;
                        if (palHealth) { palHealth.SetHpVisible(true); }
                    }
                }
                break;
            case AIState.Chase:
                {
                    if (to.magnitude > chaseDist + 3)
                    {
                        state = AIState.Idle;
                        anim.SetFloat("speed", 0);
                        if (palHealth) { palHealth.SetHpVisible(false); }
                        break;
                    }

                    // 判断是否在攻击动作的距离attackDist之内
                    if (to.magnitude > attackDist)
                    {
                        // 在攻击范围外，向目标移动
                        move = to.normalized;

                        if (to.x > 0)
                        {
                            sprite.flipX = false;
                        }

                        if (to.x < 0)
                        {
                            sprite.flipX = true;
                        }

                        anim.ResetTrigger("attack");    // 取消攻击动作
                        anim.SetFloat("speed", to.magnitude);
                    }
                    else
                    {
                        if (to.x > 0)
                        {
                            sprite.flipX = false;
                            anim.SetInteger("dir", 4);
                        }

                        if (to.x < 0)
                        {
                            sprite.flipX = true;
                            anim.SetInteger("dir", 3);
                        }

                        anim.SetTrigger("attack");
                        move = Vector2.zero;
                    }
                }
                break;
            case AIState.Behit:
                {
                    hitbackTime -= Time.deltaTime;
                    if (hitbackTime <= 0)
                    {
                        state = AIState.Chase;
                    }
                }
                break;
            case AIState.Dead:
                // 什么都不做
                return;
        }
    }

    private void FixedUpdate()
    {
        if (state == AIState.Dead)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        if (state == AIState.Idle || state == AIState.Chase)
        {
            rigid.velocity = move * speed;
        }
    }

    public void Die()
    {
        state = AIState.Dead;
        anim.SetBool("die", true);
        GetComponent<Collider2D>().enabled = false;
        GameMode.Instance?.AddPalBall();
        return;
    }

    // 被精灵球捕捉
    public void OnPalBallHit(Transform transBall)
    {
        StartCoroutine(CoOnPalBallHit(transBall));
    }

    IEnumerator CoOnPalBallHit(Transform transBall)
    {
        Vector3 pos = transBall.position;
        float time = 0.4f;
        while (time > 0)
        {
            float t = 1 - time / 0.4f;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, t);
            transform.position = Vector3.Lerp(transform.position, pos, t);
            time -= Time.deltaTime;
            yield return null;
        }
        //GetComponent<Collider2D>().enabled = false;
        gameObject.SetActive(false);
    }

    // 捕捉失败
    public void OnCatchingFailed()
    {
        gameObject.SetActive(true);
        GetComponent<Collider2D>().enabled = true;

        StartCoroutine(CoOnCatchingFailed());
    }

    IEnumerator CoOnCatchingFailed()
    {
        float time = 0.4f;
        Vector3 orig = transform.localScale;
        while (time > 0)
        {
            float t = 1 - time / 0.4f;
            transform.localScale = Vector3.Lerp(orig, Vector3.one, t);
            time -= Time.deltaTime;
            yield return null;
        }
    }

    public Transform FindNearestTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,
            chaseDist, LayerMask.GetMask("Member"));
        if (colliders.Length == 0)
        {
            return null;
        }

        System.Array.Sort(colliders, (Collider2D a, Collider2D b) => {
            float da = Vector2.Distance(a.transform.position, transform.position);
            float db = Vector2.Distance(b.transform.position, transform.position);
            return da.CompareTo(db);
        });

        return colliders[0].transform;
    }

    public void HitBack(Vector2 hitDir)
    {
        state = AIState.Behit;

        anim.SetTrigger("behit");
        hitbackTime = 0.2f;

        rigid.AddForce(hitDir * 200);
    }
}
