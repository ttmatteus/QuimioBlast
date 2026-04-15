using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    // --- UI and Data Variables ---
    public Objects[] slots;
    public Image[] slotImage;
    public int[] slotAmount;

    private InterfaceController iController;

    [Header("Debug Testing")]
    public Objects testItemToPickup; 

    // --- NEW: 2D Interaction Variables ---
    [Header("2D Interaction Settings")]
    public Transform playerTransform; 
    public float interactionRadius = 1.5f; 

    // --- Struct for Custom Sorting ---
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
        // Debug inputs for testing
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (testItemToPickup != null) AddItem(testItemToPickup);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SortInventoryAlphabetically();
        }

        // Guard Clause: Block interaction if inventory is open
        if (iController != null && iController.invActive) 
        {
            return;
        }

        HandleInteractions();
    }

    // --- 2D Proximity Interaction Logic ---
    private void HandleInteractions()
    {
        // Guard clause: ensure we have a player reference before calculating distance
        if (playerTransform == null) return;

        // Evaluates a circle area on the 2D plane
        Collider2D hit = Physics2D.OverlapCircle(playerTransform.position, interactionRadius);
        
        if(hit != null)
        {
            if(hit.CompareTag("Object")) 
            {
                ObjectType objTypeComponent = hit.GetComponent<ObjectType>();
                
                if (objTypeComponent != null && objTypeComponent.objectType != null)
                {
                    Objects currentObj = objTypeComponent.objectType;
                    iController.itemText.text = "Pressione (E) para coletar " + currentObj.name;

                    if(Input.GetKeyDown(KeyCode.E))
                    {
                        AddItem(currentObj); 
                        Destroy(hit.gameObject);
                        iController.itemText.text = ""; 
                    }
                }
            } 
            else 
            {
                iController.itemText.text = "";
            }
        }
        else
        {
            iController.itemText.text = "";
        }
    }

    // --- Inventory Data Management ---
    public void AddItem(Objects itemToAdd)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i] == null || slots[i].name == itemToAdd.name)
            {
                slots[i] = itemToAdd;
                slotAmount[i]++;
                
                slotImage[i].sprite = slots[i].itemSprite;
                slotImage[i].color = Color.white; 
                break; 
            }
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

    // --- Custom Quick Sort Implementation ---
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