using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type; 
    public float rate;

    public void Init(ItemData data) {
        name = "Gear_" + data.itemName;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;
        type = data.itemType;
        rate = data.damages[0]; 
        ApplyGear();
    }

    public void LevelUp(float rate) {
        this.rate = rate;
        ApplyGear();
    }

    public void ApplyGear() {
        switch (type) {
            case ItemData.ItemType.Glove: RateUp(); break;
            case ItemData.ItemType.Shoe: SpeedUp(); break;
        }
    }

    void RateUp() {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons) {
            if (weapon.id == 0 || weapon.id == 3) {
                weapon.speed = weapon.baseSpeed * (1f + rate);
            } else {
                weapon.speed = weapon.baseSpeed * (1f - rate);
            }
        }
    }

    void SpeedUp() {
        float speed = 3 * Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}