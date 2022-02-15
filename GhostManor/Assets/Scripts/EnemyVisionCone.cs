using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionCone : MonoBehaviour
{
    [SerializeField]
    private BasicEnemy_Controller controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        { 
            controller.addVisionCount();
        
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            controller.decreaseVisionCount();
        }
    }
}
