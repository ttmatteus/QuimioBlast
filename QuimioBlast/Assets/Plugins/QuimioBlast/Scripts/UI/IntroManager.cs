using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class IntroManager : MonoBehaviour
{
    [Header("Elementos de UI")]
    public GameObject painelIntro; // Arraste o FundoPreto
    public TextMeshProUGUI textoIntroducao; // Arraste o TextoIntroducao
    public TextMeshProUGUI textoPiscante; // Arraste o TextoPiscante

    [Header("Configurações de Tempo")]
    public float tempoAtePiscar = 10f; // Tempo total até pedir para avançar
    public float tempoDeDigitacaoTotal = 8f; // Em quantos segundos o texto todo deve aparecer

    [Header("A História")]
    [TextArea(5, 10)]
    public string historia;

    [Header("O que acontece depois?")]
    public UnityEvent aoTerminarIntro; // Onde chamaremos os outros scripts no futuro

    private bool introEncerrada = false;

    // --- FUNÇÃO PARA TESTARMOS AGORA MESMO ---
    void Start()
    {
        IniciarIntroducao();
    }
    // -----------------------------------------

    public void IniciarIntroducao()
    {
        painelIntro.SetActive(true);
        textoIntroducao.text = "";
        textoPiscante.gameObject.SetActive(false);
        introEncerrada = false;

        StartCoroutine(EscreverHistoria());
        StartCoroutine(AguardarEMostrarAviso());
    }

    void Update()
    {
        // Se a introdução estiver rodando e o jogador apertar qualquer tecla do teclado/mouse
        if (painelIntro.activeSelf && Input.anyKeyDown)
        {
            EncerrarIntroducao();
        }
    }

    private IEnumerator EscreverHistoria()
    {
        // Calcula a velocidade exata para que a história termine no tempo programado (8s)
        float tempoPorLetra = tempoDeDigitacaoTotal / historia.Length;

        foreach (char letra in historia.ToCharArray())
        {
            if (introEncerrada) yield break; // Se o jogador pulou, para de escrever

            textoIntroducao.text += letra;
            yield return new WaitForSeconds(tempoPorLetra);
        }
    }

    private IEnumerator AguardarEMostrarAviso()
    {
        // Espera os 10 segundos em silêncio
        yield return new WaitForSeconds(tempoAtePiscar);

        if (!introEncerrada)
        {
            textoPiscante.gameObject.SetActive(true);
            StartCoroutine(PiscarAviso());
        }
    }

    private IEnumerator PiscarAviso()
    {
        while (!introEncerrada)
        {
            textoPiscante.enabled = !textoPiscante.enabled;
            yield return new WaitForSeconds(0.6f); // Velocidade da piscada
        }
    }

    private void EncerrarIntroducao()
    {
        introEncerrada = true;
        StopAllCoroutines(); // Para a digitação e a piscada na mesma hora

        // Desliga o Canvas_Intro INTEIRO (fundo, textos, tudo de uma vez)
        gameObject.SetActive(false);

        // Aqui é onde o jogo avisa que a tela acabou e os personagens podem se mover
        aoTerminarIntro?.Invoke();
    }
}