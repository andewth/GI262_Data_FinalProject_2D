using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepController : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            Ninja ninja = collision.GetComponent<Ninja>();
            KeepUp keepUp = GetComponent<KeepUp>();

            if (ninja != null && keepUp != null)
            {
                keepUp.ApplyKeepUp(ninja);
                Destroy(gameObject);
            }
        }
    }
}
