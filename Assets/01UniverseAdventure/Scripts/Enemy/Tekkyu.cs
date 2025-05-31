using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Tekkyu : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TekkyuDestroy"))
        {
            Destroy(gameObject);
        }
    }

}
