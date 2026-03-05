using System.Collections;
using UnityEngine;

public class AchieveManager : MonoBehaviour
{
    [Header("# 캐릭터 선택 UI")]
    public GameObject[] lockCharacter; 
    public GameObject[] unlockCharacter; 
    
    [Header("# 해금 알림 UI")]
    public GameObject uiNotice; 
    
    enum Achieve { UnlockCount, UnlockPower } 
    Achieve[] achieves; 
    
    WaitForSecondsRealtime noticeWait;
    WaitForSecondsRealtime checkWait; 

    void Awake()
    {
        achieves = (Achieve[])System.Enum.GetValues(typeof(Achieve));
        noticeWait = new WaitForSecondsRealtime(5f);
        checkWait = new WaitForSecondsRealtime(0.5f);
        
        if (!PlayerPrefs.HasKey("MyData")) {
            Init();
        }
    }

    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);
        foreach (Achieve item in achieves) {
            PlayerPrefs.SetInt(item.ToString(), 0); 
        }
        PlayerPrefs.Save(); 
    }

    void Start()
    {
        UnlockCharacter();
        if (GameManager.instance != null) {
            StartCoroutine(CheckAchieveRoutine());
        }
    }

    void UnlockCharacter()
    {
        if (lockCharacter.Length == 0) return;

        for (int index = 0; index < lockCharacter.Length; index++)
        {
            string achieveName = achieves[index].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achieveName) == 1;
            
            if (lockCharacter[index] != null) lockCharacter[index].SetActive(!isUnlock);
            if (unlockCharacter[index] != null) unlockCharacter[index].SetActive(isUnlock);
        }
    }

    IEnumerator CheckAchieveRoutine()
    {
        while (true)
        {
            if (GameManager.instance != null)
            {
                foreach (Achieve achieve in achieves) {
                    CheckAchieve(achieve);
                }
            }
            yield return checkWait;
        }
    }

    void CheckAchieve(Achieve achieve)
    {
        bool isAchieve = false;

        switch (achieve)
        {
            case Achieve.UnlockCount: 
                isAchieve = GameManager.instance.currentState == GameManager.GameState.Victory;
                break;
            case Achieve.UnlockPower: 
                isAchieve = GameManager.instance.kill >= 50; 
                break;
        }
        
        if (isAchieve && PlayerPrefs.GetInt(achieve.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achieve.ToString(), 1);
            PlayerPrefs.Save(); 

            if (uiNotice != null) {
                for (int index = 0; index < uiNotice.transform.childCount; index++) {
                    bool isActive = (index == (int)achieve);
                    uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
                }
                StartCoroutine(NoticeRoutine());
            }
        }
    }

    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        yield return noticeWait;
        uiNotice.SetActive(false);
    }
}