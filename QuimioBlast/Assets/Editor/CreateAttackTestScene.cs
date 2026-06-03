using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CreateAttackTestScene
{
    [MenuItem("QuimioBlast/Criar Cena de Teste de Ataque")]
    public static void Criar()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Câmera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        cam.transform.position = new Vector3(0, 0, -10);
        camGO.tag = "MainCamera";

        // Player
        var playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.transform.position = Vector3.zero;

        var playerRb = playerGO.AddComponent<Rigidbody2D>();
        playerRb.gravityScale = 0f;
        playerRb.freezeRotation = true;

        playerGO.AddComponent<CircleCollider2D>().radius = 0.4f;
        playerGO.AddComponent<PlayerMovement2D>();

        var ataque = playerGO.AddComponent<PlayerAttack>();
        ataque.dano = 25f;
        ataque.alcance = 1.2f;
        ataque.cooldown = 0.5f;

        var playerSR = playerGO.AddComponent<SpriteRenderer>();
        playerSR.sprite = CriarSprite();
        playerSR.color = Color.cyan;

        // Inimigos
        CriarInimigo(new Vector3(2f,  0f, 0f), "Inimigo_Direita");
        CriarInimigo(new Vector3(-2f, 0f, 0f), "Inimigo_Esquerda");
        CriarInimigo(new Vector3(0f,  2f, 0f), "Inimigo_Cima");
        CriarInimigo(new Vector3(0f, -2f, 0f), "Inimigo_Baixo");

        string caminho = "Assets/Scenes/Testing/AttackTestScene.unity";
        EditorSceneManager.SaveScene(scene, caminho);
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(caminho);

        Debug.Log("[QuimioBlast] Cena de teste criada!");
    }

    private static void CriarInimigo(Vector3 posicao, string nome)
    {
        var go = new GameObject(nome);
        go.transform.position = posicao;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        go.AddComponent<CircleCollider2D>().radius = 0.4f;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CriarSprite();
        sr.color = Color.red;

        go.AddComponent<DummyEnemy>();
    }

    private static Sprite CriarSprite()
    {
        var tex = new Texture2D(32, 32);
        var pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
    }
}
