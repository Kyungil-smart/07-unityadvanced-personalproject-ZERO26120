using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }
    
    [Header("# 기본 정보")]
    public ItemType itemType; 
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;
    
    [Header("# 레벨별 수치")]
    public float baseDamage;
    public int baseCount;
    public float[] damages; 
    public float[] counts;  

    [Header("# 무기 설정")]
    public GameObject projectile; 
    public string projectilePoolName; 
    public Sprite hand; 
}