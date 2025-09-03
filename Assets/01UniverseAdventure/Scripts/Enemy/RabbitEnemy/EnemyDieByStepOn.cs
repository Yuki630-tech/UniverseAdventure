using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EnemyDieByStepOn : IState<EnemyBase>
{
    public void Enter(EnemyBase owner)
    {
        owner.transform.localScale = new Vector3(owner.transform.localScale.x, owner.ScaleOfDieY, owner.transform.localScale.z);
        DestroyTask(owner).Forget();
    }

    public void Update(EnemyBase owner, float deltaTime)
    {
        
    }

    public void Exit(EnemyBase owner)
    {
        
    }

    private async UniTaskVoid DestroyTask(EnemyBase owner)
    {
        owner.OnDieByStepOnSubject.OnNext(Unit.Default);
        await UniTask.Delay(TimeSpan.FromSeconds(owner.DestroyInterval));
        owner.OnDestroyByStepOnSubject.OnNext(Unit.Default);
        owner.DestroyEnemy();
        
    }

   
}
