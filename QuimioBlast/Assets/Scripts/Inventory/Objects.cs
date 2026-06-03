using UnityEngine;

[CreateAssetMenu(fileName = "New Object", menuName = "Inventory Objects/Create New")]
public class Objects : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemPrefab;

    [Header("Consumo")]
    public bool isConsumable;
    public int healAmount;
}