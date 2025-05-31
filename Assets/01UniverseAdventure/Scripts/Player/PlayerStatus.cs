using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerStatus : MonoBehaviour
{
    [Tooltip("ステータスのデータ"), SerializeField] PlayerStatusData playerStatusData;

    //UI表示用のリアクティブプロパティ
    [SerializeField] ReactiveProperty<float> maxHp = new ReactiveProperty<float>();
    [SerializeField] ReactiveProperty<float> hp = new ReactiveProperty<float>();
    [SerializeField] ReactiveProperty<float> stock = new ReactiveProperty<float>();

    public IReadOnlyReactiveProperty<float> MaxHp => maxHp;
    public IReadOnlyReactiveProperty<float> Hp => hp;
    public IReadOnlyReactiveProperty<float> Stock => stock;


    public PlayerStatusData PlayerStatusData { get => playerStatusData;}

    private void Awake()
    {
        InitialzeStatus();

        //ゲームをクリアして再度プレイするときに各パラメータを初期値に戻すようにする。
        GameManager.Instance?.OnResetGameObservable.Subscribe(_ => InitialzeStatus()).AddTo(gameObject);
    }

    /// <summary>
    /// ゲームがスタートした時に各パラメータを初期値にする関数
    /// </summary>
    public void InitialzeStatus()
    {
        playerStatusData.Hp = playerStatusData.MaxHp;
        maxHp.Value = playerStatusData.MaxHp;
        hp.Value = playerStatusData.Hp;
        playerStatusData.Stock = playerStatusData.InitializeStock;
        stock.Value = playerStatusData.Stock;
    }

    /// <summary>
    /// プレイヤーが死亡してリスタートする際にHpを初期値に戻す関数
    /// </summary>
    public void RestartStatus()
    {
        playerStatusData.Hp = playerStatusData.MaxHp;
        maxHp.Value = playerStatusData.MaxHp;
        hp.Value = playerStatusData.Hp;
    }

    private void Update()
    {
        
    }


    // Start is called before the first frame update

    public void TakeDamage(int damage)
    {

        if (playerStatusData != null)
        {
            playerStatusData.Hp -= damage;
            hp.Value = playerStatusData.Hp;
        }
    }

    public void ReduceStock(int amount = 1)
    {
        if(playerStatusData != null)
        {
            playerStatusData.Stock -= amount;
            stock.Value = playerStatusData.Stock;
        }
    }

    public void AddStock(int amount = 1)
    {
        if (playerStatusData != null)
        {
            playerStatusData.Stock += amount;
            stock.Value = playerStatusData.Stock;
        }
    }

    public void Heel(int amount = 1)
    {

    }


}
