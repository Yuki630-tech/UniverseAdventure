using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;

public class GameOverText : MonoBehaviour
{
    [Tooltip("自身のrectTransform"), SerializeField] RectTransform rectTransform;
    [Tooltip("DOTween再生前のテキストの初期位置"), SerializeField] Vector3 startPos;
    [Tooltip("DOTweenのバウンドをさせる時間"), SerializeField] float durationToBounce;
    [Tooltip("x方向のイージング"), SerializeField] Ease textEaseX;
    [Tooltip("y方向のイージング")]
    [SerializeField] Ease textEaseY;
    private void OnEnable()
    {
        //インスペクターで設定した位置をテキストのゴール地点とする
        var endPos = rectTransform.position;
        //startPosだけ移動
        rectTransform.position += startPos;

        //イージングに従ってDOTweenアニメーション
        rectTransform.DOMoveX(endPos.x, durationToBounce).SetEase(textEaseX);
        rectTransform.DOMoveY(endPos.y, durationToBounce).SetEase(textEaseY);
    }
}
