using UnityEngine;
using UnityEngine.UI;

public class ScreenDamageEffect : MonoBehaviour
{
    public Image damageOverlay;
    public float fadeSpeed = 2f;

    private float targetAlpha = 0f;

    private void Update()
    {
        Color c = damageOverlay.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        damageOverlay.color = c;
    }

    public void ShowDamage()
    {
        targetAlpha = 0.5f; // intensidade da borda vermelha
        Invoke("HideDamage", 0.2f);
    }

    void HideDamage()
    {
        targetAlpha = 0f;
    }
}