using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BeanManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private BeanSegmentScript segmentPrefab;
    [SerializeField] private Transform spawnStart;
    [SerializeField] private float segmentHeight = 2f;
    [SerializeField] private int initialSegment = 8;

    private readonly List<BeanSegmentScript> activeSegments = new List<BeanSegmentScript> ();
    void Start()
    {
        Debug.Log("Skrypt ruszył");
        Debug.Log($"--- START DIAGNOSTYKI ---");
        Debug.Log($"Prefab: {(segmentPrefab != null ? segmentPrefab.name : "BRAK (NULL!)")}");
        Debug.Log($"SpawnOrigin: {(spawnStart != null ? spawnStart.name : "BRAK (NULL!)")}");
        Debug.Log($"Wartość initialSegments z Inspektora: {initialSegment}");
        GenerateInitialStalk();
    }

    private void GenerateInitialStalk()
    {

        if (spawnStart == null)
        {
            Debug.LogError("[BeanManager] BŁĄD: Spawn Origin nie jest przypisany w Inspektorze!");
            return;
        }

        if (segmentPrefab == null)
        {
            Debug.LogError("[BeanManager] BŁĄD: Segment Prefab nie jest przypisany w Inspektorze!");
            return;
        }

        Debug.Log($"[BeanManager] Zaczynam generować pętlę dla {initialSegment} segmentów...");

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
            Debug.Log($"[BeanManager] Wygenerowano segment nr {i} na pozycji {spawnPosition}");
        }
        Debug.Log($"[BeanManager] Zakończono generowanie. Łącznie segmentów w liście: {activeSegments.Count}");
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
        Vector3 topPosition = spawnStart.position + Vector3.up * (activeSegments.Count * segmentHeight);
        BeanSegmentScript topSegment = Instantiate(segmentPrefab, topPosition, Quaternion.identity, transform);
        topSegment.Setup(RandomSide());
        activeSegments.Add(topSegment);
    }
}
