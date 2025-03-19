using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Références")]
    [SerializeField] private Rigidbody2D rb;

    public Animator animator;

    private Vector2 moveDirection;
    public bool isFacingRight = true;
    public bool isAttacking = false;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        // Récupérer le PlayerInput et l'enregistrer auprès de l'InputManager
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.SetPlayerInput(playerInput);
            }
            else
            {
                Debug.LogError("InputManager is not in the scene");
            }
        }
        else
        {
            Debug.LogError("Missing PlayerInput on GameObject");
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;

        // Gestion de l'orientation du sprite
        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // On utilise FixedUpdate pour le mouvement physique
        Move();
        float charVelocity = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", charVelocity);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Move()
    {
        // Mouvement avec Rigidbody2D pour une meilleure physique
        if (rb)
        {
            // Option 1: Velocity directe (mouvement plus direct)
            rb.linearVelocity = moveDirection * moveSpeed;
            //Debug.Log("moveDirection="+ moveDirection);
            // Option 2: Force (physique plus réaliste mais plus lente)
            // rb.AddForce(moveDirection * moveSpeed);

            // Option 3: MovePosition (plus précis mais moins d'interactions physiques)
            // Vector2 position = rb.position;
            // position += moveDirection * moveSpeed * Time.fixedDeltaTime;
            // rb.MovePosition(position);
        }
        else
        {
            Debug.LogError("Rigidbody2D is missing on PlayerController");
        }
    }
    private void Flip()
    {
        // Inverser l'orientation du sprite
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    public void OnAttack()
    {
        Debug.Log("Attaque en cours");
        isAttacking = true;
        animator.SetTrigger("Attack");

        Transform child = transform.Find("SwordInteract");
        BoxCollider2D swordBox = child.GetComponent<BoxCollider2D>();
        swordBox.enabled=true;
        // récupération du capsule collider du player pour l'agrandir pendant l'attaque
        // laissé tomber car pas propre (on risque de taper par l'arrière comme le capsule collider déborde...
        //CapsuleCollider2D cc = GetComponent<CapsuleCollider2D>();
        //cc.size = new Vector2(3f, 1.5f);
        // Rigidbody en Kinematic pour que l'épée passe à travers du gift destructible lors d'une attaque
        //rb.bodyType = RigidbodyType2D.Kinematic;

        // ajout d'une box lors de l'attaque
        /*
        BoxCollider2D attackBox = gameObject.AddComponent<BoxCollider2D>();
        attackBox.size = new Vector2(1.0f, 1f); // Exemple: largeur de 2.0 et hauteur de 4.0
        attackBox.offset = new Vector2(1f, -0.5f); // Exemple: décalage de 0.5 sur l'axe X et Y
        // permet de rendre la box "traversable"
        attackBox.isTrigger = true;
        */
    }

    public void OnStopAttack()
    {
        isAttacking = false;
        Transform child = transform.Find("SwordInteract");
        BoxCollider2D swordBox = child.GetComponent<BoxCollider2D>();
        swordBox.enabled = false;

        /*
        CapsuleCollider2D cc = player.GetComponent<CapsuleCollider2D>();
        cc.size = new Vector2(0.9f, 1.47f);
        // Rigidbody en Kinematic pour que l'épée passe à travers du gift destructible lors d'une attaque
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        //Debug.Log("fin de l'attaque");
        */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("OnCollisionEnter2D ok");
        // Vérifiez avec quel objet la collision s'est produite
        if(collision!=null) 
        {
            if(collision.gameObject.name == "Coin(Clone)")
            {
                //Debug.Log("Collision détectée avec : " + collision.gameObject.name);
                Coin monCoin = collision.gameObject.GetComponent<Coin>();

                if (InventoryManager.Instance == null)
                    Debug.LogError("InventoryManager.Instance n'est pas initialisé !");
                else
                {
                    bool testAjout = InventoryManager.Instance.AddItem(monCoin.item, 1);
                    //Debug.Log("testAjout = "+testAjout);
                    Destroy(collision.gameObject);
                    //Debug.Log(InventoryManager.Instance);
                }
            }

            if (collision.gameObject.name == "Grass(Clone)")
            {
                Debug.Log("Herbe touchée");
                Grass monGrass = collision.gameObject.GetComponent<Grass>();
                if (InventoryManager.Instance == null)
                    Debug.LogError("InventoryManager.Instance n'est pas initialisé !");
                else
                {
                    bool testAjout = InventoryManager.Instance.AddItem(monGrass.item, 1);
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}