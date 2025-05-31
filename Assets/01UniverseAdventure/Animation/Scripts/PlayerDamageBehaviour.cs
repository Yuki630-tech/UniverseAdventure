using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDamageBehaviour : StateMachineBehaviour
{
    PlayerDamage playerDamage;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerDamage = animator.gameObject.GetComponent<PlayerDamage>();
    }

    public override async void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        await UniTask.WaitUntil(() => GameManager.Instance != null && !GameManager.Instance.IsPausing);
        playerDamage.OnRecovary();
    }
}
