using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if colliding object is the player ship
        PlayerShip playerShip = other.GetComponent<PlayerShip>();
        if(playerShip != null)
        {
            // Check which power up we obtained and enable it on the player
            // Object names and string parameters hard coded
            if(this.gameObject.name == "ShadePowerup")
            {
                playerShip.EnablePowerup("shade");
            }
            if(this.gameObject.name == "SolPowerup")
            {
                playerShip.EnablePowerup("sol");
            }
            if(this.gameObject.name == "ChargePowerup")
            {
                playerShip.EnablePowerup("charge");
            }
            // Can disable power up after obtained as player handles powerup
            Destroy(this.gameObject);
        }
    }
}
