using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Necessário para Dictionary
using System.Linq;               // Necessário para ordenação facilitada

public class InventoryController : MonoBehaviour
{
    // O Dictionary armazena o Item (chave) e a Quantidade (valor)
    private Dictionary<Objects, int> inventory = new Dictionary<Objects, int>();

    [Header("Configurações de UI")]
    public Image[] slotImages; // Arraste os componentes de imagem aqui no Inspector
    public int maxSlots = 10;
    public int limitePorSlot = 10;

    [Header("Teste")]
    public Objects testItemToPickup;

    private InterfaceController iController;
    private Objects itemProximo;
    private GameObject objetoFisicoProximo;

    void Start()
    {
        iController = FindAnyObjectByType<InterfaceController>();
        UpdateInventoryUI();
    }

    void Update()
    {
        if (iController != null && iController.invActive) return;

        // Tecla 1: Adicionar item de teste
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (testItemToPickup != null) AddItem(testItemToPickup);
        }

        // Tecla 2: Ordenar (Agora muito mais simples com LINQ)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SortInventoryAlphabetically();
        }

        // Tecla E: Coleta
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
        // Verifica se o item já existe no HashMap
        if (inventory.ContainsKey(itemToAdd))
        {
            if (inventory[itemToAdd] < limitePorSlot)
            {
                inventory[itemToAdd]++;
                UpdateInventoryUI();
                return;
            }
        }

        // Se não existe, verifica se ainda há espaço em slots
        if (inventory.Count < slotImages.Length)
        {
            inventory.Add(itemToAdd, 1);
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log("Inventário cheio!");
        }
    }

    // Com HashMap, a UI deve ser atualizada como um reflexo dos dados
    private void UpdateInventoryUI()
    {
        // Primeiro, limpamos todos os slots visuais
        ClearAllSlotUI();

        int i = 0;
        foreach (var entry in inventory)
        {
            if (i < slotImages.Length)
            {
                slotImages[i].sprite = entry.Key.itemSprite;
                slotImages[i].color = Color.white;
                // Se você tiver um texto de quantidade, atualizaria aqui:
                // slotText[i].text = entry.Value.ToString();
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
                slotImages[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void SortInventoryAlphabetically()
    {
        // Com Dictionary e LINQ, a ordenação é feita em uma linha
        inventory = inventory.OrderBy(x => x.Key.itemName)
                             .ToDictionary(x => x.Key, x => x.Value);

        UpdateInventoryUI();
    }
}