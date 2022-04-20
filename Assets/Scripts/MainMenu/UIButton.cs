using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioClip hoverSoundEffect;
    public AudioClip clickSoundEffect;

    [HideInInspector] public Vector3 position;

    TextMeshProUGUI text;
    float charSpacing;

    public UnityEvent OnClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSoundEffect) MainMenuController.instance.audioSource.PlayOneShot(hoverSoundEffect);
        if (text) text.characterSpacing = charSpacing + 10;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text) text.characterSpacing = charSpacing;
    }

    private void Awake()
    {
        position = transform.position;
        TryGetComponent(out text);
        if (text) charSpacing = text.characterSpacing;
        GetComponent<Button>().onClick.AddListener(ClickAnimation);
    }

    public void ClickAnimation()
    {
        if (clickSoundEffect) MainMenuController.instance.audioSource.PlayOneShot(clickSoundEffect);
        transform.DOScale(1.3f, .075f).From(1).OnComplete(() => transform.DOScale(1, .075f).OnComplete(() => OnClick.Invoke()));
    }

}
