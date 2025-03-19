using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthBarUI : MonoBehaviour
{
    // --- SECTION 1: D�CLARATION DES VARIABLES ---

    public Slider healthSlider;                  // R�f�rence au slider de la barre de sant�
    public Vector3 offset = new Vector3(0, 0.5f, 0);  // D�calage au-dessus de l'ennemi (en unit�s de monde)

    private EnemyCombatSystem enemyCombatSystem;        // R�f�rence au syst�me de combat de l'ennemi
    private Transform target;                    // Transform de l'ennemi � suivre
    private Camera mainCamera;


    // --- SECTION 2: INITIALISATION ---

    private void Start()
    {
        mainCamera = Camera.main;
        // R�cup�rer la r�f�rence � l'ennemi parent
        enemyCombatSystem = GetComponentInParent<EnemyCombatSystem>();

        if (enemyCombatSystem != null)
        {
            // S'abonner � l'�v�nement de changement de sant�
            enemyCombatSystem.onHealthChanged.AddListener(UpdateHealthUI);

            // Initialiser l'UI avec les valeurs actuelles
            UpdateHealthUI(enemyCombatSystem.currentHealth, enemyCombatSystem.maxHealth);

            // Stocker la r�f�rence au transform de l'ennemi
            target = enemyCombatSystem.transform;
        }
        else
        {
            Debug.LogError("EnemyHealthBarUI: EnemyCombatSystem not found on parent!");
        }

        // Positionner initialement la barre de sant� au-dessus de l'ennemi
        if (target != null)
        {
            // Pour un Canvas en World Space, nous devons positionner directement le Canvas
            // par rapport � l'ennemi
            transform.position = target.position + offset;

            // Faire face � la cam�ra
            FaceCamera();
        }
    }

    // --- SECTION 3: SUIVI DE L'ENNEMI ---

    private void LateUpdate()
    {
        if (target)
        {
            // Mettre � jour la position pour suivre l'ennemi
            transform.position = target.position + offset;

            // S'assurer que la barre de sant� fait toujours face � la cam�ra
            FaceCamera();
        }
    }

    // Faire pivoter le Canvas pour qu'il fasse face � la cam�ra
    private void FaceCamera()
    {
        if (mainCamera)
        {
            // Pour une vue top-down 2D typique, nous voulons g�n�ralement que l'UI
            // soit align�e avec l'�cran, donc nous utilisons la rotation de la cam�ra
            transform.rotation = mainCamera.transform.rotation;

            // Alternative pour les jeux 3D o� la cam�ra peut tourner dans tous les axes:
            // transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }

    // --- SECTION 4: MISE � JOUR DE L'UI ---
    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider == null)
        {
            return;
        }


        if (healthSlider.maxValue != maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }
        healthSlider.value = currentHealth;

        // Optionnellement, masquer la barre de sant� si l'ennemi est � pleine vie
        // gameObject.SetActive(currentHealth < maxHealth);

    }

    // --- SECTION 5: GESTION DE LA VISIBILIT� ---

    // Ces m�thodes seront appel�es si le Renderer de l'ennemi parent devient visible/invisible
    // Note: Pour que cela fonctionne, ce script doit �tre sur le m�me GameObject que le Renderer
    // ou vous devez utiliser l'approche avec EnemyVisibilityObserver expliqu�e plus loin
    private void OnBecameVisible()
    {
        // Activer la barre de sant�
        gameObject.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        // D�sactiver la barre de sant�
        gameObject.SetActive(false);
    }

    // --- SECTION 6: NETTOYAGE ---
    private void OnDestroy()
    {
        // Se d�sabonner de l'�v�nement pour �viter les fuites de m�moire
        if (enemyCombatSystem != null)
        {
            enemyCombatSystem.onHealthChanged.RemoveListener(UpdateHealthUI);
        }
    }
}

