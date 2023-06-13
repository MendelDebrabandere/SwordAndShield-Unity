using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagWin : MonoBehaviour
{
    private HUD hud = null;

    void Start()
    {
        hud = FindObjectOfType<HUD>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            if (hud != null)
            {
                hud.Win();
            }
        }
    }

}
