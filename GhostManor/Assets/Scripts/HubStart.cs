using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubStart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //The SceneManager loads your new Scene as a single Scene (not overlapping). This is Single mode.
        SceneManager.LoadScene("Intro", LoadSceneMode.Single);
    }
}
