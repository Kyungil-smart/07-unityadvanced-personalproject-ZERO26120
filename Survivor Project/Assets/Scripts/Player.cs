using UnityEngine;
using UnityEngine.InputSystem; 

public class Player : MonoBehaviour
{
    public float speed;
    public Vector2 inputVec; 
    
    public Scanner scanner;
    public RuntimeAnimatorController[] animCon; 

    public bool isDead; 

    Rigidbody2D    rigid;
    SpriteRenderer spriter;
    Animator       anim;
    PlayerInput    playerInput;
    InputAction    moveAction;

    void Awake()
    {
        rigid       = GetComponent<Rigidbody2D>();
        spriter     = GetComponent<SpriteRenderer>();
        anim        = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        scanner     = GetComponent<Scanner>();
        moveAction  = playerInput.actions["Move"]; 
    }

    void OnEnable()
    {
        moveAction.performed += OnMove; 
        moveAction.canceled  += OnMove; 
    }

    void Start()
    {
        speed *= Character.Speed; 
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled  -= OnMove;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        inputVec = ctx.ReadValue<Vector2>(); 
    }

    void Update()
    {
        if (isDead) return; 
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;

        if (GameManager.instance.health <= 0)
        {
            isDead = true; 
            
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead"); 
            GetComponent<Collider2D>().enabled = false; 
            
            GameManager.instance.GameOver();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return; 
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;
        
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if (isDead) return; 
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;
        
        anim.SetFloat("Speed", inputVec.magnitude); 

        if (inputVec.x != 0)
            spriter.flipX = inputVec.x < 0; 
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return; 
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;
        if (!collision.gameObject.CompareTag("Enemy")) return;

        GameManager.instance.health -= Time.deltaTime * 10 * Character.DamageReduction;
    }
}