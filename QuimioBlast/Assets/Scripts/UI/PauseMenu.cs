using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    [Header("Referências de UI")]
    [SerializeField] private GameObject pauseMenuUI; // Arraste o PauseMenuPanel aqui

    private bool isPaused = false;

    void Start()
    {
        // Procura o Canvas que está no pai ou no próprio objeto
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Encontra automaticamente a câmera principal da nova cena e atribui ao Canvas
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

    // Função para retomar o jogo (pública para o botão poder acessar)
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Esconde o menu
        Time.timeScale = 1f;          // Normaliza o tempo do jogo
        isPaused = false;
    }

    // Função para pausar o jogo
    void Pause()
    {
        pauseMenuUI.SetActive(true);  // Mostra o menu
        Time.timeScale = 0f;          // Congela o tempo (física, animações independentes, etc.)
        isPaused = true;
    }

    // Função para fechar o jogo (pública para o botão poder acessar)
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo..."); // Aparece no console para testar no editor
        Time.timeScale = 1f; //Descongela o tempo antes de mudar de cena
        SceneManager.LoadScene("InitialScene"); // Arraste ou digite o nome da cena do Menu Inicial
    }
}
