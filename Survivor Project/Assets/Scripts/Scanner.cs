using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public Transform nearestTarget;

    private Collider2D[] targets = new Collider2D[100]; 
    private int targetCount; 
    private ContactFilter2D filter; 

    void Awake()
    {
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = targetLayer;
        filter.useTriggers = true; 
    }

    void FixedUpdate()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Playing) return;

        targetCount = Physics2D.OverlapCircle(transform.position, scanRange, filter, targets);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100f * 100f;

        for (int i = 0; i < targetCount; i++)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = targets[i].transform.position;
            
            float curDiff = (myPos - targetPos).sqrMagnitude; 

            if (curDiff < diff)
            {
                diff = curDiff;
                result = targets[i].transform; 
            }
        }
        return result;
    }
}