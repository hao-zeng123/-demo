using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

// 玩家脚本
public class Player : MonoBehaviour
{
    Rigidbody2D rigid;          // 刚体组件
    SpriteRenderer sprite;      // 精灵图组件（在子物体上）
    Animator anim;              // 动画状态机组件

    [Tooltip("移动速度")]
    public float speed = 3;
    public int maxHp = 5;

    [HideInInspector]
    public Vector2 move;
    [HideInInspector]
    public bool isDie = false;
    [HideInInspector]
    public int hp;

    bool isLeft = true;
    float hitbackTime = 0;
    Vector2 origPos;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        hp = maxHp;
    }

    void Update()
    {
        if (!isDie && hitbackTime <= 0)
        {
            UpdateMove();
        }
        if (hitbackTime > 0)
        {
            hitbackTime -= Time.deltaTime;
        }
    }

    void UpdateMove()
    {
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
            if (move.x > 0.01f) { isLeft = false; }
            else if (move.x < -0.01f) {  isLeft = true; }
        }

        // 玩家动画不分左右，需要将精灵图翻转
        sprite.flipX = isLeft;

        // 设置动画变量speed
        anim.SetFloat("speed", move.magnitude);
    }

    private void FixedUpdate()
    {
        if (hitbackTime > 0)
        {
            return;
        }
        if (isDie)
        {
            rigid.velocity = Vector2.zero;
        }
        // 修改刚体速度，才能真的动起来
        rigid.velocity = move * speed;
    }

	public void BeHit(float[] pars)
	{
		int damage = (int)pars[0];
		Vector2 hitDir = new Vector2(pars[1], pars[2]);

		hp -= damage;
        if (hp < 0) { hp = 0; }

        if (hp == 0)
        {
            Die();
        }

        rigid.AddForce(hitDir * 200);

        hitbackTime = 0.2f;
    }

    void Die()
    {
        isDie = true;
        anim.SetBool("die", true);
        GetComponent<Collider2D>().enabled = false;
    }

    // 复活
    public void Revive()
    {
        transform.position = origPos;
        hp = maxHp;
        isDie = false;
        GetComponent<Collider2D>().enabled = true;
        anim.SetBool("die", false);
        anim.Play("idle_right");
    }

}
