using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("# UI 패널 연결")]
    public GameObject titlePanel;          
    public GameObject characterSelectPanel; 

    void Awake()
    {
        Application.targetFrameRate = 60; 
        Screen.sleepTimeout = SleepTimeout.NeverSleep; 
    }

    void Start()
    {
        titlePanel.SetActive(true);
        characterSelectPanel.SetActive(false);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBgm(true);
        }
    }

    public void ShowCharacterSelect()
    {
        titlePanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }

    public void BackToTitle()
    {
        characterSelectPanel.SetActive(false);
        titlePanel.SetActive(true);
    }

    public void SelectCharacterAndStart(int characterId)
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterId);
        PlayerPrefs.Save(); 
        
        SceneManager.LoadScene(1); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}