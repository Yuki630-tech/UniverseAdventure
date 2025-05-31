using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerStatus : MonoBehaviour
{
    [Tooltip("�X�e�[�^�X�̃f�[�^"), SerializeField] PlayerStatusData playerStatusData;

    //UI�\���p�̃��A�N�e�B�u�v���p�e�B
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

        //�Q�[�����N���A���čēx�v���C����Ƃ��Ɋe�p�����[�^�������l�ɖ߂��悤�ɂ���B
        GameManager.Instance?.OnResetGameObservable.Subscribe(_ => InitialzeStatus()).AddTo(gameObject);
    }

    /// <summary>
    /// �Q�[�����X�^�[�g�������Ɋe�p�����[�^�������l�ɂ���֐�
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
    /// �v���C���[�����S���ă��X�^�[�g����ۂ�Hp�������l�ɖ߂��֐�
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
