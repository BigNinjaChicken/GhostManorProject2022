using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUp : MonoBehaviour
{
    [SerializeField] private float height = 0;
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, height + Mathf.Sin(Time.time) * 0.35f, transform.position.z);
    }
}
