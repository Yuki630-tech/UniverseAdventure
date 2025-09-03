using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackDetecter : MonoBehaviour
{
    [Tooltip("攻撃力とノックバックベクトルのデータ"), SerializeField] AttackData data;
    [Tooltip("攻撃判定のタイプ"), SerializeField] private DetecterType detecterType;

    [Header("一度当たり判定に入った人たち"), SerializeField] private List<AttackReceiver> receivers = new List<AttackReceiver>();

    public enum DetecterType
    {
        Enemy,
        StepOn,
        Spin
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack")) return;
        var attackReceivers = other.GetComponents<AttackReceiver>().ToList();
        var attackReceiver = attackReceivers.Find(attackReceiver => attackReceiver.DetecterType == detecterType);

        if(attackReceiver != null)
        {
            //レシーバー側にダメージを与える
            attackReceiver.OnReceivedDamage(data);
            if (detecterType == DetecterType.StepOn)
            {
                Physics.IgnoreCollision(transform.parent.GetComponent<Collider>(), other);
                receivers.Add(attackReceiver);
            }
        }

       
    }
}
