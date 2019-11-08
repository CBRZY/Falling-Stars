using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GooglePlayGames;
using System;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using GooglePlayGames.BasicApi;

public class MenuController : MonoBehaviour {

    public AudioSource audioButtonClick;
    public GameObject audioTheme;

    private GameObject[] themeMusicArr;
    private GameObject themeMusic;

    private string ultimateDestroyer;
    private bool haveLeader = false;
    public Text textLeaderName;
    public Text textLeader;

    private bool isConnectedToGoogle;

    void Awake()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("Development Build");
        }
    }

    void Start () {
        themeMusicArr = GameObject.FindGameObjectsWithTag("ThemeMusic");

        if (themeMusicArr.Length == 0)
        {
            themeMusic = Instantiate(audioTheme, new Vector3(), new Quaternion()) as GameObject;
            DontDestroyOnLoad(themeMusic);
        }

#if !UNITY_EDITOR
            PlayGamesPlatform.Activate();
            if (Debug.isDebugBuild)
            {
                PlayGamesPlatform.DebugLogEnabled = true;
            }
            Social.localUser.Authenticate((bool success) =>
            {
                isConnectedToGoogle = success;
                PlayerPrefs.SetInt("isConnectedToGoogle", Convert.ToInt32(isConnectedToGoogle));
                if (!haveLeader)
                {
                    displayLeader();
                }

            });
#endif

    }

    private void displayLeader()
    {
        PlayGamesPlatform.Instance.LoadScores(FallingStarsResources.leaderboard_the_ultimate_falling_star_destroyer, LeaderboardStart.TopScores, 1, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, (data) =>
        {
            IScore[] allScores = data.Scores;
            string[] userIds = new string[allScores.Length];

            for (int i = 0; i < allScores.Length; i++)
            {
                userIds[i] = allScores[i].userID;
            }
            Social.LoadUsers(userIds, (IUserProfile[] profiles) =>
            {
                ultimateDestroyer = profiles[0].userName;
                haveLeader = true;
                textLeader.text = "Current Ultimate Star Destroyer is";
                textLeaderName.text = ultimateDestroyer;
            });

        });
    }

    public void playGame()
    {
        StartCoroutine(finishAudio("Main"));
    }
    public void tutorial()
    {
        StartCoroutine(finishAudio("Tutorial"));
    }

    public void leaderboard()
    {
#if !UNITY_EDITOR
        if (isConnectedToGoogle)
        {
            if (!haveLeader)
            {
                displayLeader();
            }
            if (Social.localUser.authenticated)
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(FallingStarsResources.leaderboard_the_ultimate_falling_star_destroyer);
            }
            else
            {
                new MobileNativeMessage("Google Play Games Leaderboard", "Could not authenticate Google Play Games account.");
            }
        }
        else
        {
            new MobileNativeMessage("Google Play Games", "Please make sure Google Play Games is enabled and you have internet access.");
        }
#endif
    }
    public void achievements()
    {
#if !UNITY_EDITOR
        if (isConnectedToGoogle)
        {
            if (Social.localUser.authenticated)
            {
                Social.ShowAchievementsUI();
            }
            else
            {
                new MobileNativeMessage("Google Play Games Achievments", "Could not authenticate Google Play Games account.");
            }
        }
        else
        {
            new MobileNativeMessage("Google Play Games", "Please make sure Google Play Games is enabled and you have internet access.");
        }
#endif
    }

    public void exitGame()
    {
        StartCoroutine(finishAudio("Quit"));
    }
    IEnumerator finishAudio(string level)
    {
        audioButtonClick.Play();
        yield return new WaitForSeconds(0.1f);
        if (level.Equals("Quit"))
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(level);
        }
        
    }

}
