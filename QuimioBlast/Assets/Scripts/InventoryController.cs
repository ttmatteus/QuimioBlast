using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public Objects[] slots;
    public Image[] slotImage;
    public int[] slotAmount;
    private InterfaceController iController;

    public Objects testItemToPickup;

    private Objects itemProximo;
    private GameObject objetoFisicoProximo;

    private struct SlotData
    {
        public Objects item;
        public int amount;
    }

    void Start()
    {
        iController = FindAnyObjectByType<InterfaceController>();
        UpdateInventoryUI();
    }

    void Update()
    {
        if (iController != null && iController.invActive) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (testItemToPickup != null) AddItem(testItemToPickup);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SortInventoryAlphabetically();
        }

        if (itemProximo != null && Input.GetKeyDown(KeyCode.E))
        {
            AddItem(itemProximo);
            Destroy(objetoFisicoProximo);
            itemProximo = null;
            objetoFisicoProximo = null;
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