using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy_GroupController : MonoBehaviour
{
    // NOTE: GetComponent<ThirdPersonPlayer_Controls>().collider = this.GetComponent<CapsuleCollider>();

    public GameObject basicEnemyPrefab;

    private GameObject[] spawnLocations;

    private void Awake()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("EnemySpawn");
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnGroup(3, false, 1);
    }

    // Update is called once per frame
    void Update()
    {

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

        for (int i = 0; i < EnemyAmount; i++)
        {
            Instantiate(basicEnemyPrefab, selectedSpawnTransform.position, Quaternion.identity);
        }
    }
}
