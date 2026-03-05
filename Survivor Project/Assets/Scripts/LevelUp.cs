using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;
    int[] ran = new int[3];

    void Awake() {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show() {
        Next();
        rect.localScale = Vector3.one;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true); 
    }

    public void Hide() {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false); 
    }

    public void Select(int index) {
        items[index].OnClick();
        Hide();
    }

    void Next() {
        foreach (Item item in items) { item.gameObject.SetActive(false); }
        
        while (true) {
            ran[0] = Random.Range(0, items.Length - 1);
            ran[1] = Random.Range(0, items.Length - 1);
            ran[2] = Random.Range(0, items.Length - 1);
            
            if (ran[0] != ran[1] && ran[0] != ran[2] && ran[1] != ran[2])
                break;
        }

        for (int index = 0; index < ran.Length; index++) {
            Item ranItem = items[ran[index]];
            if (ranItem.level == ranItem.data.damages.Length) {
                items[5].gameObject.SetActive(true); 
            } else {
                ranItem.gameObject.SetActive(true);
            }
        }
    }
}