using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player")) return;

        List<Transform> interestNodes = new List<Transform>(GetComponentsInChildren<Transform>());
        other.GetComponentInParent<BasicEnemy_GroupController>().EnteredRoom(interestNodes);
    }
}
