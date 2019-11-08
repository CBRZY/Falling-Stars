using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System;

public class MainController : MonoBehaviour {

    private RuntimePlatform platform = Application.platform;

    private GameObject[] allStars;

    public GameObject powerUpStar;
    private bool randomPowerUp;
    private int askRate;

    private Galaxy galaxy;
    private AlgoRandom algoRandom;

    private int finalScore = -1;
    private bool isConnectedToGoogle;

    private bool fourPUs = false;
    private bool threePUs = false;
    private bool twoPUs = false;

    private bool[] powerUpsActivated = new bool[4];
    private int powerUpsActivatedCount = 0;

    private bool receivedHeart = false;
    private int lowHit = 0;
    private int highHit = 0;
    private int anyHit = 0;

    private bool receivedBomb = false;
    private int totalStars = 0;

    private bool hourglassReady = false;
    private bool slowActive = false;
    private bool receivedHourglass = false;
    private float moveSpeed = 0f;
    private double hourglassProb = 10;
    public Timer hourglassTimer;

    public AudioSource audioDestroyStar;
    public AudioSource audioLevelUp;
    public AudioSource audioDeath;
    public AudioSource audioButtonClick;
    public AudioSource audioLife;
    public AudioSource audioBombExplode;
    public AudioSource audioTimeTicking;

    public Timer levelTimer;
    public Timer spawnTimer;
    public Timer tenSecTimer;
    private float spawnTimerDuration = 3f;
    private float levelTimerDuration = 10f;
    private int tenSecs = 0;

    public SpawnerController[] spawners;
    public GameObject starExplosion;
    public GameObject heartExplosion;
    public GameObject bombExplosion;
    public GameObject skullExplosion;
    public GameObject slowStars;

    public Text textLevel;
    public Text textStarsDestroyed;
    public Text textLives;
    public Text textFinalScore;

    void Awake()
    {
        randomPowerUp = LoadReward();
        askRate = LoadRate();
        deativatePowerUpsActivated();

        isConnectedToGoogle = Convert.ToBoolean(PlayerPrefs.GetInt("isConnectedToGoogle"));
    }
    void Start () {
        galaxy = Galaxy.Instance;
        algoRandom = AlgoRandom.Instance(spawners.Length);

        textLevel.text = galaxy.getLevel().ToString();
        textStarsDestroyed.text = galaxy.getStarsDestroyed().ToString();
        textLives.text = galaxy.getLives().ToString();

        setTimerDurations(galaxy.getLevel());

        levelTimer = Timer.Register(levelTimerDuration, () => galaxyLevelUp(), isLooped: true);
        spawnTimer = Timer.Register(spawnTimerDuration, () => customUpdate(), isLooped: true);
        tenSecTimer = Timer.Register(10f, () => tenSecCheck(), isLooped: true);


    }
    void Update()
    {
        for (int i = 0; i < powerUpsActivated.Length; i++)
        {
            if (powerUpsActivated[i])
            {
                powerUpsActivatedCount++;
            }
        }
        if (powerUpsActivatedCount > 1)
        {
            twoPUs = true;
        }
        if (powerUpsActivatedCount > 2)
        {
            threePUs = true;
        }
        if (powerUpsActivatedCount > 3)
        {
            fourPUs = true;
        }

        totalStars = GameObject.FindGameObjectsWithTag("Star").Length;

        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    checkTouch(Input.GetTouch(0).position);
                }
            }
        }
        else if (platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                checkTouch(Input.mousePosition);
            }
        }
    }

    //Methods for controlling Hits
    private void checkTouch(Vector3 pos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        Collider2D hit = Physics2D.OverlapPoint(touchPos);
        if (hit)
        {
            if (hit.tag.Equals("Star"))
            {
                hitStar(touchPos, hit);
            }

            if (hit.tag.Equals("Heart"))
            {
                hitHeart(hit);
            }

            if (hit.tag.Equals("Skull"))
            {
                hitSkull(hit);
            }

            if (hit.tag.Equals("Bomb"))
            {
                hitBomb(hit);
            }
            if (hit.tag.Equals("Hourglass"))
            {
                hitHourglass();
            }

            if (hit.tag.Equals("PowerUpStar"))
            {
                hitPowerUpStar(hit);
            }

            Destroy(hit.gameObject);
        }
    }
    private void hitStar(Vector2 touchPos, Collider2D hit)
    {
        galaxyDestroyStar("Touch");
        GameObject explodeObject = Instantiate(starExplosion, hit.transform.position, hit.transform.rotation) as GameObject;
        Destroy(explodeObject, 1.0f);
        if (touchPos.y <= -1.5 && galaxy.getLives() < 3 && !receivedHeart)
        {
            lowHit++;
        }
        else
        {
            if (lowHit > 0)
            {
                lowHit--;
            }
        }
        if (touchPos.y > -1.5)
        {
            highHit++;
        }
        anyHit++;
    }
    private void hitHeart(Collider2D hit)
    {
        powerUpsActivated[0] = true;
        audioLife.Play();
        GameObject explodeObject = Instantiate(heartExplosion, hit.transform.position, hit.transform.rotation) as GameObject;
        Destroy(explodeObject, 1.0f);
        galaxyGainLife();
        anyHit++;
    }
    private void hitSkull(Collider2D hit)
    {
        powerUpsActivated[1] = true;
        GameObject explodeObject = Instantiate(skullExplosion, hit.transform.position, hit.transform.rotation) as GameObject;
        Destroy(explodeObject, 1.0f);
        galaxyLoseLife();
    }
    private void hitBomb(Collider2D hit)
    {
        powerUpsActivated[2] = true;
        GameObject explodeObject = Instantiate(bombExplosion, hit.transform.position, hit.transform.rotation) as GameObject;
        Destroy(explodeObject, 1.0f);

        GameObject[] stars = GameObject.FindGameObjectsWithTag("Star");
        GameObject[] hearts = GameObject.FindGameObjectsWithTag("Heart");
        GameObject[] hourglasses = GameObject.FindGameObjectsWithTag("Hourglass");
        foreach (GameObject star in stars)
        {
            audioBombExplode.Play();
            GameObject explosions = Instantiate(starExplosion, star.transform.position, star.transform.rotation) as GameObject;
            Destroy(star);
            galaxyDestroyStar("Bomb");
            Destroy(explosions, 1.0f);
        }
        foreach (GameObject heart in hearts)
        {
            GameObject explosions = Instantiate(heartExplosion, heart.transform.position, heart.transform.rotation) as GameObject;
            Destroy(heart);
            Destroy(explosions, 1.0f);
        }
        foreach (GameObject hourglass in hourglasses)
        {
            Destroy(hourglass);
        }
        anyHit++;
    }
    private void hitHourglass()
    {
        powerUpsActivated[3] = true;
        audioTimeTicking.Play();
        slowActive = true;
        GameObject slowStarField = Instantiate(slowStars, new Vector3(0, 0, 0), new Quaternion(90, 0, 0, 90)) as GameObject;

        allStars = GameObject.FindGameObjectsWithTag("Star");
        for (int i = 0; i < allStars.Length; i++)
        {
            allStars[i].GetComponent<StarController>().changeSpeed(0.75f);
        }
        moveSpeed = 0.75f;

        hourglassTimer = Timer.Register(7.5f, () => endHourglass(slowStarField));
        anyHit++;
    }
    private void hitPowerUpStar(Collider2D hit)
    {
        int chance = UnityEngine.Random.Range(1, 100);

        if (chance > 0 && chance < 26)
        {
            if (galaxy.getLives() > 2)
            {
                hitSkull(hit);
            }
            else
            {
                hitHeart(hit);
            }

        }
        if (chance > 25 && chance < 51)
        {
            hitSkull(hit);
        }
        if (chance > 50 && chance < 76)
        {
            hitBomb(hit);
        }
        if (chance > 75 && chance < 101)
        {
            hitHourglass();
        }
        SaveReward(false);
    }

    //Methods for manipulating Galaxy
    public void destroyGalaxy()
    {
        galaxy.destroyGalaxy();
    }
    public void galaxyDestroyStar(string origin)
    {
        if (!origin.Equals("DeathLine"))
        {
            audioDestroyStar.Play();
        }
        if (origin.Equals("Touch") || origin.Equals("Bomb"))
        {
            if (galaxy.getLives() > 0)
            {
                galaxy.starDestroyed();
                textStarsDestroyed.text = galaxy.getStarsDestroyed().ToString();
            }
        }
    }
    public void galaxyLoseLife()
    {
        if (galaxy.getLives() > 0)
        {
            audioDeath.Play();
            galaxy.loseLife();
        }
        if (galaxy.getLives() == 0)
        {
            audioTimeTicking.Stop();
            Timer.Cancel(levelTimer);
            Timer.Cancel(spawnTimer);
            Timer.Cancel(hourglassTimer);

            finalScore = (finalScore == -1 ? finalScore = galaxy.getLevel() * galaxy.getStarsDestroyed() : finalScore);
            textFinalScore.text = finalScore.ToString();

#if !UNITY_EDITOR
            if (askRate == 0)
            {
                MobileNativeRateUs ratePopUp = new MobileNativeRateUs("Enjoying Falling Stars?", "Please rate to support us.","Rate Now","Much much later","Later");
                ratePopUp.SetAndroidAppUrl("https://play.google.com/store/apps/details?id=com.AwkwardTurtleStudios.FallingStars");
                ratePopUp.Start();
                ratePopUp.OnComplete += OnRatePopUpClose;
            }
            else if (askRate > 0)
            {
                askRate--;
                SaveRate(askRate);
            }
            if (isConnectedToGoogle)
            {
                achievementsAndLeaderBoards();
            }
#endif     
        }
        textLives.text = galaxy.getLives().ToString();
    }
    private void galaxyGainLife()
    {
        if (galaxy.getLives() > 0)
        {
            galaxy.gainLife();
        }
        textLives.text = galaxy.getLives().ToString();
    }
    private void galaxyLevelUp()
    {
        if (galaxy.getLives() > 0)
        {
            galaxy.levelUp();
            deativatePowerUpsActivated();
            audioLevelUp.Play();
            textLevel.text = galaxy.getLevel().ToString();
            setTimerDurations(galaxy.getLevel());
        }
    }

    //Methods for Spawning
    private void customUpdate() {

        if (randomPowerUp)
        {
            randomPowerUp = false;
            Instantiate(powerUpStar);
        }
        spawnStar();

        if (!receivedHeart && lowHit >= 10 && galaxy.getLives() < 3)
        {
            spawnHeart();
        }
        if(!receivedHourglass && galaxy.getLevel() > 4 && hourglassReady && !slowActive)
        {
            spawnHourglass();
        }
        if (anyHit >= 5)
        {
            spawnSkull();
        }
        if (!receivedBomb && totalStars > 6)
        {
            spawnBomb();
        }

    }
    private void spawnStar()
    {
        spawners[algoRandom.nextSpawner()].spawnObject("STAR", galaxy.getLevel() > 2, moveSpeed);
    }
    private void spawnHeart()
    {
        spawners[algoRandom.nextSpawner()].spawnObject("HEART", false, moveSpeed);
        receivedHeart = true;
        lowHit = 0;
    }
    private void spawnSkull()
    {
        spawners[algoRandom.nextSpawner()].spawnObject("SKULL", false, moveSpeed);
        anyHit = 0;
    }
    private void spawnBomb()
    {
        spawners[algoRandom.nextSpawner()].spawnObject("BOMB", false, moveSpeed);
        receivedBomb = true;
    }
    private void spawnHourglass()
    {
        spawners[algoRandom.nextSpawner()].spawnObject("HOURGLASS", false, moveSpeed);
        receivedHourglass = true;
        hourglassProb = 10;
    }

    //Methods for misc Timers
    private void setTimerDurations(int currentLevel)
    {
        levelTimerDuration = (levelTimerDuration < 60f) ? 10f * currentLevel : 60f;
        spawnTimerDuration = (spawnTimerDuration > 0.375f) ? 3f / currentLevel : 0.375f;

        if (levelTimer != null && spawnTimer != null)
        {
            levelTimer.duration = levelTimerDuration;
            spawnTimer.duration = spawnTimerDuration;
        }
    }
    private void tenSecCheck()
    {
        tenSecs += 10;

        if (tenSecs % 30 == 0)
        {
            receivedBomb = false;
            receivedHourglass = false;
        }
        if (tenSecs % 60 == 0)
        {
            receivedHeart = false;
        }

        if (UnityEngine.Random.Range(1, 100) <= hourglassProb)
        {
            hourglassReady = true;
        }
        else
        {
            hourglassReady = false;
            if (hourglassProb < 35)
            {
                hourglassProb += 5;
            }
        }
    }
    private void endHourglass(GameObject slowStarField)
    {
        audioTimeTicking.Stop();
        slowActive = false;
        allStars = GameObject.FindGameObjectsWithTag("Star");
        float speed = Mathf.Pow(1.1f, galaxy.getLevel()) < 4.0f ? Mathf.Pow(1.1f, galaxy.getLevel()) : 4.0f;
        for (int i = 0; i < allStars.Length; i++)
        {
            allStars[i].GetComponent<StarController>().changeSpeed(speed);
        }

        moveSpeed = 0f;
        Destroy(slowStarField);
        Timer.Cancel(hourglassTimer);
    }

    //Methods for Unity Ads
    public void watchAd()
    {
        audioButtonClick.Play();
        if (Advertisement.IsReady("randomPowerUp"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("randomPowerUp", options);
        }
    }
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                if (Debug.isDebugBuild)
                    Debug.Log("The ad was successfully shown.");
                SaveReward(true);
                break;
            case ShowResult.Skipped:
                if (Debug.isDebugBuild)
                    Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                if (Debug.isDebugBuild)
                    Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

    //Methods for Saving and Loading Rewards and Ratings
    private void SaveReward(bool haveReward)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/randomPowerUpInfo.dat");

        bool randomPowerUpObj = haveReward;

        bf.Serialize(file, randomPowerUpObj);
        file.Close();
    }
    private bool LoadReward()
    {
        bool powerUp = false;
        if (File.Exists(Application.persistentDataPath + "/randomPowerUpInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/randomPowerUpInfo.dat", FileMode.Open);
            bool randomPowerUpObj = (bool)bf.Deserialize(file);

            powerUp = randomPowerUpObj;

            file.Close();
        }
        return powerUp;
    }
    private void SaveRate(int askRate)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/ratingInfo.dat");

        int askRateObj = askRate;

        bf.Serialize(file, askRateObj);
        file.Close();
    }
    private int LoadRate()
    {
        int rate = 3;
        if (File.Exists(Application.persistentDataPath + "/ratingInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/ratingInfo.dat", FileMode.Open);
            rate = (int)bf.Deserialize(file);

            file.Close();
        }
        return rate;
    }

    private void OnRatePopUpClose(MNDialogResult result)
    {
        switch (result)
        {
            case MNDialogResult.RATED:
                if (Debug.isDebugBuild)
                    Debug.Log("Rate Option picked");
                SaveRate(-1);
                break;
            case MNDialogResult.REMIND:
                if (Debug.isDebugBuild)
                    Debug.Log("Much much later Option picked");
                SaveRate(6);
                break;
            case MNDialogResult.DECLINED:
                if (Debug.isDebugBuild)
                    Debug.Log("Later Option picked");
                SaveRate(3);
                break;
        }
    }

    private void deativatePowerUpsActivated()
    {
        for (int i = 0; i < powerUpsActivated.Length; i++)
        {
            powerUpsActivated[i] = false;
        }
        powerUpsActivatedCount = 0;
    }

    public void leaderboards()
    {
#if !UNITY_EDITOR
        if (isConnectedToGoogle)
        {
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
    //Methods for Reporting GooglePlay Achievements
    private void achievementsAndLeaderBoards()
    {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    reportLevelAchievements(galaxy.getLevel());
                    reportPowerUpAchievements();

                    reportLeaderboardScore();
                }
                else
                {
                    new MobileNativeMessage("Google Play Games Achievements/Leaderboard", "Please make sure Google Play Games is enabled and you have internet access.");
                }
            
            });
    }

    private void reportLevelAchievements(int levelReached)
    {
        if (levelReached > 5 && !PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_level_5).IsUnlocked)
        {
            Social.ReportProgress(FallingStarsResources.achievement_level_5, 100f, (bool success) => { });
        }
        if (levelReached > 10 && !PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_level_10).IsUnlocked)
        {
            Social.ReportProgress(FallingStarsResources.achievement_level_10, 100f, (bool success) => { });
        }
        if (levelReached > 15 && !PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_level_15).IsUnlocked)
        {
            Social.ReportProgress(FallingStarsResources.achievement_level_15, 100f, (bool success) => { });
        }
        if (levelReached > 20 && !PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_level_20).IsUnlocked)
        {
            Social.ReportProgress(FallingStarsResources.achievement_level_20, 100f, (bool success) => { });
        }
        if (levelReached > 25 && !PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_level_25).IsUnlocked)
        {
            Social.ReportProgress(FallingStarsResources.achievement_level_25, 100f, (bool success) => { });
        }
    }

    //Report Progress on Google Play Games Achievements
    private void reportPowerUpAchievements()
    {
        
        if (fourPUs)
        {
            if (PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_3_power_ups).IsUnlocked)
            {
                if (!PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_4_power_ups).IsUnlocked)
                {
                    Social.ReportProgress(FallingStarsResources.achievement_4_power_ups, 100f, (bool success) => { });
                }
            }
        }
        if (threePUs)
        {
            if (PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_2_power_ups).IsUnlocked)
            {
                if (!PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_3_power_ups).IsUnlocked)
                {
                    Social.ReportProgress(FallingStarsResources.achievement_3_power_ups, 100f, (bool success) => { });
                }
            }
        }
        if (twoPUs)
        {
            if (!PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_2_power_ups).IsUnlocked)
            {
                Social.ReportProgress(FallingStarsResources.achievement_2_power_ups, 100f, (bool success) => { });
            }
        }
    }
    private void reportLeaderboardScore()
    {
        Social.ReportScore(finalScore,FallingStarsResources.leaderboard_the_ultimate_falling_star_destroyer, (bool success) => { });

        //Get the top player from the leaderboard, make sure it is rank 1 and compare ID with local user id. 
        PlayGamesPlatform.Instance.LoadScores(FallingStarsResources.leaderboard_the_ultimate_falling_star_destroyer, LeaderboardStart.TopScores, 1, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, (data) =>
        { 
            IScore[] allScores = data.Scores;

            for (int i = 0; i < allScores.Length; i++)
            {
                if (allScores[i].userID == Social.localUser.id && allScores[i].rank == 1)
                {
                    if (!PlayGamesPlatform.Instance.GetAchievement(FallingStarsResources.achievement_ultimate_falling_star_destroyer).IsUnlocked)
                    {
                        Social.ReportProgress(FallingStarsResources.achievement_ultimate_falling_star_destroyer, 100f, (bool success) => { });
                    }
                }
            }
        });
    }
}


