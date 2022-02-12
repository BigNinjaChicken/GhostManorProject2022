
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
    private GameObject[] interestNodes;
    public GameObject basicEnemyPrefab;

    public GameObject selectedRoom;
    public bool foundRoom;
    private GameObject[] spawnLocations;

    private void Awake()
    {
        roomNodes = GameObject.FindGameObjectsWithTag("RoomNode");
        unsearchedRooms = new List<GameObject>(roomNodes);
        interestNodes = GameObject.FindGameObjectsWithTag("InterestNode");
        spawnLocations = GameObject.FindGameObjectsWithTag("EnemySpawn");
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
            foreach (BasicEnemy_Controller enemy in BasicEnemy)
            {
                NavMeshAgent nav = enemy.GetComponent<NavMeshAgent>();
                nav.SetDestination(selectedRoom.transform.position);
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
        float minDist = float.MaxValue;

        int rand = UnityEngine.Random.Range(0, BasicEnemy.Count);
        foreach (GameObject room in unsearchedRooms)
        {
            float closestRoom = Vector3.Distance(room.transform.position, BasicEnemy[rand].transform.position);
            if (closestRoom < minDist)
            {
                minDist = closestRoom;
                selectedRoom = room;
            }
        }
        foundRoom = true;
    }

    bool GetFoundRoom()
    {
        return foundRoom;
    }

    void AreaOfInterest()
    {
        
    }
}