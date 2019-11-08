using UnityEngine;
using System.Collections;

public class MasterSpawner : MonoBehaviour {

    enum SpawnableObjects
    {
        STAR,
        HEART,
        SKULL,
        BOMB,
        HOURGLASS
    };

    public GameObject star;
    public GameObject heart;
    public GameObject skull;
    public GameObject bomb;
    public GameObject hourglass;

    private static MasterSpawner instance;

    public GameObject spawnObject(string objectName, Vector3 position, Quaternion rotation)
    {
        GameObject spawnObject;
        try
        {
            SpawnableObjects spawning = (SpawnableObjects)System.Enum.Parse(typeof(SpawnableObjects), objectName, true);

            switch (spawning)
            {
                case SpawnableObjects.STAR:
                    {
                        spawnObject = Instantiate(star, position, rotation) as GameObject;
                        break;
                    }
                case SpawnableObjects.HEART:
                    {
                        spawnObject = Instantiate(heart, position, rotation) as GameObject;
                        break;
                    }
                case SpawnableObjects.SKULL:
                    {
                        spawnObject = Instantiate(skull, position, rotation) as GameObject;
                        break;
                    }
                case SpawnableObjects.BOMB:
                    {
                        spawnObject = Instantiate(bomb, position, rotation) as GameObject;
                        break;
                    }
                case SpawnableObjects.HOURGLASS:
                    {
                        spawnObject = Instantiate(hourglass, position, rotation) as GameObject;
                        break;
                    }
                default:
                    {
                        spawnObject = Instantiate(star, position, rotation) as GameObject;
                        break;
                    }
            }
        }
        catch (System.ArgumentException)
        {
            spawnObject = Instantiate(star, position, rotation) as GameObject;
        }
        
        return spawnObject;
    }

}
