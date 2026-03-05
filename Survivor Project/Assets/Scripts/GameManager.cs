using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Ready, Playing, LevelUp, GameOver, Victory }
    public GameState currentState { get; private set; }

    [Header("# 게임 정보")]
    public float gameTime;
    public float maxGameTime = 300f; 
    public int playerId;
    
    [Header("# 플레이어 스탯")]
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 10, 15, 20, 25, 35, 45, 55, 65, 80, 95, 100, 800 };

    [Header("# 필요 컴포넌트 연결")]
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform uiJoy;
    

    public event Action<GameState> OnStateChanged;
    public event Action OnExpChanged;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        int selectedId = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameStart(selectedId); 
        
        StartCoroutine(MergeExpRoutine());
    }

    void Update()
    {
        if (currentState != GameState.Playing) return;

        gameTime += Time.deltaTime;
        if (gameTime >= maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory(); 
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        
        if (currentState == GameState.Playing)
        {
            Time.timeScale = 1; 
            if (uiJoy != null) uiJoy.localScale = Vector3.one; 
        }
        else
        {
            Time.timeScale = 0; 
            if (uiJoy != null) uiJoy.localScale = Vector3.zero; 
        }

        OnStateChanged?.Invoke(currentState); 
    }

    public void GameStart(int id)
    {
        playerId = id;
    
        maxHealth = Character.MaxHealth; 
        health = maxHealth;
    
        level = 0;
        exp = 0;
        kill = 0;
        gameTime = 0f;
    
        player.gameObject.SetActive(true); 
        uiLevelUp.Select(playerId % 2); 
    
        ChangeState(GameState.Playing); 
    
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GetExp(int amount = 1)
    {
        if (currentState != GameState.Playing) return;
        
        exp += amount; 
        OnExpChanged?.Invoke(); 

        int maxExpForLevel = nextExp[Mathf.Min(level, nextExp.Length - 1)];
        if (exp >= maxExpForLevel)
        {
            level++;
            exp -= maxExpForLevel; 
            uiLevelUp.Show(); 
            ChangeState(GameState.LevelUp); 
        }
    }

    public void Resume()
    {
        ChangeState(GameState.Playing); 

        int maxExpForLevel = nextExp[Mathf.Min(level, nextExp.Length - 1)];
        if (exp >= maxExpForLevel)
        {
            level++;
            exp -= maxExpForLevel; 
            uiLevelUp.Show(); 
            ChangeState(GameState.LevelUp); 
        }
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSecondsRealtime(1.5f); 
        
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach(Enemy enemy in allEnemies)
        {
            enemy.gameObject.SetActive(false);
        }

        ChangeState(GameState.GameOver);
        
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    private IEnumerator GameVictoryRoutine()
    {
        ChangeState(GameState.Victory);
        
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach(Enemy enemy in allEnemies)
        {
            enemy.gameObject.SetActive(false);
        }
        
        yield return new WaitForSecondsRealtime(1.5f);
        
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }
    
    public void GameLobby()
    {
        SceneManager.LoadScene(0); 
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(1); 
    }

    private IEnumerator MergeExpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); 
            if (currentState != GameState.Playing) continue;

            DropItem[] allDrops = FindObjectsByType<DropItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
            if (allDrops.Length > 10)
            {
                System.Array.Sort(allDrops, (a, b) => 
                    Vector3.Distance(player.transform.position, b.transform.position)
                        .CompareTo(Vector3.Distance(player.transform.position, a.transform.position))
                );

                int totalExp = 0;
                int mergeTargetCount = allDrops.Length - 5; 
                int mergedCount = 0;

                foreach (DropItem item in allDrops)
                {
                    if (item.type == DropItem.Type.Box || item.isMagnet || item.type == DropItem.Type.Heal) continue;

                    totalExp += item.expValue;
                    item.gameObject.SetActive(false); 
                    mergedCount++;

                    if (mergedCount >= mergeTargetCount) break;
                }

                if (totalExp > 0)
                {
                    string gemName = "Exp1";
                    if (totalExp >= 10) gemName = "Exp2";
                    if (totalExp >= 30) gemName = "Exp3"; 

                    GameObject megaGem = PoolManager.instance.Get(gemName); 
                    if (megaGem != null)
                    {
                        Vector2 randomPos = UnityEngine.Random.insideUnitCircle.normalized * 5f;
                        megaGem.transform.position = (Vector2)player.transform.position + randomPos; 
                        megaGem.GetComponent<DropItem>().expValue = totalExp; 
                    }
                }
            }
        }
    }
}