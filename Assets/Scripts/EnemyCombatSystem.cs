using UnityEngine;
using UnityEngine.Events;

public class EnemyCombatSystem : MonoBehaviour
{
    // --- PARAMÈTRES DE SANTÉ ---
    [Header("Health Settings")]
    public int maxHealth = 50;           // Santé maximale de l'ennemi
    public int currentHealth;           // Santé actuelle (initialisée dans Start)

    // --- PARAMÈTRES DE COMBAT ---
    [Header("Combat Settings")]
    public int attackDamage = 5;         // Dégâts infligés par chaque attaque
    public float attackRange = 1.0f;     // Portée de l'attaque (en unités Unity)
    public float attackCooldown = 1.0f;  // Temps minimum entre deux attaques (en secondes)

    // --- PARAMÈTRES DE RÉCOMPENSE ---
    [Header("Reward Settings")]
    public int experienceReward = 10;    // Points d'expérience donnés au joueur à la mort
    public GameObject[] possibleDrops;   // Tableau d'objets que l'ennemi peut laisser tomber
    [Range(0, 1)]                        // [Range] limite la valeur entre 0 et 1
    public float dropChance = 0.3f;      // Probabilité de laisser tomber un objet (30% par défaut)

    // --- ÉVÉNEMENTS ---
    // Les UnityEvents permettent de connecter des fonctions dans l'inspecteur
    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;  // Déclenché quand la santé change (paramètres: santé actuelle, santé max)
    public UnityEvent onEnemyDeath;               // Déclenché quand l'ennemi meurt

    // --- VARIABLES PRIVÉES ---
    private float lastAttackTime;        // Moment de la dernière attaque (pour le cooldown)
    private Animator animator;           // Référence au composant Animator
    private Enemy enemy;             // Référence au composant Enemy

    // Awake est appelé quand l'objet est initialisé, avant Start
    private void Awake()
    {
        // Récupérer les composants nécessaires sur ce même GameObject
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    // Start est appelé avant la première frame
    private void Start()
    {
        // Initialiser la santé actuelle à la santé maximale
        currentHealth = maxHealth;

        // Déclencher l'événement onHealthChanged pour initialiser l'UI
        // Le ? avant Invoke est un "opérateur de propagation nulle"
        // qui vérifie si onHealthChanged n'est pas null avant d'appeler Invoke
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Méthode appelée quand l'ennemi prend des dégâts
    public void TakeDamage(int damage)
    {
        // Réduire la santé par le montant de dégâts
        currentHealth -= damage;

        // Déclencher l'événement de changement de santé
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        // Jouer l'animation de dégât si un Animator existe
        /*
        if (animator != null)
        {
            animator.SetTrigger("EnemyHit");
        }
        */

        // Vérifier si l'ennemi est mort (santé ≤ 0)
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Méthode pour attaquer le joueur
    public void Attack(PlayerCombatSystem player)
    {
        // Vérifier si le cooldown est terminé 
        if (Time.time - lastAttackTime < attackCooldown || player.currentHealth <= 0)
            return;  // Sortir de la fonction si le cooldown n'est pas terminé

        // Mettre à jour le temps de la dernière attaque
        lastAttackTime = Time.time;

        // Jouer l'animation d'attaque si un Animator existe
        if (animator)
            animator.SetTrigger("EnemyStrike");

        // Infliger des dégâts au joueur
        player.TakeDamage(attackDamage);
    }

    // Méthode privée appelée quand l'ennemi meurt
    private void Die()
    {
        // Jouer l'animation de mort si un Animator existe
        if (animator != null)
        {
            animator.SetTrigger("EnemyDie");
        }

        // Désactiver l'IA pour que l'ennemi arrête de bouger
        if (enemy != null)
        {
            enemy.enabled = false;
        }

        // Déclencher l'événement de mort
        onEnemyDeath?.Invoke();

        // Donner de l'expérience au joueur
        PlayerCombatSystem player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCombatSystem>();
        if (player != null)
        {
            // Si vous avez un système d'expérience, ajoutez l'expérience ici
            // player.AddExperience(experienceReward);

            // Pour l'instant, on peut juste l'afficher dans la console
            Debug.Log("Joueur reçoit " + experienceReward + " points d'expérience");
        }

        // Potentiellement faire tomber un objet
        DropItem();

        // Détruire l'ennemi après un délai pour laisser l'animation se jouer
        Destroy(gameObject, 2.0f);
    }

    // Méthode privée pour faire tomber un objet aléatoire
    private void DropItem()
    {
        // Si aucun objet possible ou le tirage au sort échoue, ne rien faire
        if (possibleDrops.Length == 0 || Random.value > dropChance)
            return;

        // Sélectionner un objet aléatoire parmi les possibles
        int randomIndex = Random.Range(0, possibleDrops.Length);
        GameObject drop = possibleDrops[randomIndex];

        // Instantier (créer) l'objet à la position de l'ennemi
        if (drop != null)
        {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }

    // Méthode publique pour obtenir la santé actuelle (pourcentage)
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    // Méthode publique pour restaurer de la santé (utile pour les objets de soin)
    public void RestoreHealth(int amount)
    {
        // Augmenter la santé sans dépasser le maximum
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // Mettre à jour l'UI
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
