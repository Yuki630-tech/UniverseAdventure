using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Tooltip("リスタート時に表示させる敵"), SerializeField] private GameObject enemy;
    [Tooltip("コインを落とすItemDropper"), SerializeField] private ItemDropper coinDropper;
    private Vector3 defaultScale;

    private void Awake()
    {
        var enemyBase = enemy.GetComponent<EnemyBase>();
        enemyBase.OnDestroyByStepOnObservable.Subscribe(_ => coinDropper.SpawnItem()).AddTo(gameObject);
        defaultScale = transform.localScale;
        GameManager.Instance.OnPlayerRestartObservable.Where(_ => !enemy.activeSelf)
            .Subscribe(_ =>
            {
                enemy.SetActive(true);
                enemy.transform.localScale = defaultScale;
            }).AddTo(gameObject);
    }

    private void Update()
    {
        coinDropper.transform.position = enemy.transform.position;
        coinDropper.transform.rotation = enemy.transform.rotation;
    }
}
