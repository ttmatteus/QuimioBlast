using UnityEngine;
using UnityEngine.SceneManagement;

public class DevDebug2D : MonoBehaviour
{
    public GameObject enemyPrefab; 
    public GameObject enemyInstance; 
    void Start()
    {
        if (enemyInstance == null)
        {
            enemyInstance = GameObject.Find("Enemy"); 
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (enemyInstance != null)
            {
                // Move o inimigo existente
                enemyInstance.transform.position = mousePos;
            }
            else if (enemyPrefab != null)
            {
                // Se não existe, instancia
                enemyInstance = Instantiate(enemyPrefab, mousePos, Quaternion.identity);
            }

            Debug.Log("Inimigo colocado em: " + mousePos);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}