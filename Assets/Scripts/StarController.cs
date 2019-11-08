using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {
    private Galaxy galaxy;

    public GameObject mcObject;
    private MainController mainController;

    private int rotateDirection = 0;
    private float level;
    private Vector3 directionVector;

    public GameObject explosionBig;

    public float starMoveSpeed;
    public float starSpinSpeed;


    //Initiate object with angle to move in degrees
    //Degrees are converted to radians
    //Radians are used to create a new Vector3 with the direction of the original angle in degrees
    void init(float degrees)
    {
        
        float radians = degrees * Mathf.Deg2Rad;
        directionVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * 2.5f;

        galaxy = Galaxy.Instance;
        level = galaxy.getLevel();

        mcObject = GameObject.Find("GameMaster");
        mainController = mcObject.GetComponent<MainController>();

        setStarColour();
        rotateDirection = (Random.Range(-10f, 10f) >= 0) ? 1 : -1;

        float speed = Mathf.Pow(1.1f, level) < 4.0f ? Mathf.Pow(1.1f, level) : 4.0f;
        starSpinSpeed = speed;
        starMoveSpeed = speed;
    }

    public void changeSpeed(float moveSpeed)
    {
        starMoveSpeed = moveSpeed;
        starSpinSpeed = moveSpeed;
    }
	
	void Update () {
        spin();
        moveDown();
	}

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathLine") && galaxy.getLives() > 0)
        {
            mainController.galaxyLoseLife();
        } else if (other.CompareTag("Star"))
        {
            mainController.galaxyDestroyStar("Star");

            Vector3 centrePoint = Vector3.Lerp(this.gameObject.transform.position, other.transform.position, 0.5f);
            GameObject explodeObject = Instantiate(explosionBig, centrePoint, other.transform.rotation) as GameObject;
            Destroy(this.gameObject);
            Destroy(explodeObject, 1.0f);
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DeathLine"))
        {
            mainController.galaxyDestroyStar("DeathLine");
            Destroy(this.gameObject);
        }
    }

    private void spin()
    {
        transform.Rotate((Vector3.forward * rotateDirection), 45 * starSpinSpeed * Time.deltaTime);
    }

    private void moveDown()
    {
        transform.Translate((directionVector) * starMoveSpeed * Time.deltaTime, Space.World);
    }

    private void setStarColour()
    {
        Color mainColour = new Color(1, 0, 0, 1);
        int num = 51 * galaxy.getLevel();
        float numValue;
        if (galaxy.getLevel() < 6)
        {
            numValue = num / 255f;
            mainColour.g = numValue;
        }
        else if (galaxy.getLevel() < 11)
        {
            numValue = (num - 255f) / 255f;
            mainColour.g = 1;
            mainColour.b = numValue;
        }else
        {
            mainColour.g = 1;
            mainColour.b = 1;
        }
        GetComponent<Renderer>().material.SetColor("_Color",mainColour);
        mainColour.a = 0.8f;
        GetComponent<Renderer>().material.SetColor("_ReflectColor", mainColour);
    }
}

