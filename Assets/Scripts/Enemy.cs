using UnityEngine;
public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;      // Points de patrouille
    public float patrolSpeed = 2f;          // Vitesse de déplacement
    public float waitTime = 1f;           // Temps d'attente à chaque point

    private int currentPointIndex = 0;    // Index du point actuel
    private bool isWaiting = false;       // L'ennemi attend-il à un point?
    private float waitTimer = 0f;         // Compteur pour l'attente
    private Rigidbody2D rb;               // Référence au Rigidbody2D

    private enum EnemyState
    {
        Patrol,  // Première option (valeur 0 par défaut)
        Chase,    // Deuxième option (valeur 1 par défaut)
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
    public float chaseTime = 4f;          // Durée de poursuite après avoir perdu le joueur
    private Animator enemyAnim;
    private void Awake()
    {
        // Récupérer le composant Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Vérifier si des points de patrouille sont définis
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("Aucun point de patrouille défini pour " + gameObject.name);
            enabled = false;  // Désactiver ce script
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

            // Déterminer l'état en fonction de la distance au joueur
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
                // Réduire le temps de poursuite
                chaseTimer -= Time.deltaTime;

                // Si le temps est écoulé, retourner à la patrouille
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
            // Le joueur est à portée
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
        // Récupérer la position cible actuelle
        Transform playerTransform = player.transform;
        Vector2 targetPosition = playerTransform.position;

        // Calculer la direction vers la cible
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Déplacer l'ennemi vers la cible
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
        // Si l'ennemi attend à un point de patrouille
        if (isWaiting)
        {
            // Décrémenter le compteur d'attente
            waitTimer -= Time.deltaTime;

            // Si le temps d'attente est écoulé
            if (waitTimer <= 0)
            {
                isWaiting = false;  // Arrêter d'attendre

                // Passer au point suivant (en bouclant si nécessaire)
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }

            // Ne pas continuer le reste de la fonction pendant l'attente
            return;
        }

        // Récupérer la position cible actuelle
        Vector2 targetPosition = patrolPoints[currentPointIndex].position;

        // Calculer la direction vers la cible
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Déplacer l'ennemi vers la cible
        rb.linearVelocity = direction * patrolSpeed;

        // Orienter l'ennemi dans la direction du mouvement
        if (direction.x != 0)
        {
            // Un autre moyen de faire comme le Flip() de notre PlayerController
            transform.localScale = new Vector3(direction.x > 0 ? 2.5f : -2.5f, 2.5f);
        }

        // Vérifier si l'ennemi est arrivé au point de patrouille
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Arrêter le mouvement
            rb.linearVelocity = Vector2.zero;

            // Commencer à attendre
            isWaiting = true;
            waitTimer = waitTime;
        }
    }

    private void HandleAttack()
    {
        if (isWaiting)
        {
            // Décrémenter le compteur d'attente
            waitTimer -= Time.deltaTime;

            // Si le temps d'attente est écoulé
            if (waitTimer <= 0)
            {
                isWaiting = false;  // Arrêter d'attendre
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
        // timer juste après l'attaque de l'ennemi
        isWaiting = true;
        waitTimer = 5f;
        // inflige des dégats au joueur
        playerCS.TakeDamage(10);
        // animation étourdi du joueur
        // (géré par playerCS.TakeDamage()

    }

    private void OnDrawGizmosSelected()
    {
        // Dessiner le rayon de détection
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