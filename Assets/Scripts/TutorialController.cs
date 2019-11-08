using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialController : MonoBehaviour {
    public AudioSource audioButtonClick;

    public int state;

    public GameObject star;
    public GameObject buttonLeft;
    public GameObject buttonRight;
    public GameObject[] starTuts;
    public GameObject[] levelTuts;
    public GameObject[] destroyTuts;
    public GameObject[] livesTuts;
    public GameObject[] powerUpTuts;
    public GameObject[] randomPowerUpTuts;
    public GameObject[] finalTuts;

    public Text levelText;
    public Text destroyText;
    public Text livesText;

    private float textF = 1f;

    void Start()
    {
        state = 1;

        starTuts = GameObject.FindGameObjectsWithTag("StarTut");
        levelTuts = GameObject.FindGameObjectsWithTag("LevelTut");
        destroyTuts = GameObject.FindGameObjectsWithTag("DestroyTut");
        livesTuts = GameObject.FindGameObjectsWithTag("LivesTut");
        powerUpTuts = GameObject.FindGameObjectsWithTag("PowerUpTut");
        randomPowerUpTuts = GameObject.FindGameObjectsWithTag("RandomPowerUpTut");
        finalTuts = GameObject.FindGameObjectsWithTag("FinalTut");

        updateState();
    }

    void Update()
    {
        spinStar();
        colourText();
    }

    public void nextState()
    {
        state++;
        updateState();
    }

    public void previousState()
    {
        state--;
        updateState();
    }

    private void updateState()
    {
        switch (state)
        {
            case 1:
                buttonLeft.gameObject.SetActive(false);
                buttonRight.gameObject.SetActive(true);
                starTutsSetActive(true);
                levelTutsSetActive(false);
                destroyTutsSetActive(false);
                livesTutsSetActive(false);
                powerUpTutsSetActive(false);
                randomPowerUpTutsSetActive(false);
                finalTutsSetActive(false);
                break;
            case 2:
                buttonLeft.gameObject.SetActive(true);
                buttonRight.gameObject.SetActive(true);
                starTutsSetActive(false);
                levelTutsSetActive(true);
                destroyTutsSetActive(false);
                livesTutsSetActive(false);
                powerUpTutsSetActive(false);
                randomPowerUpTutsSetActive(false);
                finalTutsSetActive(false);
                break;
            case 3:
                buttonLeft.gameObject.SetActive(true);
                buttonRight.gameObject.SetActive(true);
                starTutsSetActive(false);
                levelTutsSetActive(false);
                destroyTutsSetActive(true);
                livesTutsSetActive(false);
                powerUpTutsSetActive(false);
                randomPowerUpTutsSetActive(false);
                finalTutsSetActive(false);
                break;
            case 4:
                buttonLeft.gameObject.SetActive(true);
                buttonRight.gameObject.SetActive(true);
                starTutsSetActive(false);
                levelTutsSetActive(false);
                destroyTutsSetActive(false);
                livesTutsSetActive(true);
                powerUpTutsSetActive(false);
                randomPowerUpTutsSetActive(false);
                finalTutsSetActive(false);
                break;
            case 5:
                buttonLeft.gameObject.SetActive(true);
                buttonRight.gameObject.SetActive(true);
                starTutsSetActive(false);
                levelTutsSetActive(false);
                destroyTutsSetActive(false);
                livesTutsSetActive(false);
                powerUpTutsSetActive(true);
                randomPowerUpTutsSetActive(false);
                finalTutsSetActive(false);
                break;
            case 6:
                buttonLeft.gameObject.SetActive(true);
                buttonRight.gameObject.SetActive(true);
                starTutsSetActive(false);
                levelTutsSetActive(false);
                destroyTutsSetActive(false);
                livesTutsSetActive(false);
                powerUpTutsSetActive(false);
                randomPowerUpTutsSetActive(true);
                finalTutsSetActive(false);
                break;
            case 7:
                buttonLeft.gameObject.SetActive(true);
                buttonRight.gameObject.SetActive(false);
                starTutsSetActive(false);
                levelTutsSetActive(false);
                destroyTutsSetActive(false);
                livesTutsSetActive(false);
                powerUpTutsSetActive(false);
                randomPowerUpTutsSetActive(false);
                finalTutsSetActive(true);
                break;
        }
    }

    private void starTutsSetActive(bool value)
    {
        for (int i = 0; i < starTuts.Length; i++)
        {
            starTuts[i].SetActive(value);
        }
    }
    private void levelTutsSetActive(bool value)
    {
        for (int i = 0; i < levelTuts.Length; i++)
        {
            levelTuts[i].SetActive(value);
        }
    }
    private void destroyTutsSetActive(bool value)
    {
        for (int i = 0; i < destroyTuts.Length; i++)
        {
            destroyTuts[i].SetActive(value);
        }
    }
    private void livesTutsSetActive(bool value)
    {
        for (int i = 0; i < livesTuts.Length; i++)
        {
            livesTuts[i].SetActive(value);
        }
    }
    private void powerUpTutsSetActive(bool value)
    {
        for (int i = 0; i < powerUpTuts.Length; i++)
        {
            powerUpTuts[i].SetActive(value);
        }
    }
    private void randomPowerUpTutsSetActive(bool value)
    {
        for (int i = 0; i < randomPowerUpTuts.Length; i++)
        {
            randomPowerUpTuts[i].SetActive(value);
        }
    }
    private void finalTutsSetActive(bool value)
    {
        for (int i = 0; i < finalTuts.Length; i++)
        {
            finalTuts[i].SetActive(value);
        }
    }

    private void spinStar()
    {
        star.transform.Rotate((Vector3.forward * 1), 45 * 1 * Time.deltaTime);
    }

    private void colourText()
    {
        StartCoroutine(changeTextColour());
    }

    public void mainMenu()
    {
        StartCoroutine(finishAudio("Menu"));
    }

    IEnumerator changeTextColour()
    {
        if (textF > 0f)
        {
            for (; textF > 0; textF -= 0.1000f)
            {
                Color mainColour = new Color(1, textF, textF, 1);
                levelText.color = mainColour;
                destroyText.color = mainColour;
                livesText.color = mainColour;
                yield return new WaitForSeconds(0.1f);
            }
        }else if (textF < 1f)
        {
            for (; textF < 1; textF += 0.1000f)
            {
                Color mainColour = new Color(1, textF, textF, 1);
                levelText.color = mainColour;
                destroyText.color = mainColour;
                livesText.color = mainColour;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
    }

    IEnumerator finishAudio(string level)
    {
        audioButtonClick.Play();
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(level);
    }
}
