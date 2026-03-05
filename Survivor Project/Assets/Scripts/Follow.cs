using UnityEngine;

public class Follow : MonoBehaviour
{
    private RectTransform rect;
    private Camera mainCam;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    void FixedUpdate()
    {
        if (mainCam != null && GameManager.instance.player != null)
        {
            rect.position = mainCam.WorldToScreenPoint(GameManager.instance.player.transform.position);
        }
    }
}