using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
[CreateAssetMenu(fileName = "PlayerStatus", menuName = "Status/PlayerStatus")]
public class PlayerStatusData : ScriptableObject
{
    [SerializeField] int hp;

    [SerializeField] int maxHp;

    [SerializeField] int stock;

    [SerializeField] int maxStock = 99;
    [SerializeField] int initializeStock = 5;

  

    public int Hp
    {
        get { return hp; }

        set
        {
            if(value < 0)
            {
                hp  = 0;
            }

            else if(value >= maxHp)
            {
                hp = maxHp;
            }

            else
            {
                hp = value;
            }
        }
    }

    public int MaxHp
    {
        get { return maxHp; }
        set
        {

            if(value <= 3)
            {
                maxHp = 3;
            }

            else
            {
                maxHp = 6;
            }
        }
    }

    public int Stock
    {
        get { return stock; }
        set
        {
            if(value <= 0)
            {
                stock = 0;
            }

            else if(value >= maxStock)
            {
                stock = maxStock;
            }

            else
            {
                stock = value;
            }
        }
    }

    public int InitializeStock { get => initializeStock; }
}
