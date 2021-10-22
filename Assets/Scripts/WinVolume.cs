using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinVolume : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Detect object to determine if player
        PlayerShip playerShip = other.gameObject.GetComponent<PlayerShip>();
        if(playerShip != null)
        {
            playerShip.Win();
        }
    }
}
