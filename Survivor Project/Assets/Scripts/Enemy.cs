using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    
    private string myPoolName; 
    private Rigidbody2D target;
    private bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait; 

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;
        if (!isLive || target == null || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return; 

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero; 
    }

    void LateUpdate()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;
        if (!isLive || target == null) return;
        
        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        isLive = false;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data, Rigidbody2D playerTarget)
    {
        myPoolName = data.poolName;
        target = playerTarget; 
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        
        isLive = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive || !collision.CompareTag("Bullet")) return;

        if (collision.TryGetComponent(out Bullet bullet))
        {
            health -= bullet.damage;
            StartCoroutine(Knockback()); 

            if (health > 0)
            {
                anim.SetTrigger("Hit");
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            }
            else
            {
                Die(); 
            }
        }
    }

    void Die()
    {
        isLive = false;
        coll.enabled = false;
        rigid.simulated = false; 
        spriter.sortingOrder = 1; 
        anim.SetBool("Dead", true);
        
        GameManager.instance.kill++;
        DropExp(); 

        if (GameManager.instance.currentState == GameManager.GameState.Playing)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
    }

    void DropExp()
    {
        string expName = "Exp1";
        int expVal = 1;

        if (maxHealth >= 100) { expName = "Exp2"; expVal = 3; }
        if (maxHealth >= 300) { expName = "Exp3"; expVal = 5; }

        GameObject expObj = PoolManager.instance.Get(expName);
        if (expObj != null)
        {
            expObj.transform.position = transform.position;
            expObj.GetComponent<DropItem>().expValue = expVal; 
        }
    }

    void Dead() { PoolManager.instance.Release(myPoolName, gameObject); }

    IEnumerator Knockback()
    {
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dir = transform.position - playerPos;
        rigid.AddForce(dir.normalized * 3, ForceMode2D.Impulse);
    }
}