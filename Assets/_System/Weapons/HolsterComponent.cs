using System;
using UnityEngine;

public class HolsterComponent : MonoBehaviour
{
    public GameObject currentWeapon;

    public void PickUpWeapon()
    {
        //SOIT COLLISION, SOIT TOUCHE ETC
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            if (currentWeapon != null)
                return;
            currentWeapon = other.gameObject;
        }
    }
    
    //Faire fonction pour jeter l'arme
}
