using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy_Controller : MonoBehaviour
{
    private Transform trans;
    private GameObject player;
    private Transform playerTrans;
    private ThirdPersonPlayer_Controls playerController;
    [SerializeField]
    private LayerMask nodeMask;

    // any number above 0 for inVision means player is in vision cone
    [SerializeField]
    private int VisionCount; // player in vision

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        trans = this.GetComponent<Transform>();
        playerTrans = player.GetComponent<Transform>();
        playerController = player.GetComponent<ThirdPersonPlayer_Controls>();
    }

    private void inVision(int count)
    {
        // if left vision
        if (count < 0)
        {
            // check to see if player was in vision
            // before they left vision
            if (VisionCount > 0)
            {
                VisionCount += count;
                playerController.ChangeVisionCount(count);
            }
        } 
        else
        {
            Vector3 dir = (playerTrans.position - trans.position).normalized;

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(trans.position, dir, out hit, nodeMask))
            {
                VisionCount += count;
                playerController.ChangeVisionCount(count);
            }
        }
    }

    public void RemoveEnemy()
    {
        BasicEnemy_GroupController groupController = gameObject.GetComponentInParent<BasicEnemy_GroupController>();

        groupController.RemoveEnemy(this);
        Destroy(gameObject);
    }

    public void addVisionCount()
    {
        inVision(1);
    }

    public void decreaseVisionCount()
    {
        inVision(-1);
    }
}
