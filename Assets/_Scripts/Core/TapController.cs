using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapController : MonoBehaviour, IPointerDownHandler
{
    public static event System.Action OnTap;

    bool blockTap = false;
    int blocksCount = 0;
    
    private void OnEnable()
    {
        GameManager.OnGameOver += BlockTap;
        GameManager.OnGameStart += BlockTap;
        CameraMovement.OnCameraMoveFinished += UnblockTap;
    }

    private void OnMouseDown()
    {
        GameManager.OnGameOver -= BlockTap;
        GameManager.OnGameStart -= BlockTap;
        CameraMovement.OnCameraMoveFinished -= UnblockTap;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!blockTap)
        {
            OnTap?.Invoke();
        }
    }

    private void BlockTap()
    {
        blocksCount++;

        if (blocksCount > 0)
            blockTap = true;
    }

    private void UnblockTap()
    {
        blocksCount--;

        if (blocksCount <= 0)
            blockTap = false;
    }
}
