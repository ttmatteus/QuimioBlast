using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pain�is do Menu")]
    [SerializeField] private GameObject menuInicialPanel;
    [SerializeField] private GameObject creditosPanel;

    [Header("Configura��o de Cenas")]
    [SerializeField] private string nomeDaCenaDoJogo; // Digite o nome exato da sua cena principal aqui

    void Start()
    {
        // Aplica o design robótico nos créditos caso o componente não esteja no prefab
        if (creditosPanel != null && !creditosPanel.TryGetComponent<CreditosPanelDesign>(out _))
            creditosPanel.AddComponent<CreditosPanelDesign>();

        menuInicialPanel.SetActive(true);
        creditosPanel.SetActive(false);
    }

    // Chamado pelo bot�o "Jogar"
    public void Jogar()
    {
        // Carrega a cena principal do seu jogo
        SceneTransition.Instance.LoadScene(nomeDaCenaDoJogo);
    }

    // Chamado pelo bot�o "Cr�ditos"
    public void AbrirCreditos()
    {
        menuInicialPanel.SetActive(false);
        creditosPanel.SetActive(true);
    }

    // Chamado pelo bot�o "Voltar" dentro da tela de cr�ditos
    public void FecharCreditos()
    {
        creditosPanel.SetActive(false);
        menuInicialPanel.SetActive(true);
    }

    // Chamado pelo bot�o "Sair" (Fecha o jogo)
    public void SairDoJogo()
    {
        Debug.Log("Fechando o aplicativo...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
