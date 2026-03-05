using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float outsideOffset = 1.5f; 
    public SpawnData[] spawnData;

    private Transform playerTr;
    private Camera mainCam;
    private int level;
    private float timer;
    private float boxTimer;

    void Awake() { mainCam = Camera.main; } 
    void Start() { playerTr = GameManager.instance.player.transform; }

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;
        
        timer += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 30f);
        int currentLevel = Mathf.Min(level, spawnData.Length - 1);

        if (timer > spawnData[currentLevel].spawnTime)
        {
            timer = 0;
            Spawn(currentLevel);
        }

        boxTimer += Time.deltaTime;
        if (boxTimer > 30f)
        {
            boxTimer = 0f;
            SpawnBox();
        }
    }

    void SpawnBox()
    {
        GameObject box = PoolManager.instance.Get("Box");
        if (box != null)
        {
            Vector2 randomPos = Random.insideUnitCircle.normalized * 3f;
            box.transform.position = (Vector2)playerTr.position + randomPos;
        }
    }

    void Spawn(int currentLevel)
    {
        Vector2 spawnPos = GetOutsideScreenPos();
        SpawnData data = spawnData[currentLevel];
        GameObject enemyObj = PoolManager.instance.Get(data.poolName);
        
        if (enemyObj != null)
        {
            enemyObj.transform.position = spawnPos;
            enemyObj.GetComponent<Enemy>().Init(data, GameManager.instance.player.GetComponent<Rigidbody2D>());
        }
    }

    Vector2 GetOutsideScreenPos()
    {
        float camH = mainCam.orthographicSize;
        float camW = camH * mainCam.aspect;
        Vector2 camPos = mainCam.transform.position;

        int side = Random.Range(0, 4);
        float x = camPos.x, y = camPos.y;

        switch (side)
        {
            case 0: x = Random.Range(camPos.x - camW, camPos.x + camW); y = camPos.y + camH + outsideOffset; break;
            case 1: x = Random.Range(camPos.x - camW, camPos.x + camW); y = camPos.y - camH - outsideOffset; break;
            case 2: x = camPos.x - camW - outsideOffset; y = Random.Range(camPos.y - camH, camPos.y + camH); break;
            case 3: x = camPos.x + camW + outsideOffset; y = Random.Range(camPos.y - camH, camPos.y + camH); break;
        }
        return new Vector2(x, y);
    }
}

[System.Serializable]
public class SpawnData
{
    public string poolName; 
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}