using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private RectTransform window;

    public void Show()
    {
        gameObject.SetActive(true);

        if(bg != null)
        {
            bg.DOKill();
            bg.SetAlpha(0);
            bg.DOFade(0.85f, 0.3f);
        }

        window.SetLocalScale(0);
        window.DOScale(1, 0.4f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        if (bg != null)
        {
            bg.DOFade(0, 0.3f);
        }
        window.DOScale(0, 0.4f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
    }
}
