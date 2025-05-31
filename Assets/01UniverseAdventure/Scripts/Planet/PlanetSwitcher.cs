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
            //�f���̃��X�g�̒����猻�݃v���C���[�����Ȃ��f����I�����ăv���C���[��Gravity�ɓo�^����
            await gravity.ChoosePlanet(gameObject, planets, hasImmovableEffect, hasWaitUntillGroundedEffect);
        }
        

    }
}
