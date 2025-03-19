using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayHUD : MonoBehaviour
{
    [Header("Player Health Bar")]
    public Slider healthSlider;              // R�f�rence au slider pour la barre de vie
    public TextMeshProUGUI healthText;       // Texte affichant les valeurs num�riques de sant�

    [Header("Player Mana Bar")]
    public Slider manaSlider;                // R�f�rence au slider pour la barre de mana
    public TextMeshProUGUI manaText;         // Texte affichant les valeurs num�riques de mana

    private PlayerCombatSystem playerCombatSystem;  // R�f�rence au syst�me de combat du joueur

    private void Start()
    {
        // Trouver le joueur et r�cup�rer son composant PlayerCombatSystem
        playerCombatSystem = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCombatSystem>();

        if (playerCombatSystem != null)
        {
            // S'abonner aux �v�nements de changement de sant� et de mana
            playerCombatSystem.onHealthChanged.AddListener(UpdateHealthUI);
            playerCombatSystem.onManaChanged.AddListener(UpdateManaUI);

            // Initialiser l'UI avec les valeurs actuelles
            UpdateHealthUI(playerCombatSystem.currentHealth, playerCombatSystem.maxHealth);
            UpdateManaUI(playerCombatSystem.currentMana, playerCombatSystem.maxMana);
        }
    }

    // Mise � jour de l'UI de sant�
    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider == null || healthText == null)
        {
            return;
        }

        if (healthSlider.maxValue != maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }

        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    // Mise � jour de l'UI de mana
    private void UpdateManaUI(int currentMana, int maxMana)
    {
        if (manaSlider == null || manaText == null)
        {
            return;
        }

        if (manaSlider.maxValue != maxMana)
        {
            manaSlider.maxValue = maxMana;
        }

        manaSlider.value = currentMana;
        manaText.text = currentMana.ToString();
    }

}
