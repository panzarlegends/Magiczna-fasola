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
    private Coroutine swingCoroutine;

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

        //float targetRotation = isLeft ? -angle : angle;
        //transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
        if (swingCoroutine != null) StopCoroutine(swingCoroutine);
        swingCoroutine = StartCoroutine(SwingSword(isLeft));

        if (!beanManager.CanChop(isLeft))
        {
            //Debug.Log("Game over,");
            isBroken = true;
            beanManager.gameOver();
            return;
        }

        //checking if next segment is winning bean
        BeanSegmentScript bottomSegment = beanManager.getBottomSegment();
        bool winningChop = (bottomSegment != null && bottomSegment.CurrentSide == BranchSide.Bean);

        beanManager.Chop(isLeft);
        score++;

        if (winningChop)
        {
            isBroken = true;

            int goldenBeans = PlayerPrefs.GetInt("Golden Beans", 0);
            goldenBeans++;
            PlayerPrefs.SetInt("Golden Beans", goldenBeans);
            PlayerPrefs.Save();
            //beanManager.winScreen.SetActive(true);
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

    //swing animation function
    private System.Collections.IEnumerator SwingSword(bool isLeft)
    {
        float restAngle = isLeft ? -angle : angle;
        float hitAngle = isLeft ? -75f : 75f;

        // rest position and hit position
        float restX = isLeft ? -sideOffset : sideOffset;
        float hitX = isLeft ? -sideOffset * 0.55f : sideOffset * 0.55f;

        // animation parameters
        transform.rotation = Quaternion.Euler(0f, 0f, hitAngle);
        transform.position = new Vector3(hitX, transform.position.y, transform.position.z);

        float duration = 0.08f;
        float elapsed = 0f;

        //animation
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float currentAngle = Mathf.Lerp(hitAngle, restAngle, t);
            float currentX = Mathf.Lerp(hitX, restX, t);

            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            transform.position = new Vector3(currentX, transform.position.y, transform.position.z);

            yield return null;
        }

        // back to rest position
        transform.rotation = Quaternion.Euler(0f, 0f, restAngle);
        transform.position = new Vector3(restX, transform.position.y, transform.position.z);
    }
}
