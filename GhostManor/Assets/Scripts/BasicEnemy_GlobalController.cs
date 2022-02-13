
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy_GlobalController : MonoBehaviour
{
    // NOTE: GetComponent<ThirdPersonPlayer_Controls>().collider = this.GetComponent<CapsuleCollider>();

    public GameObject basicEnemyPrefab;
    public GameObject basicEnemy_GroupControllerPrefab;

    private GameObject[] spawnLocations;

    private void Awake()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("EnemySpawn");
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnGroup(23, false, 1);
        SpawnGroup(2, false, 0);
    }

    // EnemyAmount: Amount of enemies spawning
    // doRandomSpawn: Pick random spot to spawn on
    // SpawnLocationNumber: If doRandomSpawn is false, choose a location number to spawn at
    void SpawnGroup(int EnemyAmount, bool doRandomSpawn, int spawnLocationIndex)
    {
        if (doRandomSpawn)
            spawnLocationIndex = UnityEngine.Random.Range(0, spawnLocations.Length);
        if (spawnLocationIndex >= spawnLocations.Length)
            spawnLocationIndex = spawnLocations.Length;

        GameObject selectedSpawnObject = spawnLocations[spawnLocationIndex];
        Transform selectedSpawnTransform = selectedSpawnObject.GetComponent<Transform>();

        GameObject groupController = Instantiate(basicEnemy_GroupControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Transform groupControllerTransform = groupController.GetComponent<Transform>();
        for (int i = 0; i < EnemyAmount; i++)
        {
            GameObject InstantiatedGameObject = Instantiate(basicEnemyPrefab, selectedSpawnTransform.position, Quaternion.identity);
            InstantiatedGameObject.transform.SetParent(groupControllerTransform);
        }
        groupController.GetComponent<BasicEnemy_GroupController>().PopulateGroupArray();
    }
}