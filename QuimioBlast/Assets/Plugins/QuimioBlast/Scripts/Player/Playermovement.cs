using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Configurações de Velocidade")]
    public float velocidadeBase = 7f;
    public float multiplicadorSprint = 1.5f;
    
    [Header("Configurações de Dash")]
    public float forcaDash = 20f;
    public float tempoDash = 0.2f;
    public float intervaloDoubleTap = 0.3f; // Tempo máximo entre cliques para o dash

    private Rigidbody2D rb;
    private Vector2 inputMovimento;
    private bool estaDashing;
    
    // Variáveis para detectar Double Tap
    private float tempoUltimoClique;
    private KeyCode ultimaTecla;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        if (estaDashing) return;

        ProcessarInputs();
        VerificarDoubleTap();
    }

    private void FixedUpdate()
    {
        if (estaDashing) return;

        Mover();
    }

    private void ProcessarInputs()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // Lógica de 4 eixos: Se estiver movendo no X, ignora o Y
        if (x != 0)
        {
            inputMovimento = new Vector2(x, 0);
        }
        else if (y != 0)
        {
            inputMovimento = new Vector2(0, y);
        }
        else
        {
            inputMovimento = Vector2.zero;
        }
    }

    private void Mover()
    {
        float velocidadeAtual = velocidadeBase;

        // Shift para correr
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidadeAtual *= multiplicadorSprint;
        }

        rb.linearVelocity = inputMovimento.normalized * velocidadeAtual;
    }

    private void VerificarDoubleTap()
    {
        // Lista de teclas para monitorar
        KeyCode[] teclasDirecao = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

        foreach (KeyCode tecla in teclasDirecao)
        {
            if (Input.GetKeyDown(tecla))
            {
                float tempoDesdeUltimoClique = Time.time - tempoUltimoClique;

                if (tecla == ultimaTecla && tempoDesdeUltimoClique < intervaloDoubleTap)
                {
                    StartCoroutine(ExecutarDash());
                }

                ultimaTecla = tecla;
                tempoUltimoClique = Time.time;
            }
        }
    }

    private System.Collections.IEnumerator ExecutarDash()
    {
        estaDashing = true;
        
        // Define a direção do dash baseada no input atual ou na última tecla pressionada
        Vector2 direcaoDash = inputMovimento != Vector2.zero ? inputMovimento : ObterDirecaoPorTecla(ultimaTecla);
        
        rb.linearVelocity = direcaoDash * forcaDash;

        yield return new WaitForSeconds(tempoDash);

        estaDashing = false;
    }

    private Vector2 ObterDirecaoPorTecla(KeyCode tecla)
    {
        switch (tecla)
        {
            case KeyCode.W: case KeyCode.UpArrow: return Vector2.up;
            case KeyCode.S: case KeyCode.DownArrow: return Vector2.down;
            case KeyCode.A: case KeyCode.LeftArrow: return Vector2.left;
            case KeyCode.D: case KeyCode.RightArrow: return Vector2.right;
            default: return Vector2.zero;
        }
    }
}