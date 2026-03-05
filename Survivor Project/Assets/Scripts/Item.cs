using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;
    
    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;
        
        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    GameObject newWeaponObj = new GameObject("Weapon_" + data.itemName);
                    weapon = newWeaponObj.AddComponent<Weapon>(); 
                    weapon.Init(data); 
                }
                else
                {
                    float nextDamage = data.baseDamage + (data.baseDamage * data.damages[level]);
                    int nextCount = (int)data.counts[level];
                    weapon.LevelUp(nextDamage, nextCount);
                }
                break;

            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.Glove: 
                if (level == 0)
                {
                    GameObject newGearObj = new GameObject("Gear_" + data.itemName);
                    gear = newGearObj.AddComponent<Gear>(); 
                    gear.Init(data);
                }
                else
                {
                    gear.LevelUp(data.damages[level]);
                }
                break;

            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }
        
        level++;

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}