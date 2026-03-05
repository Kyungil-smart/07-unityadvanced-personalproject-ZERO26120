using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public string prefabPoolName; 
    public float damage;
    public int count; 
    public float speed; 
    public float baseSpeed; 

    float timer;
    Player player;

    void Awake() { player = GameManager.instance.player; }

    void Update() {
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;
        
        if (id == 0 || id == 3) {
            transform.Rotate(Vector3.back * speed * Time.deltaTime);
        }
        else {
            timer += Time.deltaTime;
            if (timer > speed) { 
                timer = 0f; 
                Fire(); 
            }
        }
    }

    public void Init(ItemData data) {
        name = "Weapon_" + data.itemName;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = (int)(data.baseCount + Character.Count);
        prefabPoolName = data.projectilePoolName; 

        switch (id) {
            case 0: baseSpeed = 150 * Character.WeaponSpeed; break;
            case 3: baseSpeed = 180 * Character.WeaponSpeed; break;
            case 1: baseSpeed = 0.5f * Character.WeaponRate; break;
            case 2: baseSpeed = 0.8f * Character.WeaponRate; break;
            default: baseSpeed = 0.5f * Character.WeaponRate; break;
        }
        speed = baseSpeed;

        timer = Random.Range(0f, speed);

        if (id == 0 || id == 3) {
            Batch(); 
        }
        
        ApplyAllGears(); 
    }

    public void LevelUp(float damage, int count) {
        this.damage = damage * Character.Damage;
        this.count += count; 
        
        if (id == 0 || id == 3) Batch(); 
        ApplyAllGears(); 
    }

    void Fire() {
        if (!player.scanner.nearestTarget) return;
        Vector3 dir = (player.scanner.nearestTarget.position - transform.position).normalized;
        
        if (id == 2) FireSpread(dir, 20f); 
        else CreateBullet(dir);
        
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }

    void CreateBullet(Vector3 direction) {
        Transform bullet = PoolManager.instance.Get(prefabPoolName).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        bullet.localScale = Vector3.one * Character.Scale;
        
        int per = (id == 2) ? 0 : count;
        bullet.GetComponent<Bullet>().Init(damage, per, direction, prefabPoolName);
    }

    void Batch() {
        for (int index = 0; index < count; index++) {
            Transform bullet;
            if (index < transform.childCount) bullet = transform.GetChild(index);
            else { 
                bullet = PoolManager.instance.Get(prefabPoolName).transform; 
                bullet.parent = transform; 
            }
            
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;
            bullet.Rotate(Vector3.forward * 360 * index / count);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.localScale = Vector3.one * Character.Scale;
            
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero, prefabPoolName); 
        }
    }

    void FireSpread(Vector3 dir, float spreadAngle) {
        for (int i = 0; i < count; i++) {
            float angle = count == 1 ? 0 : -spreadAngle + (spreadAngle * 2 * i / (count - 1));
            CreateBullet(Quaternion.Euler(0, 0, angle) * dir); 
        }
    }

    void ApplyAllGears() {
        Gear[] gears = player.GetComponentsInChildren<Gear>();
        foreach (Gear gear in gears) { gear.ApplyGear(); }
    }
}