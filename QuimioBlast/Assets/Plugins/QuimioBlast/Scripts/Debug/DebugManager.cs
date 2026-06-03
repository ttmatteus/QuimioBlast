
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevDebug2D : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
