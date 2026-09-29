using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BeanManager : MonoBehaviour
{
    //game logic
    [Header("Settings")]
    [SerializeField] private BeanSegmentScript segmentPrefab;
    [SerializeField] private Transform spawnStart;
    [SerializeField] private float segmentHeight = 2f;
    [SerializeField] private int initialSegment = 8;

    private readonly List<BeanSegmentScript> activeSegments = new List<BeanSegmentScript> ();
    private int segmentCounter = 0;
    private int levelTarget = 100;
    private bool Spawning = true;

    //UI
    public Text scoreText;
    public int playerScore;
    public Text highScoreText;
    public int highScore;
    public GameObject gameOverScreen;
    public GameObject newRecord;
    public int goldenBeans;
    public Text goldenBeansText;
    public GameObject winScreen;
    public GameObject pauseScreen;
    public GameObject controlsScreen;

    void Start()
    {
        GenerateInitialStalk();
        highScore = PlayerPrefs.GetInt("Highscore", 0);
        goldenBeans = PlayerPrefs.GetInt("Golden Beans", 0);
        levelTarget = 10 + (goldenBeans * 25);
        UpdateHighScore();
        UpdateBeanScore();
    }

    private void GenerateInitialStalk()
    {

        if (spawnStart == null)
        {
            return;
        }

        if (segmentPrefab == null)
        {
            return;
        }


        for (int i = 0; i < initialSegment; i++)
        {
            // Vector3 spawnPosition = spawnStart.position + Vector3.up * (i +  (segmentHeight+i));
            Vector3 spawnPosition = spawnStart.position + Vector3.up * (i * segmentHeight);
            BeanSegmentScript newSegment = Instantiate(segmentPrefab,spawnPosition, Quaternion.identity, transform);
            if(i < 2)
            {
                newSegment.Setup(BranchSide.None);
            }
            else
            {
                newSegment.Setup(RandomSide());
            }
            activeSegments.Add(newSegment);
            segmentCounter++;
        }
    }

    private BranchSide RandomSide()
    {
        float rnd = Random.value;
        if (rnd < 0.4f) return BranchSide.None;
        if (rnd < 0.7f) return BranchSide.Left;
        return BranchSide.Right;
    }

    public void Chop()
    {
        if (activeSegments.Count == 0) return;
        //chop down one segment
        BeanSegmentScript bottom = activeSegments[0];
        activeSegments.RemoveAt(0);
        Destroy(bottom.gameObject);

        //drop sequence of segments down
        for(int i = 0; i < activeSegments.Count; i++)
        {
            activeSegments[i].transform.position -= Vector3.up * segmentHeight;
        }

        //spawn new segment on top of sequence
        if (Spawning)
        {
            Vector3 topPosition = spawnStart.position + Vector3.up * (activeSegments.Count * segmentHeight);
            BeanSegmentScript topSegment = Instantiate(segmentPrefab, topPosition, Quaternion.identity, transform);
            if (segmentCounter < levelTarget)
            {
                topSegment.Setup(RandomSide());
                segmentCounter++;
            }
            else if (segmentCounter >= levelTarget)
            {
                topSegment.Setup(BranchSide.Bean);
                Spawning = false;
            }
            activeSegments.Add(topSegment);
        }
        playerScore++;
        scoreText.text = playerScore.ToString();
    }

    public bool CanChop(bool isPlayerLeft)
    {
        if (activeSegments.Count == 0) return true;

        BranchSide bottomSide = activeSegments[0].CurrentSide;

        if (isPlayerLeft && bottomSide == BranchSide.Left) return false;
        if (!isPlayerLeft && bottomSide == BranchSide.Right) return false;

        return true;
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
        if(playerScore > highScore)
        {
            highScore = playerScore;
            PlayerPrefs.SetInt("Highscore", highScore);
            PlayerPrefs.Save();
            newRecord.SetActive(true);
            UpdateHighScore();
        }
    }

    public void UpdateHighScore()
    {
        highScoreText.text = highScore.ToString();
    }

    public void UpdateBeanScore()
    {
        goldenBeansText.text = goldenBeans.ToString();
    }

    public void restartGame()
    {
        SceneManager.LoadScene("GameScene");
        Debug.Log("restart scene");
    }

    public void closeGame()
    {
        Application.Quit();
        Debug.Log("closed game");
    }

    public void controls()
    {
        controlsScreen.SetActive(true);
    }
    public void closeControls()
    {
        controlsScreen.SetActive(false);
    }
    public BeanSegmentScript getBottomSegment()
    {
        if(activeSegments.Count == 0) return null;
        return activeSegments[0];
    }

    [ContextMenu("Resetuj Rekord")]
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        highScore = 0;
        UpdateHighScore();
        Debug.Log("Rekord zresetowany!");
    }
    [ContextMenu("Resetuj fasolki")]
    public void ResetBeanScore()
    {
        PlayerPrefs.DeleteKey("Golden Beans");
        goldenBeans = 0;
        UpdateBeanScore();
        Debug.Log("Rekord zresetowany!");
    }
}
