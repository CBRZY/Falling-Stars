using UnityEngine;
using System.Collections;

public class PowerUpController : MonoBehaviour {
    private Galaxy galaxy;

    public GameObject mcObject;
    private MainController mainController;

    private float powerUpMoveSpeed;

    private Vector3 directionVector;

    //Initiate object with angle to move in degrees
    //Degrees are converted to radians
    //Radians are used to create a new Vector3 with the direction of the original angle in degrees
    void init(float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        directionVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * 2.5f;
    }
    void changeSpeed(float moveSpeed)
    {
        powerUpMoveSpeed = moveSpeed;
    }

    void Start () {
        powerUpMoveSpeed = 1.5f;
    }

	void Update () {
        moveDown();
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DeathLine"))
        {
            Destroy(this.gameObject);
        }
    }

    private void moveDown()
    {
        transform.Translate((directionVector) * powerUpMoveSpeed * Time.deltaTime, Space.World);
    }
}
