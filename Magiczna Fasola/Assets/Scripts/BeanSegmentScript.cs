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
    }

}
