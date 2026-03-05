using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;
    Rigidbody2D rigid; 

    void Awake()
    {
        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>(); 
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area")) return;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos     = transform.position;
        
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = GameManager.instance.player.inputVec;
        float dirX = playerDir.x < 0 ? -1f : 1f;
        float dirY = playerDir.y < 0 ? -1f : 1f;

        switch (transform.tag)
        {
            case "Ground":
                Vector3 groundMove = Vector3.zero;
                if (diffX > diffY)
                    groundMove = Vector3.right * dirX * 40f;
                else if (diffX < diffY)
                    groundMove = Vector3.up * dirY * 40f;
                
                transform.Translate(groundMove);
                break;

            case "Enemy":
                if (GameManager.instance.currentState != GameManager.GameState.Playing) return;

                if (coll.enabled)
                {
                    Vector3 enemyMove = (playerDir * 20f) + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
                    if (rigid != null)
                        rigid.position += (Vector2)enemyMove;
                    else
                        transform.Translate(enemyMove);
                }
                break;

            case "Box":
            case "Item":
                if (GameManager.instance.currentState != GameManager.GameState.Playing) return;

                Vector3 itemMove = (playerDir * 20f) + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
                transform.position += itemMove;
                break;
        }
    }
}