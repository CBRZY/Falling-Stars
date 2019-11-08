using UnityEngine;
using System.Collections;

public class SpawnerController : MonoBehaviour {
    public GameObject msObject;
    private MasterSpawner masterSpawner;

    public float minDegrees;
    public float maxDegrees;

    void Start () {
        msObject = GameObject.Find("SpawnMaster");
        masterSpawner = msObject.GetComponent<MasterSpawner>();
    }

    public void spawnObject(string spawnObject, bool shootAtAngle, float moveSpeed)
    {
        float angle = Random.Range(minDegrees, maxDegrees);
        GameObject spawnedObject;
        spawnedObject = masterSpawner.spawnObject(spawnObject, transform.position, transform.rotation);
        spawnedObject.SendMessage("init", shootAtAngle ? angle : 270f);
        if (moveSpeed > 0)
        {
            spawnedObject.SendMessage("changeSpeed", moveSpeed);
        }
    }
}
