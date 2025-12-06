using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Member : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    PalHealth palHeath;

    public float speed = 5;

    Transform master;
    Vector2 move;
    bool isLeft = false;
    bool isDie = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        palHeath = GetComponent<PalHealth>();
        master = GameObject.FindGameObjectWithTag("Player").transform;
        if (master)
        {
            speed = master.GetComponent<Player>().speed;
        }
    }

    void Update()
    {
        if (isDie)
        {
            move = Vector3.zero;
            return;
        }
        if (Vector3.Distance(transform.position, master.position) > 3)
        {
            move = (master.position - transform.position).normalized;
            return;
        }

        // 获取横向和纵向输入，变量范围0.0f ~ 1.0f
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool attack = Input.GetButtonDown("Fire1");

        move = new Vector2(h, v);
        if (move.magnitude > 1)     // 当上和右同时按下时，move向量长度会超过1
        {
            move = move.normalized;
        }

        // 方向变量dir
        if (move.magnitude > 0.1f)
        {
            if (move.x > 0) { isLeft = false; }
            else { isLeft = true; }
        }

        // 动画不分左右，需要将精灵图翻转
        sprite.flipX = isLeft;

        if (attack)
        {
            anim.SetTrigger("attack");
        }

        // 设置动画变量speed
        anim.SetFloat("speed", move.magnitude);
        if (isLeft) { anim.SetInteger("dir", 3); }
        else { anim.SetInteger("dir", 4); }
    }

    private void FixedUpdate()
    {
        rigid.velocity = move * speed;
    }

    public void Die()
    {
        isDie = true;
        anim.SetBool("die", true);
        GetComponent<Collider2D>().enabled = false;
    }

    public void HitBack(Vector2 hitDir)
    {
        anim.SetTrigger("behit");
    }
}
