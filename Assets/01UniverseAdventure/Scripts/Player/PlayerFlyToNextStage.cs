using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using DG;
using DG.Tweening;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class PlayerFlyToNextStage : MonoBehaviour
{
    [SerializeField] SplineAnimate splineAnimate;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip flyingSe;
    [SerializeField] Animator animator;
    [SerializeField] Player player;
    IDisposable pauseDisposable;
    IDisposable unPauseDisposable;

    public SplineAnimate SplineAnimate { get => splineAnimate; }
    private void Start()
    {
       
    }
    public void StartFlyToNextStage(SplineContainer spline)
    {
        splineAnimate.enabled = true;
        splineAnimate.Container = spline;
        if(splineAnimate.NormalizedTime > 0)
        {
            splineAnimate.NormalizedTime = 0;

        }
        //------アニメーション------------
        animator.SetBool("IsFlying", true);
        animator.SetBool("isGrounded", false);
        animator.SetFloat("Speed", 0f);

        //------スプライン------------------
        splineAnimate.ObjectUpAxis = SplineComponent.AlignAxis.YAxis;
        splineAnimate.ObjectForwardAxis = SplineComponent.AlignAxis.ZAxis;
        splineAnimate.Play();
        pauseDisposable = GameManager.Instance?.OnPauseGameObservable.Subscribe(_ => PauseSplineAnimate()).AddTo(gameObject);
        unPauseDisposable = GameManager.Instance?.OnUnPauseGameObservable.Where(_ => splineAnimate.Container != null).Subscribe(_ => UnPauseSplineAnimate()).AddTo(gameObject);

        //-------オーディオ-----------------
        audioSource.clip = flyingSe;
        audioSource.volume = 1f;
        audioSource.loop = true;
        audioSource.time = 0f;
        audioSource.Play();
        
    }

    public void UpdateFly()
    {
        if (splineAnimate.NormalizedTime >= 0.995f)
        {
            audioSource.DOFade(0f, 1f);
        }
        if (splineAnimate.NormalizedTime == 1f)
        {
            audioSource.Stop();
  
            player.ChangeToMoveState();
            splineAnimate.Container = null;
            animator.SetBool("IsFlying", false);
            pauseDisposable.Dispose();
            unPauseDisposable.Dispose();
        }
    }

    /// <summary>
    /// ポーズ時にスプラインのアニメーションを止める
    /// </summary>
    void PauseSplineAnimate()
    {
        splineAnimate.Pause();
        audioSource.Pause();
    }

    /// <summary>
    /// ポーズ解除時にスプラインアニメーションを再開
    /// </summary>
    void UnPauseSplineAnimate()
    {
        splineAnimate.Play();
        audioSource.Play();
    }
}
