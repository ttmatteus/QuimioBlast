using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class MainMenuManager : MonoBehaviour
{
    [Header("Painéis do Menu")]
    [SerializeField] private GameObject menuInicialPanel;
    [SerializeField] private GameObject creditosPanel;

    [Header("Configuração de Cenas")]
    [SerializeField] private string nomeDaCenaDoJogo; // Digite o nome exato da sua cena principal aqui

    void Start()
    {
        // Garante que o menu começa na tela inicial e os créditos escondidos
        menuInicialPanel.SetActive(true);
        creditosPanel.SetActive(false);
    }

    // Chamado pelo botão "Jogar"
    public void Jogar()
    {
        // Carrega a cena principal do seu jogo
        SceneManager.LoadScene(nomeDaCenaDoJogo);
    }

    // Chamado pelo botão "Créditos"
    public void AbrirCreditos()
    {
        menuInicialPanel.SetActive(false);
        creditosPanel.SetActive(true);
    }

    // Chamado pelo botão "Voltar" dentro da tela de créditos
    public void FecharCreditos()
    {
        creditosPanel.SetActive(false);
        menuInicialPanel.SetActive(true);
    }

    // Chamado pelo botão "Sair" (Fecha o jogo)
    public void SairDoJogo()
    {
        Debug.Log("Fechando o aplicativo...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
