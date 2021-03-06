using UnityEngine;
using System;
using System.Collections.Generic;
public class ProbabilitySpawner : MonoBehaviour
{
    [SerializeField] private List<ProbabilitySpawn> probabilitySpawns;
    public List<ProbabilitySpawn> ProbabilitySpawns { get => probabilitySpawns; }
    [SerializeField] private List<SpawnedObject> spawnedObjects;
    public List<SpawnedObject> SpawnedObjects {get => spawnedObjects; }
    

    void Awake()
    {
        spawnedObjects = new List<SpawnedObject>();
    }

    void Start()
    {
        // Spawn All
        Spawn(probabilitySpawns);
    }
    void Update()
    {
        
    }

    public void Spawn(ProbabilitySpawn spawn)
    {
        if (spawn == null) return;
        List<Transform> positions = new List<Transform> (spawn.positions);
        byte maxSpawns = (byte)positions.Count;
        byte spawns = (byte)RandomGenerator.NewRandom((byte)spawn.minQuantity, spawn.maxQuantity);
        //Debug.Log("spawns for " + spawn.gameObject + ": " + spawns);
        //if (spawns > positions.Count) continue;

        for (int i = 0; i < spawns; i++)
        {
            if (i > maxSpawns)
            {
                break;
            }
            Transform spawnPos = RandomGenerator.RandomElement<Transform>(positions);
            //spawn.SpawnedPos = spawnPos;

            //if (RandomXRotation || RandomYRotation)
            //{
                /*Quaternion rotation = new Quaternion
                (
                    spawn.randomXRotation? UnityEngine.Random.Range(0, 1) : spawn.gameObject.transform.rotation.x,
                    spawn.randomYRotation? UnityEngine.Random.Range(0, 1) : spawn.gameObject.transform.rotation.y, 
                    0, 0
                );*/
            //}

            try
            {
                GameObject instantiated = Instantiate(spawn.gameObject, spawnPos.position, spawn.gameObject.transform.rotation);
                
                spawnedObjects.Add(new SpawnedObject(instantiated, spawnPos.position, spawn));

                positions.Remove(spawnPos);
                 
            }
            catch (System.Exception ex)
            {
                
                Debug.Log(ex.GetType() + " ---- spawning " + spawn.gameObject + " --- Probably null transform(s) in position list.");
            }


        }
    }
    public void Spawn(List<ProbabilitySpawn> probSpawns)
    {
        foreach (var spawn in probSpawns)
        {
            if (RandomGenerator.MatchProbability(spawn.probability))
            {
                Spawn(spawn);
            }
        }
    }


    public ProbabilitySpawn GetProbabilitySpawnFrom(GameObject gameObject)
    {
        var spawn = probabilitySpawns.Find(p => p.gameObject == gameObject);
        return spawn;
    }

    public ProbabilitySpawn GetProbabilitySpawnFrom(Enemy enemy)
    {
        var enemies = GetEnemiesPs();
        foreach (var e in enemies)
        {
            if (e.gameObject.TryGetComponent<Enemy>(out var enmy))
            {
                if (enmy.enemyName == enemy.enemyName)
                {
                    return e;
                }
            }
        }
        return null;
    }

    public List<ProbabilitySpawn> GetEnemiesPs()
    {
        var enemies = new List<ProbabilitySpawn>();
        var pspwns = new List<ProbabilitySpawn>(probabilitySpawns);
        pspwns.RemoveAll(ps => ps.gameObject == null);
        foreach (var ps in pspwns)
        {
            if (ps.gameObject.TryGetComponent<Enemy>(out var enmy))
            {
                enemies.Add(ps);
            }
        }
        return enemies;
    }

    

}