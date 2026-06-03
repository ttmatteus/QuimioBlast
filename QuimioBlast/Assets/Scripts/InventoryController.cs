using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public Objects[] slots;
    public Image[] slotImage;
    public int[] slotAmount;
    private InterfaceController iController;
    private PlayerHealth playerHealth;

    private Objects itemProximo;
    private GameObject objetoFisicoProximo;

    // Teclas da hotbar mapeadas aos índices dos slots
    private readonly KeyCode[] hotbarKeys = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    private struct SlotData
    {
        public Objects item;
        public int amount;
    }

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
        // Hotbar (1-5): funciona apenas com o inventário fechado
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

        // Coletar item próximo com E
        if (itemProximo != null && Input.GetKeyDown(KeyCode.E))
        {
            AddItem(itemProximo);
            Destroy(objetoFisicoProximo);
            itemProximo = null;
            objetoFisicoProximo = null;
            if (iController != null)
                iController.itemText.text = "";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            ObjectType objTypeComponent = collision.GetComponent<ObjectType>();
            if (objTypeComponent != null && objTypeComponent.objectType != null)
            {
                itemProximo = objTypeComponent.objectType;
                objetoFisicoProximo = collision.gameObject;
                iController.itemText.text = "Pressione (E) para coletar " + itemProximo.itemName;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            itemProximo = null;
            objetoFisicoProximo = null;
            if (iController != null)
                iController.itemText.text = "";
        }
    }

    /// <summary>
    /// Usa o item do slot indicado pelo índice (0 = tecla 1, 1 = tecla 2, etc).
    /// Aplica o efeito do item consumível no jogador e decrementa a quantidade.
    /// </summary>
    public void UseItemInSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        Objects item = slots[index];

        if (item == null)
        {
            Debug.Log($"[Inventário] Slot {index + 1} está vazio.");
            return;
        }

        if (!item.isConsumable)
        {
            Debug.Log($"[Inventário] {item.itemName} não é um item consumível.");
            return;
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("[Inventário] PlayerHealth não encontrado. Não foi possível usar o item.");
            return;
        }

        if (playerHealth.IsFullHealth())
        {
            Debug.Log($"[Inventário] Vida já está cheia! {item.itemName} não foi consumido.");
            return;
        }

        // Aplica o efeito
        playerHealth.Heal(item.healAmount);
        Debug.Log($"[Inventário] {item.itemName} usado! +{item.healAmount} de vida.");

        // Decrementa quantidade e limpa slot se necessário
        slotAmount[index]--;
        if (slotAmount[index] <= 0)
        {
            ClearSlot(index);
            SortInventoryAlphabetically();
        }
    }

    /// <summary>
    /// Limpa um slot do inventário (remove item e atualiza a UI).
    /// </summary>
    private void ClearSlot(int index)
    {
        slots[index] = null;
        slotAmount[index] = 0;

        if (slotImage[index] != null)
        {
            slotImage[index].sprite = null;
            slotImage[index].color = new Color(1, 1, 1, 0);
        }
    }

    public void AddItem(Objects itemToAdd)
    {
        int limitePorSlot = 10;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].itemName == itemToAdd.itemName && slotAmount[i] < limitePorSlot)
            {
                slotAmount[i]++;
                UpdateSlotUI(i);
                return;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = itemToAdd;
                slotAmount[i] = 1;
                UpdateSlotUI(i);
                return;
            }
        }

        Debug.Log("Inventário cheio!");
    }

    private void UpdateSlotUI(int index)
    {
        if (slotImage[index] != null)
        {
            slotImage[index].sprite = slots[index].itemSprite;
            slotImage[index].color = Color.white;
        }
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slotImage[i] != null)
            {
                slotImage[i].sprite = slots[i].itemSprite;
                slotImage[i].color = Color.white;
            }
        }
    }

    public void SortInventoryAlphabetically()
    {
        int validItemCount = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) validItemCount++;
        }

        SlotData[] compactedItems = new SlotData[validItemCount];
        int currentIndex = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                compactedItems[currentIndex] = new SlotData { item = slots[i], amount = slotAmount[i] };
                currentIndex++;
            }
        }

        if (compactedItems.Length > 1)
        {
            QuickSort(compactedItems, 0, compactedItems.Length - 1);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = null;
            slotAmount[i] = 0;
            if (slotImage[i] != null)
            {
                slotImage[i].sprite = null;
                slotImage[i].color = new Color(1, 1, 1, 0);
            }
        }

        for (int i = 0; i < compactedItems.Length; i++)
        {
            slots[i] = compactedItems[i].item;
            slotAmount[i] = compactedItems[i].amount;
        }

        UpdateInventoryUI();
    }

    private void QuickSort(SlotData[] array, int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(array, low, high);
            QuickSort(array, low, partitionIndex - 1);
            QuickSort(array, partitionIndex + 1, high);
        }
    }

    private int Partition(SlotData[] array, int low, int high)
    {
        SlotData pivot = array[high];
        int i = (low - 1);

        for (int j = low; j < high; j++)
        {
            if (string.Compare(array[j].item.itemName, pivot.item.itemName, System.StringComparison.Ordinal) < 0)
            {
                i++;
                SlotData temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        SlotData temp1 = array[i + 1];
        array[i + 1] = array[high];
        array[high] = temp1;
        return i + 1;
    }
}