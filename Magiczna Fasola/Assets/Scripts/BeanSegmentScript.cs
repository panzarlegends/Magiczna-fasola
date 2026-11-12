using UnityEngine;

public enum BranchSide
{
    None,
    Left,
    Right,
    Bean
}
public class BeanSegmentScript : MonoBehaviour
{
    [SerializeField] private GameObject leftLeaf;
    [SerializeField] private GameObject rightLeaf;
    [SerializeField] private GameObject goldenBean;
    [SerializeField] private GameObject winningGlow;

    public BranchSide CurrentSide{  get; private set; }
    public void Setup(BranchSide side)
    {
        CurrentSide = side;
        if(leftLeaf != null)
        {
            leftLeaf.SetActive(side == BranchSide.Left);
        }
        if(rightLeaf != null)
        {
            rightLeaf.SetActive(side == BranchSide.Right);
        }

        if(goldenBean != null) goldenBean.SetActive(side == BranchSide.Bean);
        if (winningGlow != null) winningGlow.SetActive(false);
    }

    //direction 1 if player chops from left, direction -1 when player chops from right
    public void LaunchAndDestroy(int direction)
    {
        Debug.Log("yeet" + direction);
        // separate chopped segment from sequence
        transform.SetParent(null);

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in renderers)
        {
            sr.sortingOrder += 100;
        }

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3.5f;

        // animation parameters
        float forceX = Random.Range(15f, 20f) * direction;
        float forceY = Random.Range(3.5f, 5.5f);
        rb.linearVelocity = new Vector2(forceX, forceY);

        // spin
        rb.AddTorque(-direction * Random.Range(100f, 120f));

        Destroy(gameObject, 1f);
    }

    public void PlayWinningSequence(GameObject winScreen)
    {
        // enabling glow and setting bean with glow to be always on top
        if(goldenBean != null)
        {
            goldenBean.SetActive(true);
            SpriteRenderer beanSr = goldenBean.GetComponent<SpriteRenderer>();
            if(beanSr != null) beanSr.sortingOrder = 100;
        }

        if(winningGlow != null)
        {
            winningGlow.SetActive(true);
            SpriteRenderer glowSr = winningGlow.GetComponent<SpriteRenderer>();
            if (glowSr != null) glowSr.sortingOrder = 99;
        }

        StartCoroutine(WinAnimation(winScreen));
    }

    private System.Collections.IEnumerator WinAnimation(GameObject winScreen)
    {
        Transform beanTransform = goldenBean != null ? goldenBean.transform : transform;
        Transform winningGlowT = winningGlow != null ? winningGlow.transform : transform;

        Vector3 startPos = beanTransform.position;
        Vector3 targetPos = new Vector3(0f, 0f, 0f);

        Vector3 startScale = beanTransform.localScale;
        Vector3 targetScale = startScale * 2.5f;

        Vector3 winStartScale = winningGlowT.localScale;
        Vector3 winTargetScale = winStartScale * 50.0f;

        float moveDuration = 1.2f;
        float holdDuration = 2.0f;
        float elapsed = 0f;

        // bean zoom in
        while (elapsed <= moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            beanTransform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            beanTransform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);

            if (winningGlow != null)
            {
                winningGlowT.Rotate(0f, 0f, -120f * Time.deltaTime);
                winningGlowT.localScale = Vector3.Lerp(winStartScale, winTargetScale, smoothT);
            }

            yield return null;
        }

        beanTransform.position = targetPos;
        beanTransform.localScale = targetScale;

        // pause before win screen
        float holdElapsed = 0f;
        while (holdElapsed < holdDuration)
        {
            holdElapsed += Time.deltaTime;
            winningGlowT.Rotate(0f, 0f, -20f * Time.deltaTime);
            yield return null;
        }

        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }
    }

}
