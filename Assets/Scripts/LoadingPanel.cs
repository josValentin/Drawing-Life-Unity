using DG.Tweening;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(bool animated = false)
    {
        gameObject.SetActive(true);

        if (animated)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.5f);
        }
        else
        {
            canvasGroup.alpha = 1;
        }
    }

    public void Hide(bool animated = true)
    {
        if (!animated)
        {
            gameObject.SetActive(false);
            return;
        }

        canvasGroup.DOFade(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
}