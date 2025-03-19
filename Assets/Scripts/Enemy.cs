using UnityEngine;
public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;      // Points de patrouille
    public float patrolSpeed = 2f;          // Vitesse de d�placement
    public float waitTime = 1f;           // Temps d'attente � chaque point

    private int currentPointIndex = 0;    // Index du point actuel
    private bool isWaiting = false;       // L'ennemi attend-il � un point?
    private float waitTimer = 0f;         // Compteur pour l'attente
    private Rigidbody2D rb;               // R�f�rence au Rigidbody2D

    private enum EnemyState
    {
        Patrol,  // Premi�re option (valeur 0 par d�faut)
        Chase,    // Deuxi�me option (valeur 1 par d�faut)
        Attack
    }

    private EnemyState currentState;    // = EnemyState.Patrol;
    public PlayerController player;
    public PlayerCombatSystem playerCS;
    private float detectionRadius = 4f;
    public LayerMask obstacleLayer;       // Layer des obstacles

    // Variables pour la poursuite
    private bool isChasing = false;
    private float chaseTimer = 0f;
    //private float currentSpeed;
    public float chaseTime = 4f;          // Dur�e de poursuite apr�s avoir perdu le joueur
    private Animator enemyAnim;
    private void Awake()
    {
        // R�cup�rer le composant Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // V�rifier si des points de patrouille sont d�finis
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("Aucun point de patrouille d�fini pour " + gameObject.name);
            enabled = false;  // D�sactiver ce script
        }
        else
            currentState = EnemyState.Patrol;

        enemyAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (player != null)
        {
            Transform playerTransform = player.transform;
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // D�terminer l'�tat en fonction de la distance au joueur
            if (distanceToPlayer != 0f && distanceToPlayer <= detectionRadius && currentState != EnemyState.Attack)
            {
                bool hasLineOfSight = !Physics2D.Raycast(transform.position,
                (playerTransform.position - transform.position).normalized,
                distanceToPlayer, obstacleLayer);

                Debug.Log("vision hasLineOfSight=" + hasLineOfSight);
                if (hasLineOfSight)
                {
                    // Le joueur est visible, commencer la poursuite
                    currentState = EnemyState.Chase;
                    isChasing = true;
                    chaseTimer = chaseTime;
                }
                else
                {
                    isChasing = false;
                    currentState = EnemyState.Patrol;
                }
                //currentState = EnemyState.Chase;
            }

            // Si on est en poursuite mais que le joueur n'est plus visible
            if (isChasing && currentState == EnemyState.Chase && distanceToPlayer > detectionRadius)
            {
                // R�duire le temps de poursuite
                chaseTimer -= Time.deltaTime;

                // Si le temps est �coul�, retourner � la patrouille
                if (chaseTimer <= 0)
                {
                    isChasing = false;
                    currentState = EnemyState.Patrol;
                }
            }
        }
        else
        {
            /*
            GameObject tempObject = new GameObject("TempObject");
            Transform playerTransform = tempObject.transform;
            distanceToPlayer = 0f;
            */
            currentState = EnemyState.Patrol;
        }

        //Debug.Log(distanceToPlayer+" "+currentState);
        /*
        if (distanceToPlayer <= detectionRadius)
        {
            // Le joueur est � port�e
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Patrol;
        }
        */



        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
            break;

            case EnemyState.Chase:
                HandleChase();
            break;

            case EnemyState.Attack:
                HandleAttack();
            break;

            default:
                HandlePatrol();
            break;
        }
    }

    private void HandleChase()
    {
        // R�cup�rer la position cible actuelle
        Transform playerTransform = player.transform;
        Vector2 targetPosition = playerTransform.position;

        // Calculer la direction vers la cible
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // D�placer l'ennemi vers la cible
        rb.linearVelocity = direction * patrolSpeed*2;

        // Orienter l'ennemi dans la direction du mouvement
        if (direction.x != 0)
        {
            // Un autre moyen de faire comme le Flip() de notre PlayerController
            transform.localScale = new Vector3(direction.x > 0 ? 2.5f : -2.5f, 2.5f);
        }
    }

    private void HandlePatrol()
    {
        // Si l'ennemi attend � un point de patrouille
        if (isWaiting)
        {
            // D�cr�menter le compteur d'attente
            waitTimer -= Time.deltaTime;

            // Si le temps d'attente est �coul�
            if (waitTimer <= 0)
            {
                isWaiting = false;  // Arr�ter d'attendre

                // Passer au point suivant (en bouclant si n�cessaire)
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }

            // Ne pas continuer le reste de la fonction pendant l'attente
            return;
        }

        // R�cup�rer la position cible actuelle
        Vector2 targetPosition = patrolPoints[currentPointIndex].position;

        // Calculer la direction vers la cible
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // D�placer l'ennemi vers la cible
        rb.linearVelocity = direction * patrolSpeed;

        // Orienter l'ennemi dans la direction du mouvement
        if (direction.x != 0)
        {
            // Un autre moyen de faire comme le Flip() de notre PlayerController
            transform.localScale = new Vector3(direction.x > 0 ? 2.5f : -2.5f, 2.5f);
        }

        // V�rifier si l'ennemi est arriv� au point de patrouille
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Arr�ter le mouvement
            rb.linearVelocity = Vector2.zero;

            // Commencer � attendre
            isWaiting = true;
            waitTimer = waitTime;
        }
    }

    private void HandleAttack()
    {
        if (isWaiting)
        {
            // D�cr�menter le compteur d'attente
            waitTimer -= Time.deltaTime;

            // Si le temps d'attente est �coul�
            if (waitTimer <= 0)
            {
                isWaiting = false;  // Arr�ter d'attendre
                // arret de l'animation
                enemyAnim.SetTrigger("EnemyIdle");
                // retour en mode Chase
                currentState = EnemyState.Chase;
            }

            // Ne pas continuer le reste de la fonction pendant l'attente
            return;
        }

        // animation d'attaque de l'ennemi
        enemyAnim.SetTrigger("EnemyStrike");
        // timer juste apr�s l'attaque de l'ennemi
        isWaiting = true;
        waitTimer = 5f;
        // inflige des d�gats au joueur
        playerCS.TakeDamage(10);
        // animation �tourdi du joueur
        // (g�r� par playerCS.TakeDamage()

    }

    private void OnDrawGizmosSelected()
    {
        // Dessiner le rayon de d�tection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision enemy ok "+collision.gameObject.name);
        // condition si l'ennemi touche le player
        if(collision.gameObject.name=="Player" && currentState==EnemyState.Chase)
        {
            Debug.Log("Enemy : passage en mode attaque");
            currentState = EnemyState.Attack;
        }
    }
}