using UnityEngine;


public class PauseMenu : MonoBehaviour
{
    [Header("Refer�ncias de UI")]
    [SerializeField] private GameObject pauseMenuUI; // Arraste o PauseMenuPanel aqui

    private bool isPaused = false;

    void Start()
    {
        // Procura o Canvas que est� no pai ou no pr�prio objeto
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Encontra automaticamente a c�mera principal da nova cena e atribui ao Canvas
            canvas.worldCamera = Camera.main;
        }
    }

    void Update()
    {
        // Monitora se o jogador apertou a tecla ESC (comum para pause)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Fun��o para retomar o jogo (p�blica para o bot�o poder acessar)
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Esconde o menu
        Time.timeScale = 1f;          // Normaliza o tempo do jogo
        isPaused = false;
    }

    // Fun��o para pausar o jogo
    void Pause()
    {
        pauseMenuUI.SetActive(true);  // Mostra o menu
        Time.timeScale = 0f;          // Congela o tempo (f�sica, anima��es independentes, etc.)
        isPaused = true;
    }

    // Fun��o para fechar o jogo (p�blica para o bot�o poder acessar)
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo..."); // Aparece no console para testar no editor
        SceneTransition.Instance.LoadScene("MenuPrincipal");
    }
}
