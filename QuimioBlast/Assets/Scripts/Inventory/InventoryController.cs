using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    // Requisito obrigatório: HashMap/Dictionary usando a chave única (ID/Nome)
    // O valor armazena a referência do item (que contém o nome/sprite) e a quantidade.
    public class InventoryRecord
    {
        public Objects itemData;
        public int quantity;
    }

    // A chave do dicionário agora é uma string (representando o ID ou Nome do item)
    private Dictionary<string, InventoryRecord> inventory = new Dictionary<string, InventoryRecord>();

    [Header("Configurações de UI")]
    public Image[] slotImages;

    [Header("Limites do Inventário")]
    public int maxSlots = 5; // Requisito atendido: até 5 espaços de itens diferentes
    public int limitePorSlot = 10;

    [Header("Teste")]
    public Objects testItemToPickup;

    private InterfaceController iController;
    private PlayerHealth playerHealth;
    private Objects itemProximo;
    private GameObject objetoFisicoProximo;

    // Teclas da hotbar mapeadas aos índices dos slots (0 = tecla 1, ..., 4 = tecla 5)
    private readonly KeyCode[] hotbarKeys = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    void Start()
    {
        iController = FindAnyObjectByType<InterfaceController>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerHealth == null)
            Debug.LogWarning("[InventoryController] PlayerHealth não encontrado na cena. Itens consumíveis não funcionarão.");

        UpdateInventoryUI();
    }

    void Update()
    {
        // Hotbar (1-5): usar itens consumíveis quando o inventário estiver fechado
        if (iController == null || !iController.invActive)
        {
            for (int i = 0; i < hotbarKeys.Length; i++)
            {
                if (Input.GetKeyDown(hotbarKeys[i]))
                {
                    UseItemInSlot(i);
                }
            }
        }

        // Tecla E: Coleta de item no chão
        if (itemProximo != null && Input.GetKeyDown(KeyCode.E))
        {
            AddItem(itemProximo);
            Destroy(objetoFisicoProximo);
            itemProximo = null;
            objetoFisicoProximo = null;
            if (iController != null) iController.itemText.text = "";
        }
    }

    /// <summary>
    /// Usa o item no slot da hotbar indicado pelo índice (0 = tecla 1, ...).
    /// Aplica o efeito do item consumível no jogador e decrementa a quantidade.
    /// </summary>
    public void UseItemInSlot(int index)
    {
        // Converte o índice da hotbar na chave do dicionário (por ordem de inserção)
        var keys = new System.Collections.Generic.List<string>(inventory.Keys);
        if (index < 0 || index >= keys.Count)
        {
            Debug.Log($"[Inventário] Slot {index + 1} está vazio.");
            return;
        }

        string itemID = keys[index];
        InventoryRecord record = inventory[itemID];

        if (!record.itemData.isConsumable)
        {
            Debug.Log($"[Inventário] {itemID} não é um item consumível.");
            return;
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("[Inventário] PlayerHealth não encontrado. Não foi possível usar o item.");
            return;
        }

        if (playerHealth.IsFullHealth())
        {
            Debug.Log($"[Inventário] Vida já está cheia! {itemID} não foi consumido.");
            return;
        }

        // Aplica o efeito
        playerHealth.Heal(record.itemData.healAmount);
        Debug.Log($"[Inventário] {itemID} usado! +{record.itemData.healAmount} de vida.");

        // Decrementa quantidade e remove do inventário se chegar a zero
        record.quantity--;
        if (record.quantity <= 0)
        {
            inventory.Remove(itemID);
            Debug.Log($"[Inventário] {itemID} acabou e foi removido do inventário.");
            SortInventoryAlphabetically();
        }
        UpdateInventoryUI();
    }

    // --- REQUISITOS OBRIGATÓRIOS DO EXERCÍCIO ---

    // 1. Adicionar Item (Atualiza a quantidade se existir, cria registro se não existir)
    public void AddItem(Objects itemToAdd)
    {
        string itemID = itemToAdd.itemName; // Utilizando o nome como Chave Única/ID

        if (inventory.ContainsKey(itemID))
        {
            if (inventory[itemID].quantity < limitePorSlot)
            {
                inventory[itemID].quantity++;
                Debug.Log($"Quantidade do item '{itemID}' atualizada para {inventory[itemID].quantity}.");
                UpdateInventoryUI();
            }
            else
            {
                Debug.Log($"Limite por slot atingido para o item '{itemID}'.");
            }
        }
        else
        {
            if (inventory.Count < maxSlots)
            {
                inventory.Add(itemID, new InventoryRecord { itemData = itemToAdd, quantity = 1 });
                Debug.Log($"Novo registro criado: Item '{itemID}' adicionado ao inventário.");
                UpdateInventoryUI();
            }
            else
            {
                Debug.Log($"Inventário cheio! Limite máximo de {maxSlots} itens atingido.");
            }
        }
    }

    // 2. Consultar Item (Informa se existe e a quantidade)
    public void ConsultItem(string searchID)
    {
        if (inventory.ContainsKey(searchID))
        {
            int qtd = inventory[searchID].quantity;
            Debug.Log($"Consulta: O item '{searchID}' EXISTE. Quantidade armazenada: {qtd}.");
        }
        else
        {
            Debug.Log($"Consulta: O item '{searchID}' NÃO foi encontrado no inventário.");
        }
    }

    // 3. Exibir Inventário (Lista todos os itens armazenados)
    public void ShowInventory()
    {
        Debug.Log("--- LISTA COMPLETA DO INVENTÁRIO ---");
        if (inventory.Count == 0)
        {
            Debug.Log("O inventário está vazio.");
            return;
        }

        foreach (var entry in inventory)
        {
            Debug.Log($"ID/Nome: {entry.Key} | Quantidade: {entry.Value.quantity}");
        }
        Debug.Log("------------------------------------");
    }

    // 4. Remover Item (Permite remover uma quantidade ou o item completo)
    public void RemoveItem(string itemID, int amountToRemove = 1, bool removeCompletely = false)
    {
        if (inventory.ContainsKey(itemID))
        {
            if (removeCompletely)
            {
                inventory.Remove(itemID);
                Debug.Log($"O item '{itemID}' foi removido COMPLETAMENTE do inventário.");
            }
            else
            {
                inventory[itemID].quantity -= amountToRemove;
                if (inventory[itemID].quantity <= 0)
                {
                    inventory.Remove(itemID);
                    Debug.Log($"As unidades acabaram. O item '{itemID}' foi removido do inventário.");
                }
                else
                {
                    Debug.Log($"Removida(s) {amountToRemove} unidade(s) de '{itemID}'. Restam: {inventory[itemID].quantity}.");
                }
            }
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log($"Remoção falhou: O item '{itemID}' não existe no inventário.");
        }
    }

    // --- ATUALIZAÇÃO E FUNÇÕES DE INTERFACE ---

    private void UpdateInventoryUI()
    {
        ClearAllSlotUI();
        int i = 0;

        // Iteramos sobre os valores armazenados no dicionário para refletir na UI
        foreach (var record in inventory.Values)
        {
            if (i < slotImages.Length && i < maxSlots)
            {
                slotImages[i].sprite = record.itemData.itemSprite;
                slotImages[i].color = Color.white;
                i++;
            }
        }
    }

    private void ClearAllSlotUI()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] != null)
            {
                slotImages[i].sprite = null;
                slotImages[i].color = new Color(1, 1, 1, 0); // Fica transparente
            }
        }
    }

    public void SortInventoryAlphabetically()
    {
        // Reordena o dicionário com base no nome e reconstrói o HashMap
        inventory = inventory.OrderBy(x => x.Value.itemData.itemName)
                             .ToDictionary(x => x.Key, x => x.Value);
        UpdateInventoryUI();
    }

    // Funções de gatilho de física (Triggers) mantidas idênticas
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            ObjectType objTypeComponent = collision.GetComponent<ObjectType>();
            if (objTypeComponent != null && objTypeComponent.objectType != null)
            {
                itemProximo = objTypeComponent.objectType;
                objetoFisicoProximo = collision.gameObject;
                if (iController != null) iController.itemText.text = "Pressione (E) para coletar " + itemProximo.itemName;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            itemProximo = null;
            objetoFisicoProximo = null;
            if (iController != null) iController.itemText.text = "";
        }
    }
}