using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private RectTransform healthUI;

    private float maxHealth = 100f;
    public float currentHealth;

    [SerializeField] private float decayMultiplier = 0.8f;
    private ThirdPersonPlayer_Controls playerScript;
    private int visionCount = 0;

    [SerializeField] private float killScore = 10f;

    [SerializeField] private GameObject capturedPlayer;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = gameObject.GetComponent<ThirdPersonPlayer_Controls>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        visionCount = playerScript.VisionCount;
        healthUI.localScale = new Vector3(currentHealth / 100, 1, 1);

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (capturedPlayer.activeInHierarchy)
        {
            //currentHealth -= (Time.deltaTime * decayMultiplier);
        }
        else
        {
            currentHealth -= (Time.deltaTime * decayMultiplier) + (visionCount * 0.1f);
        }
    }

    public void gotKill()
    {
        currentHealth += killScore;
    }
}
