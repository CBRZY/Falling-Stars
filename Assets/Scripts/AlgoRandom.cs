using UnityEngine;

public class AlgoRandom{

    private static AlgoRandom instance;

    //Actual amount of spawners. Not index of spawners
    private int numberOfSpawners;

    //Used to ensure that the same spawner doesn't spawn consecutively
    private int currentActiveSpawner;
    private bool readyToSpawn;

    private AlgoRandom(int numberOfSpawners)
    {
        this.numberOfSpawners = numberOfSpawners;
        currentActiveSpawner = -1;
        readyToSpawn = false;
    }

    public static AlgoRandom Instance(int numberofSpawners)
    {
        if (instance == null)
        {
            instance = new AlgoRandom(numberofSpawners);
        }
        return instance;
    }

    public int nextSpawner()
    {
        int nextSpawner;
        do
        {
            nextSpawner = Random.Range(0, numberOfSpawners);
            if (currentActiveSpawner != nextSpawner)
            {
                currentActiveSpawner = nextSpawner;
                readyToSpawn = true;
            }
            else
            {
                readyToSpawn = false;
            }
        } while (!readyToSpawn);
        return currentActiveSpawner;
    }
}
