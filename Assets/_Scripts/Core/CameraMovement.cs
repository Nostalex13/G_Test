using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Vector3 cameraMovementOffset;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endViewOffset;
    [SerializeField] float cameraMovementTime = 0.5f;
    [SerializeField] TweenPosition positionTween;

    bool blockMovement = false;
    PlatformManager platformManager;

    new Transform transform;
    Vector3 targetPos;

    Coroutine movementCoroutine;

    public static event System.Action OnCameraMoveFinished;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        positionTween.AddOnFinished(() =>
        {
            blockMovement = false;
            OnCameraMoveFinished?.Invoke();
        });
    }

    private void Start()
    {
        platformManager = GameManager.Instance.PlatformManager;
    }

    private void OnEnable()
    {
        GameManager.OnGameStart += RepositionCamera;
        GameManager.OnGameOver += FocusCameraOnPlatforms;
        PlatformManager.OnLastTowerReproduced += FocusOnLastTower;
    }

    private void OnDisable()
    {
        TapController.OnTap -= MoveUp;
        GameManager.OnGameOver -= FocusCameraOnPlatforms;
        GameManager.OnGameStart -= RepositionCamera;
        PlatformManager.OnLastTowerReproduced -= FocusOnLastTower;
    }

    private void RepositionCamera()
    {
        if ((transform.position - startPosition).sqrMagnitude < 0.01f)
        {
            OnCameraMoveFinished?.Invoke();
        }
        else
        {
            MoveCamera(transform.position, startPosition);
        }

        TapController.OnTap += MoveUp;
    }

    private void MoveCamera(Vector3 from, Vector3 to)
    {
        blockMovement = true;
        positionTween.from = from;
        positionTween.to = to;
        positionTween.ResetToBeginning();
        positionTween.PlayForward();
    }

    private void MoveUp()
    {
        if (!blockMovement)
        {
            movementCoroutine = StartCoroutine(StartCameraMovement());
        }
    }

    IEnumerator StartCameraMovement()
    {
        Vector3 currentPos = transform.position;
        targetPos = new Vector3(startPosition.x + platformManager.PreviousPlatformTrans.position.x,
            transform.position.y,
            startPosition.z + platformManager.PreviousPlatformTrans.position.z);
        targetPos += cameraMovementOffset;

        float moveOffset = 0f;

        while (moveOffset < cameraMovementTime)
        {
            moveOffset += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(currentPos, targetPos, moveOffset / cameraMovementTime);

            yield return null;
        }
    }

    private void FocusOnLastTower()
    {
        FocusCameraOnPlatforms(true);
    }

    private void FocusCameraOnPlatforms()
    {
        FocusCameraOnPlatforms(false);
    }

    private void FocusCameraOnPlatforms(bool immediately)
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        var platformSpawnManager = GameManager.Instance.PlatformManager;

        Vector3 lastPlatform = new Vector3(0f, platformSpawnManager.PreviousPlatformTrans.position.y, 0f);
        Vector3 firstPlatform = platformSpawnManager.StartPlatformTrans.position;

        Vector3 distance = firstPlatform - lastPlatform;
        Vector3 midPoint = platformSpawnManager.PreviousPlatformTrans.position + distance / 2f;

        Vector3 viewportFirst = GetComponent<Camera>().WorldToViewportPoint(firstPlatform);
        Vector3 viewportLast = GetComponent<Camera>().WorldToViewportPoint(lastPlatform);

        float initialDistance = 30f;
        float maxViewportOffset = 5f;
        float relativeViewportOffset = (viewportFirst.z - viewportLast.z) / maxViewportOffset;
        float focusDistance = relativeViewportOffset > 1f ? initialDistance * relativeViewportOffset : initialDistance;

        Vector3 cameraTargetPos = midPoint + (-transform.forward * focusDistance);
        cameraTargetPos.z = -cameraTargetPos.x;
        cameraTargetPos += endViewOffset;

        if (immediately)
            transform.position = cameraTargetPos;
        else
            MoveCamera(transform.position, cameraTargetPos);

        TapController.OnTap -= MoveUp;
    }
}
