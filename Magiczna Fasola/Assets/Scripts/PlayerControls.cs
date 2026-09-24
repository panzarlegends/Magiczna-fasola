using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private BeanManager beanManager;

    [Header("Movement settings")] 
    [SerializeField] private float sideOffset = 1.5f;
    [SerializeField] private float angle = 17f;
    private bool isBroken = false;
    private int score = 0;

    void Update()
    {
        if (isBroken) return;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) PerformChop(isLeft: true);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) PerformChop(isLeft: false);
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
    }
}
