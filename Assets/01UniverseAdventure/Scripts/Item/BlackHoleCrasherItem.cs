using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlackHoleCrasherItem : ItemBaseWithEvent<Player>
{
    [Tooltip("•¨—‰‰Z"), SerializeField] private Rigidbody rb;
    [Tooltip("oŒ»‚É”ò‚Ñã‚ª‚é—Í"), SerializeField] private float jumpPower;
    private void OnEnable()
    {
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

}
