using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InventoryManager : MonoBehaviour
{

    [Header("Input Settings")]
    public InputActionReference inventoryAction;

    [Header("Inventory Settings")]
    public int inventorySize = 20;
    public List<InventorySlot> slots = new List<InventorySlot>();
    Dictionary<string, InventorySlot> itemsById = new Dictionary<string, InventorySlot>();
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform slotsGrid; // Le parent o� les slots seront instanci�s
    public GameObject slotPrefab; // Le prefab pour un emplacement d'inventaire

    private bool isInventoryOpen = false;

    public static InventoryManager Instance { get; private set; }
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        Debug.Log("Awake");

        // Initialiser les slots d'inventaire
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }

        // Fermer l'inventaire au d�marrage
        if (inventoryPanel != null)
        {
            Debug.Log("fermeture inventaire");
            inventoryPanel.SetActive(false);
        }

        // Configurer l'action d'input pour ouvrir l'inventaire
        if (inventoryAction != null)
        {
            inventoryAction.action.started += OnInventoryInput;
        }
    }


    private void Start()
    {
        Debug.Log("Start");

        // Cr�er les slots UI
        RefreshInventoryUI();
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        // Activer l'action d'input
        if (inventoryAction != null)
        {
            inventoryAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        // D�sactiver l'action d'input
        if (inventoryAction != null)
        {
            inventoryAction.action.Disable();
        }
    }
    private void OnDestroy()
    {
        // Nettoyer les abonnements d'�v�nements
        if (inventoryAction != null)
        {
            inventoryAction.action.started -= OnInventoryInput;
        }
    }
    private void OnInventoryInput(InputAction.CallbackContext obj)
    {
        ToggleInventory();
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;


        if (inventoryPanel)
        {
            inventoryPanel.SetActive(isInventoryOpen);
        }
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        //Debug.Log("ajout d'un item ok");
        // V�rifier si l'item est empilable
        if (item.isStackable)
        {
            //Debug.Log("item isStackable ok");
            // Chercher un slot existant avec le m�me item
            for (int i = 0; i < slots.Count; i++)
            {
                //Debug.Log("On rentre dans la boucle");
                if (slots[i].item == item && slots[i].quantity < item.maxStackSize)
                {
                    //Debug.Log("On rentre dans la condition");
                    quantity = slots[i].AddItem(item, quantity);

                    if (quantity <= 0)
                    {
                        RefreshInventoryUI();
                        return true; // Tout a �t� ajout�
                    }
                }
            }
        }

        // Chercher un slot vide
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
            {
                quantity = slots[i].AddItem(item, quantity);

                if (quantity <= 0)
                {
                    RefreshInventoryUI();
                    return true; // Tout a �t� ajout�
                }
            }
        }

        // Si on arrive ici, c'est que l'inventaire est plein
        Debug.Log("Inventaire plein, impossible d'ajouter " + item.itemName);
        RefreshInventoryUI();
        return false;
    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].RemoveItem(quantity);
                RefreshInventoryUI();
                return;
            }
        }
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            if (!slots[slotIndex].IsEmpty())
            {
                slots[slotIndex].item.Use();
                RefreshInventoryUI();
            }
        }
    }

    private void RefreshInventoryUI()
    {
        // Si l'UI n'est pas encore initialis�e, retourner
        if (slotsGrid == null || slotPrefab == null)
        {
            return;
        }

        // Supprimer tous les slots actuels pour les recr�er
        foreach (Transform child in slotsGrid)
        {
            Destroy(child.gameObject);
        }

        // Cr�er de nouveaux slots
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsGrid);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                slotUI.SetupSlot(i, slots[i]);
            }
        }
    }
}
