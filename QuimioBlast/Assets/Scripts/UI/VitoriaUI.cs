using UnityEngine;

// Tela de Vitória exibida quando o Boss morre.
//
// Como configurar:
//   1. Crie uma Canvas (Render Mode = Screen Space - Overlay) na cena, ex: "CanvasVitoria".
//   2. Dentro dela, crie um Panel "VitoriaPanel" cobrindo a tela, com:
//       - Texto "Você Venceu!" (ou similar)
//       - Botão "Menu Principal"  → OnClick > VitoriaUI.VoltarAoMenu
//   3. Adicione este script na Canvas.
//   4. Arraste o "VitoriaPanel" para o campo "Painel Vitoria" e desative-o na Hierarquia.
//   5. Ajuste "Cena Menu Principal" se necessário (padrão: "MenuPrincipal").

public class VitoriaUI : MonoBehaviour
{
    [Tooltip("Nome da cena do menu principal.")]
    public string cenaMenuPrincipal = "MenuPrincipal";

    [Tooltip("Painel exibido ao vencer. Deve começar desativado na Hierarquia.")]
    public GameObject painelVitoria;

    public static VitoriaUI Instancia { get; private set; }

    private void Awake()
    {
        Instancia = this;

        if (painelVitoria != null)
            painelVitoria.SetActive(false);
    }

    public void Mostrar()
    {
        if (painelVitoria != null)
            painelVitoria.SetActive(true);

        Time.timeScale = 0f;
    }

    public void VoltarAoMenu()
    {
        DestruirPlayerPersistente();
        SceneTransition.Instance.LoadScene(cenaMenuPrincipal);
    }

    private void DestruirPlayerPersistente()
    {
        if (InventoryManager.Instancia != null)
            InventoryManager.Instancia.DestruirSingleton();
    }
}
