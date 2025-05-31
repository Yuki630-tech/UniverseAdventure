using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSwitcher : MonoBehaviour
{

    [SerializeField] List<GameObject> planets = new List<GameObject>();
    [SerializeField] bool hasImmovableEffect;
    [SerializeField] bool hasWaitUntillGroundedEffect;
    Gravity gravity;
    [SerializeField] bool isPlayerOnly;

    private void Awake()
    {
        
    }
    private async void OnTriggerEnter(Collider other)
    {
        if (isPlayerOnly && !other.CompareTag("Player")) return;
        gravity = other.GetComponent<Gravity>();
        if(gravity != null)
        {
            //惑星のリストの中から現在プレイヤーがいない惑星を選択してプレイヤーのGravityに登録する
            await gravity.ChoosePlanet(gameObject, planets, hasImmovableEffect, hasWaitUntillGroundedEffect);
        }
        

    }
}
