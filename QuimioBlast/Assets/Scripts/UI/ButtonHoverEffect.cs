using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Coroutine anim;

    private void Awake() => originalScale = transform.localScale;

    public void OnPointerEnter(PointerEventData _) => Animar(1.06f, 0.1f);
    public void OnPointerExit(PointerEventData _)  => Animar(1.0f,  0.1f);
    public void OnPointerDown(PointerEventData _)  => Animar(0.95f, 0.07f);
    public void OnPointerUp(PointerEventData _)    => Animar(1.06f, 0.07f);

    private void Animar(float escala, float duracao)
    {
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(AnimarCoroutine(originalScale * escala, duracao));
    }

    private IEnumerator AnimarCoroutine(Vector3 alvo, float duracao)
    {
        Vector3 inicio = transform.localScale;
        float t = 0f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(inicio, alvo, t / duracao);
            yield return null;
        }
        transform.localScale = alvo;
    }
}
