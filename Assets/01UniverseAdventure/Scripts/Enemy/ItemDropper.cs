using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class ItemDropper : MonoBehaviour
{
    [Tooltip("Gravity"), SerializeField] private Gravity enemyGravity;
    [Tooltip("死亡した時に出すアイテムのリスト"), SerializeField] private List<ItemSet> itemSets = new List<ItemSet>();

    [Header("出現した後に登録すべきイベントがある場合はここに登録")]
    [SerializeField] UnityEvent setUnityEvent;
    [Serializable]
    public struct ItemSet
    {
        public GameObject Item;
        public int Number;
    }

    /// <summary>
    /// アイテムを出現させる
    /// </summary>
    public void SpawnItem()
    {
        foreach(ItemSet set in itemSets)
        {
            for(int i = 0; i < set.Number; i++)
            {
                GameObject item = Instantiate(set.Item, transform.position, transform.rotation);
                Gravity itemGravity = item.GetComponent<Gravity>();
                Gravity[] itemGravities = item.GetComponentsInChildren<Gravity>();
                itemGravity?.SetGravity(enemyGravity);
                if(itemGravities.Length > 0)
                {
                    foreach(Gravity gravity in itemGravities)
                    {
                        gravity.SetGravity(enemyGravity);
                    }
                }
                ItemBaseWithEvent<Player> itemWithEvent = item.GetComponentInChildren<ItemBaseWithEvent<Player>>();
                if(itemWithEvent != null)
                {
                    itemWithEvent.SetTouchEvent(setUnityEvent);
                }
            }
        }
    }
}
