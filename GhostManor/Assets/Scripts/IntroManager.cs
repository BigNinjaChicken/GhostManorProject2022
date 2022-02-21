using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;
using System;

public class IntroManager : MonoBehaviour
{
    // Reference to the gerated input Action C# script
    private GhostInputActions ghostInputActions;
    private bool allowPlayerInput = false;

    [SerializeField] private CanvasGroup uponTimeText;
    [SerializeField] private CanvasGroup uponTimeBackground;
    private bool doFadeUponTime = false;
    private bool doFadeUponTimeBackground = false;

    [SerializeField] private AudioSource thunderAudio;
    [SerializeField] private AudioSource backgroundAudio;
    [SerializeField] private AudioSource HelicopterAudio;
    [SerializeField] private AudioSource KnockAudio;

    [SerializeField] private GameObject chat;
    [SerializeField] private List<CanvasGroup> chatList;
    private int chatListIndex = 0;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Input action asset is not static or gloabal
        // So we must make a new instance of the input action asset
        ghostInputActions = new GhostInputActions();
    }

    private void OnEnable()
    {
        ghostInputActions.Player.Fire.performed += DoFire;
        ghostInputActions.Player.Fire.Enable();
    }

    // Reason for OnDisable(): events wont get called and
    // thus throw errors if the object is disabled
    private void OnDisable()
    {
        ghostInputActions.Player.Fire.Disable();
    }

    private void DoFire(InputAction.CallbackContext obj)
    {
        if (allowPlayerInput)
        {
            allowPlayerInput = false;
            nextChat();
        }
    }

    void nextChat()
    {
        if (chatListIndex == chatList.Count) 
        {
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        } 
        else
        {
            if (chatListIndex == 2)
            {
                StartCoroutine(Helicopter());
            } 
            else if (chatListIndex == 11)
            {
                StartCoroutine(KnockDoor());
            }
            else
            {
                chatList[chatListIndex - 1].alpha = 0;
                chatList[chatListIndex].alpha = 1;
                chatListIndex++;

                allowPlayerInput = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        chatList = new List<CanvasGroup>(chat.GetComponentsInChildren<CanvasGroup>());

        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(UponATime());
    }

    // Update is called once per frame
    void Update()
    {
        if (doFadeUponTime)
        {
            uponTimeText.alpha -= Time.deltaTime;
        }

        if (doFadeUponTimeBackground)
        {
            uponTimeBackground.alpha -= Time.deltaTime;
        }
    }

    IEnumerator UponATime()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2.5f);

        doFadeUponTime = true;
        thunderAudio.Play(0);

        yield return new WaitForSeconds(1);
        doFadeUponTimeBackground = true;

        yield return new WaitForSeconds(1.0f);
        backgroundAudio.Play(0);

        yield return new WaitForSeconds(3);
        chatList[chatListIndex].alpha = 1;
        chatListIndex++;
        allowPlayerInput = true;
    }

    IEnumerator Helicopter()
    {
        chatList[chatListIndex - 1].alpha = 0;

        yield return new WaitForSeconds(1);
        HelicopterAudio.Play(0);

        yield return new WaitForSeconds(2f);
        chatList[chatListIndex].alpha = 1;
        chatListIndex++;

        allowPlayerInput = true;
    }

    IEnumerator KnockDoor()
    {
        chatList[chatListIndex - 1].alpha = 0;

        yield return new WaitForSeconds(1);
        KnockAudio.Play(0);

        yield return new WaitForSeconds(2f);
        chatList[chatListIndex].alpha = 1;
        chatListIndex++;

        allowPlayerInput = true;
    }
}
