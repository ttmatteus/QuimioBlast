using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{

    public GameObject inventoryPanel;
    public Text itemText;
    public bool invActive = false;
    void Start()
    {
        inventoryPanel.SetActive(false);
        invActive = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        invActive = !invActive;
        inventoryPanel.SetActive(invActive);

        if(invActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
