using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    [Header("Painéis da Tela de Início")]
    public GameObject panelPrincipal;    // Onde ficam os botões Jogar, Créditos e Sair
    public GameObject panelCreditos;  // Onde fica o texto dos desenvolvedores

    // A variável do IntroManager foi removida temporariamente nesta branch!

    public void IniciarJogo()
    {
        // Como a Intro não existe nesta branch, deixamos um aviso no console para testar o botão!
        Debug.Log("Botão JOGAR clicado! A tela de introdução será chamada aqui após o merge das branches.");

        // Quando você fizer o merge com a branch da intro, você vai adicionar aqui:
        // gameObject.SetActive(false);
        // introManager.IniciarIntroducao();
    }

    public void MostrarCreditos()
    {
        panelPrincipal.SetActive(false);
        panelCreditos.SetActive(true);
    }

    public void VoltarParaInicio()
    {
        panelCreditos.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");
        // Application.Quit() fecha o executável final do jogo. 
        // Lembre-se que dentro do Editor da Unity ele não faz nada visualmente além do Debug.Log.
        Application.Quit();
    }
}
