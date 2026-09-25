using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private BeanManager beanManager;
    [SerializeField] private Image timerBar;

    [Header("Movement settings")] 
    [SerializeField] private float sideOffset = 1.5f;
    [SerializeField] private float angle = 17f;
    private bool isBroken = false;
    private bool isPaused = false;
    private int score = 0;

    [Header("Timer settings")]
    [SerializeField] private float maxTimer = 5f;
    [SerializeField] private float timeBonus = 0.15f;
    [SerializeField] private float drainSpeed = 0.5f;
    private float currentTimer;

    void Start()
    {
        currentTimer = maxTimer;
    }

    void Update()
    {
        //movement
        if (Input.GetKeyDown(KeyCode.Escape) && !isBroken)
        {
            if (isPaused) unpause();
            else pause();
        }
        if (isBroken || isPaused) return;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) PerformChop(isLeft: true);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) PerformChop(isLeft: false);

        //timer
        currentTimer -= Time.deltaTime * drainSpeed;

        if(timerBar != null) timerBar.fillAmount = currentTimer/maxTimer;
        if(currentTimer <= 0)
        {
            timeOver();
            return;
        }
            
    }

    private void PerformChop(bool isLeft)
    {
        float target = isLeft ? -sideOffset : sideOffset;
        transform.position = new Vector3(target, transform.position.y, transform.position.z);

        float targetRotation = isLeft ? -angle : angle;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);

        if (!beanManager.CanChop(isLeft))
        {
            //Debug.Log("Game over,");
            isBroken = true;
            beanManager.gameOver();
            return;
        }

        //checking if next segment is bean
        BeanSegmentScript bottomSegment = beanManager.getBottomSegment();
        bool winningChop = (bottomSegment != null && bottomSegment.CurrentSide == BranchSide.Bean);

        beanManager.Chop();
        score++;

        if (winningChop)
        {
            isBroken = true;

            int goldenBeans = PlayerPrefs.GetInt("Golden Beans", 0);
            goldenBeans++;
            PlayerPrefs.SetInt("Golden Beans", goldenBeans);
            PlayerPrefs.Save();
            beanManager.winScreen.SetActive(true);
        }

        //bonus points to timer after chop
        currentTimer += timeBonus;
        currentTimer = Mathf.Clamp(currentTimer, 0f, maxTimer);
    }

    private void timeOver()
    {
        isBroken = true;
        beanManager.gameOver();
    }
    private void pause()
    {
        isPaused = true;
        beanManager.pauseScreen.SetActive(true);
    }

    public void unpause()
    {
        isPaused = false;
        beanManager.pauseScreen.SetActive(false);
    }
}
