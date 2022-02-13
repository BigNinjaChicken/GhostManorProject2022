
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy_GroupController : MonoBehaviour
{
    public List<BasicEnemy_Controller> BasicEnemy;
    // NOTE: GetComponent<ThirdPersonPlayer_Controls>().collider = this.GetComponent<CapsuleCollider>();

    private GameObject[] roomNodes;
    public List<GameObject> unsearchedRooms;
    private List<Transform> interestNodes;
    public GameObject basicEnemyPrefab;

    public GameObject selectedRoom;
    public bool foundRoom;
    private GameObject[] spawnLocations;
    private bool inRoom;
    private bool goingToInterest;
    private bool searching;

    private void Awake()
    {
        roomNodes = GameObject.FindGameObjectsWithTag("RoomNode");
        unsearchedRooms = new List<GameObject>(roomNodes);
        spawnLocations = GameObject.FindGameObjectsWithTag("EnemySpawn");

        foundRoom = false;
        inRoom = false;
        goingToInterest = false;
        searching = false;
    }

    public void PopulateGroupArray()
    {
        BasicEnemy = new List<BasicEnemy_Controller>(GetComponentsInChildren<BasicEnemy_Controller>());
    }

    // Update is called once per frame
    void Update()
    {
        if (GetFoundRoom())
        {
            // the inRoom var gets changed in EnteredRoom
            if (GetInRoom())
            {
                GoToInterest();
            } 
            else
            {
                GoToRoom();
            }
        }
        else
        {
            FindRoom();
        }
    }

    // Picks a random enemy in the group to find the closest room
    // then sets "findRoom" to false once selectedRoom
    void FindRoom()
    {
        if (!searching)
        {
            searching = true;

            float minDist = float.MaxValue;

            int rand = UnityEngine.Random.Range(0, BasicEnemy.Count);
            for (int i = 0; i < unsearchedRooms.Count; i++)
            {
                float closestRoom = Vector3.Distance(unsearchedRooms[i].transform.position, BasicEnemy[rand].transform.position);
                if (closestRoom < minDist)
                {
                    minDist = closestRoom;
                    selectedRoom = unsearchedRooms[i];
                }
            }

            unsearchedRooms.Remove(selectedRoom);
            if (unsearchedRooms.Count.Equals(0))
            {
                unsearchedRooms = new List<GameObject>(roomNodes);
                unsearchedRooms.Remove(selectedRoom);
            }

            foundRoom = true;
        }
    }

    bool GetFoundRoom()
    {
        return foundRoom;
    }

    void GoToRoom()
    {
        foreach (BasicEnemy_Controller enemy in BasicEnemy)
        {
            NavMeshAgent nav = enemy.GetComponent<NavMeshAgent>();
            nav.SetDestination(selectedRoom.transform.position);
        }
    }

    public void EnteredRoom(List<Transform> interestNodesTemp)
    {
        if (!inRoom)
        {
            inRoom = true;
            interestNodes = interestNodesTemp;
        }
    }

    void GoToInterest()
    {
        if (!goingToInterest)
        {
            goingToInterest = true;
            foreach (BasicEnemy_Controller enemy in BasicEnemy)
            {
                int randNode;
                randNode = UnityEngine.Random.Range(1, interestNodes.Count);

                NavMeshAgent nav = enemy.GetComponent<NavMeshAgent>();
                nav.SetDestination(interestNodes[randNode].position);
            }

            int waitTime = UnityEngine.Random.Range(4,7);
            Invoke("WaitedAtInterest", waitTime);
        }
    }

    void WaitedAtInterest()
    {
        foundRoom = false;
        inRoom = false;
        goingToInterest = false;
        searching = false;
    }

    bool GetInRoom()
    {
        return inRoom;
    }
}