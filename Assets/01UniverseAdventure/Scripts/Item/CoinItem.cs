using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : ItemBase<PlayerStatus>
{
    [Tooltip("éÊÇ¡ÇΩéûÇÃSE"), SerializeField] private AudioClip audioClip;
    [Tooltip("ÉRÉCÉìÇÃRigidbody"), SerializeField] private Rigidbody rb;
    [Tooltip("èoåªÇ∑ÇÈç€Ç…îÚÇ—è„Ç™ÇÈóÕ"), SerializeField] private float jumpPower;

    private void OnEnable()
    {
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    protected override void GetItem(PlayerStatus getter)
    {
        getter.Heel();
        AudioSource audioSource = getter.GetComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.PlayOneShot(audioClip);
        }

    }
}
