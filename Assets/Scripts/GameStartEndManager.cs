using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStartEndManager : MonoBehaviour {

    private Galaxy galaxy;
    private Animator anim;

    public Text textStartWords;
    public Text textFinalScore;
    public Image gameOver;

    void Start () {
        galaxy = Galaxy.Instance;
        anim = GetComponent<Animator>();
        anim.SetTrigger("GameStartTrigger");
    }
	
	void Update () {
        anim.ResetTrigger("GameOverTrigger");
        if (galaxy.getLives() <= 0)
        {
            anim.SetTrigger("GameOverTrigger");
        }
    }

}
