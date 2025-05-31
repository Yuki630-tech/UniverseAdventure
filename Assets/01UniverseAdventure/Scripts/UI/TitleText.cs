using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TitleText : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] GameObject buttonSet;
    [SerializeField] float duration;
    [SerializeField] InputManager inputManager;

    
    private void OnEnable()
    {
        buttonSet.SetActive(false);
        var endScale = rectTransform.localScale;
        rectTransform.localScale = Vector3.zero;

        rectTransform.DOScale(endScale, duration).OnComplete(() => buttonSet.SetActive(true));
        //初プレイでなければタイトルの演出をスキップできるようにする
        inputManager.IsDecided.Where(isDecided => isDecided && !(GameManager.Instance != null && GameManager.Instance.IsFirstPlay)).Subscribe(_ => gameObject.transform.DOComplete()).AddTo(gameObject);

    }
}
