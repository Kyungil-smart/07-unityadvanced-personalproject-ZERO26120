using UnityEngine;

public class DropItem : MonoBehaviour
{
    public enum Type { Exp1, Exp2, Exp3, Box, Heal, Magnet }
    public Type type;
    public int expValue = 1; 

    public bool isMagnet = false;
    Transform player;

    void Awake()
    {
        player = GameManager.instance.player.transform;
    }

    void OnEnable()
    {
        isMagnet = false; 
    }

    void Update()
    {
        if (isMagnet && type != Type.Box)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, 15f * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == Type.Box && collision.CompareTag("Bullet"))
        {
            SpawnItem();
            gameObject.SetActive(false);
            return;
        }

        if (!isMagnet && type != Type.Box && collision.CompareTag("MagnetArea"))
        {
            isMagnet = true;
        }

        if (type != Type.Box && collision.CompareTag("Player"))
        {
            ApplyEffect();
            gameObject.SetActive(false);
        }
    }

    void SpawnItem()
    {
        string dropName = Random.Range(0, 2) == 0 ? "Heal" : "Magnet";
        GameObject dropObj = PoolManager.instance.Get(dropName);
        if (dropObj != null)
        {
            dropObj.transform.position = transform.position; 
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit); 
    }

    void ApplyEffect()
    {
        switch (type)
        {
            case Type.Exp1:
            case Type.Exp2:
            case Type.Exp3:
                GameManager.instance.GetExp(expValue); 
                break;

            case Type.Heal: 
                GameManager.instance.health = Mathf.Min(GameManager.instance.maxHealth, GameManager.instance.health + 30);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp); 
                break;

            case Type.Magnet: 
                DropItem[] allItems = FindObjectsByType<DropItem>(FindObjectsSortMode.None);
                foreach (DropItem item in allItems)
                {
                    if (item.type == Type.Exp1 || item.type == Type.Exp2 || item.type == Type.Exp3) 
                    {
                        item.isMagnet = true;
                    }
                }
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
                break;
        }
    }
}