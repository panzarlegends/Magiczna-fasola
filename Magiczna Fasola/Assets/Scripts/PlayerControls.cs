using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private BeanManager beanManager;

    [Header("Movement settings")] 
    [SerializeField] private float sideOffset = 1.5f;
    [SerializeField] private float angle = 17f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) PerformChop(isLeft: true);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) PerformChop(isLeft: false);
    }

    private void PerformChop(bool isLeft)
    {
        float target = isLeft ? -sideOffset : sideOffset;
        transform.position = new Vector3(target, transform.position.y, transform.position.z);

        float targetRotation = isLeft ? -angle : angle;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);

        beanManager.Chop();
    }
}
