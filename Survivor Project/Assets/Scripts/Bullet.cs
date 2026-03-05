using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage; 
    public int per; 
    private string myPoolName; 
    Rigidbody2D rigid;

    void Awake() { rigid = GetComponent<Rigidbody2D>(); }

    public void Init(float damage, int per, Vector3 dir, string poolName)
    {
        this.damage = damage;
        this.per = per;
        this.myPoolName = poolName;

        if (per > -1) rigid.linearVelocity = dir * 15f; 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Box"))
        {
            if (per > -1) 
            {
                per--; 
                if (per == -1) 
                {
                    rigid.linearVelocity = Vector2.zero; 
                    PoolManager.instance.Release(myPoolName, gameObject);
                }
            }
        }
    }
}