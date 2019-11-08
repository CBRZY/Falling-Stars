public class Galaxy
{
    private static Galaxy instance;
    private int lives = 1;
    private int level = 1;
    private int starsDestroyed = 0;

    private Galaxy()
    {
    }

    public static Galaxy Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Galaxy();
            }
            return instance;
        }
    }

    public void loseLife()
    {
        lives--;
    }
    public void gainLife()
    {
        lives++;
    }
    public void starDestroyed()
    {
        starsDestroyed++;
    }

    public void levelUp()
    {
        level++;
    }

    public int getLives()
    {
        return lives;
    }

    public int getLevel()
    {
        return level;
    }

    public int getStarsDestroyed()
    {
        return starsDestroyed;
    }

    public void destroyGalaxy()
    {
        instance = null;
    }

}
