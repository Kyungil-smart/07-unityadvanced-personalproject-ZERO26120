using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;
    
    private Text myText;
    private Slider mySlider;

    private int currentLevel = -1;
    private int currentKill = -1;
    private int currentTime = -1;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Playing)
            return;

        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                int expLevel = Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1);
                float maxExp = GameManager.instance.nextExp[expLevel];
                mySlider.value = curExp / maxExp;
                break;

            case InfoType.Level: 
                if (currentLevel != GameManager.instance.level)
                {
                    currentLevel = GameManager.instance.level;
                    myText.text = $"Lv.{currentLevel:F0}";
                }
                break;

            case InfoType.Kill:
                if (currentKill != GameManager.instance.kill)
                {
                    currentKill = GameManager.instance.kill;
                    myText.text = $"{currentKill:F0}";
                }
                break;

            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int timeInt = Mathf.FloorToInt(remainTime);

                if (currentTime != timeInt)
                {
                    currentTime = timeInt;
                    int min = currentTime / 60;
                    int sec = currentTime % 60;
                    myText.text = $"{min:D2}:{sec:D2}";
                }
                break;

            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}