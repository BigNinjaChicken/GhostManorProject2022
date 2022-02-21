using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomFog : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(-0.46f, 2.7f, 0.34f);
    }
}
