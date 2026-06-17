using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
// GameOverUI — painel exibido quando o Player morre (PlayerHealth.Morrer()).
//
// COMO CONFIGURAR:
//   1. Crie uma Canvas (Render Mode = Screen Space - Overlay), ex.: "CanvasGameOver".
//   2. Dentro dela, crie um Panel "GameOverPanel" cobrindo a tela (fundo
//      semitransparente), com um texto "Game Over" e dois botões:
//        - "Tentar Novamente"
//        - "Menu Principal"
//   3. Adicione este script na Canvas (ou em um objeto vazio dentro dela).
//   4. Arraste o "GameOverPanel" para o campo "Painel Game Over" abaixo
//      e desative-o na Hierarquia (ele é ativado automaticamente na morte).
//   5. No botão "Tentar Novamente": OnClick > arraste o objeto com este
//      script > GameOverUI.TentarNovamente.
//   6. No botão "Menu Principal": OnClick > arraste o objeto com este
//      script > GameOverUI.VoltarAoMenu.
//   7. Ajuste "Cena Menu Principal" se o nome da cena do menu for diferente
//      de "InitialScene".
// ─────────────────────────────────────────────────────────────────────────────
public class GameOverUI : MonoBehaviour
{
    [Tooltip("Nome da cena do menu principal, usada pelo botão 'Menu Principal'.")]
    public string cenaMenuPrincipal = "MenuPrincipal";

    [Tooltip("Painel exibido quando o Player morre. Deve começar desativado na Hierarquia.")]
    public GameObject painelGameOver;

    public static GameOverUI Instancia { get; private set; }

    private void Awake()
    {
        Instancia = this;

        if (painelGameOver != null)
            painelGameOver.SetActive(false);
    }

    /// <summary>Exibe o painel de Game Over e pausa o jogo.</summary>
    public void Mostrar()
    {
        if (painelGameOver != null)
            painelGameOver.SetActive(true);

        Time.timeScale = 0f;
    }

    // Chamado pelo botão "Tentar Novamente": recarrega a cena atual do zero.
    public void TentarNovamente()
    {
        DestruirPlayerPersistente();
        SceneTransition.Instance.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void VoltarAoMenu()
    {
        DestruirPlayerPersistente();
        SceneTransition.Instance.LoadScene(cenaMenuPrincipal);
    }

    // O InventoryManager usa DontDestroyOnLoad no Player, então o Player "morto"
    // sobreviveria ao reload. Sem isso, o InventoryManager do Player novo se
    // autodestrói (singleton já ocupado) e a câmera/inputs ficam sem referência.
    private void DestruirPlayerPersistente()
    {
        if (InventoryManager.Instancia != null)
            InventoryManager.Instancia.DestruirSingleton();
    }
}
