using UnityEngine;
using System.Collections;

public class AlgoWave {

    private static AlgoWave instance;

    //Actual amount of spawners. Not index of spawners
    private int numberOfSpawners;
    private int nextActiveSpawner;

    private bool increment;

    private AlgoWave(int numberOfSpawners)
    {
        this.numberOfSpawners = numberOfSpawners;

        if (Random.Range(-10f, 10f) >= 0)
        {
            nextActiveSpawner = 0;
            increment = true;
        }
        else
        {
            nextActiveSpawner = numberOfSpawners - 1;
            increment = false;
        }
    }

    public static AlgoWave Instance(int numberofSpawners)
    {
        if (instance == null)
        {
            instance = new AlgoWave(numberofSpawners);
        }
        return instance;
    }

    public int nextSpawner()
    {
        int currentSpawner = nextActiveSpawner;
        if (increment)
        {
            nextActiveSpawner++;
            if (nextActiveSpawner == numberOfSpawners - 1)
            {
                increment = false;
            }
        }
        else
        {
            nextActiveSpawner--;
            if (nextActiveSpawner == 0)
            {
                increment = true;
            }
        }

        return currentSpawner;


    }
}
