using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpeedItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerControl player= other.GetComponent<PlayerControl>();
        if (player != null)
        {
            //player.ApplyBuffSpeed();
            Destroy(gameObject);
        }
    }
}
