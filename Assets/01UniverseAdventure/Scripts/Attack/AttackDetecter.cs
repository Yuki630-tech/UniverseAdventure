using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetecter : MonoBehaviour
{
    [Tooltip("攻撃力とノックバックベクトルのデータ"), SerializeField] AttackData data;

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack")) return;
        var attackReceiver = other.GetComponent<AttackReceiver>();

        if(attackReceiver != null)
        {
            //レシーバー側にダメージを与える
            attackReceiver.OnReceivedDamage(data);
        }
    }
}
