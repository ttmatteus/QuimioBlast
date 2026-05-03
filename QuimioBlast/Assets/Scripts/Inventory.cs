using UnityEngine;

public class Inventory : MonoBehaviour
{

    public Item[] item;

    public GameObject mouseItem;
    public void DragItem(GameObject button)
    {
        mouseItem = button;
        mouseItem.transform.position = Input.mousePosition;
    }

    public void DropItem(GameObject button)
    {
        if (mouseItem != null)
        {

            Transform aux = mouseItem.transform.parent;
            mouseItem.transform.SetParent(button.transform.parent);
            button.transform.SetParent(aux);
        }
    }
    
}
