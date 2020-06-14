using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float platformMovementSpeed = 4f;
    [SerializeField] GameObject fallPlatformPrefab;
    [SerializeField] RandomGradient randomGradient;

    new Renderer renderer;
    MaterialPropertyBlock materialPropertyBlock;
    new Transform transform;
    Vector3 startPoint;
    Vector3 targetPoint;
    public Color Color { get; private set; }

    FallPlatform fallPlatform;

    Transform previousPlatform;

    Coroutine movementCoroutine;
    System.Action DivisionMode = delegate { };

    public static event System.Action<GameObject> OnPlatformStop;

    private void OnDisable()
    {
        GameManager.OnPlatformsCollided -= SlicePlatform;
    }

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(materialPropertyBlock);
        transform = GetComponent<Transform>();
    }

    public void Init(Transform previousPlatform, float spawnOffset)
    {
        this.previousPlatform = previousPlatform;
        gameObject.SetActive(true);
        InitMovement(spawnOffset);

        Color = randomGradient.GetRandomColor();
        materialPropertyBlock.SetColor("_Color", Color);
        renderer.SetPropertyBlock(materialPropertyBlock);

        TapController.OnTap += StopPlatform;
        GameManager.OnPlatformsCollided += SlicePlatform;
    }

    private void InitMovement(float spawnOffset)
    {
        int randomPoint = Random.Range(0, 4);
        startPoint = new Vector3(previousPlatform.position.x,
            previousPlatform.position.y + previousPlatform.localScale.y,
            previousPlatform.position.z);
        targetPoint = startPoint;

        switch ((SpawnPoint)randomPoint)
        {
            case SpawnPoint.NegativeX:
                DivisionForX(-spawnOffset);
                break;
            case SpawnPoint.PositiveX:
                DivisionForX(spawnOffset);
                break;
            case SpawnPoint.NegativeZ:
                DivisionForZ(-spawnOffset);
                break;
            case SpawnPoint.PositiveZ:
                DivisionForZ(spawnOffset);
                break;
        }

        transform.position = startPoint;
        transform.localScale = previousPlatform.localScale;

        movementCoroutine = StartCoroutine(StartMovement());
    }

    private void DivisionForX(float offset)
    {
        startPoint.x = offset;
        targetPoint.x = -offset;
        DivisionMode = DividePlatformX;
    }

    private void DivisionForZ(float offset)
    {
        startPoint.z = offset;
        targetPoint.z = -offset;
        DivisionMode = DividePlatformZ;
    }

    private void DividePlatformX()
    {
        float difference = transform.position.x - previousPlatform.position.x;
        float newScaleX = transform.localScale.x - Mathf.Abs(difference);
        float newPosX = previousPlatform.position.x + difference / 2f;

        transform.localScale = new Vector3(newScaleX, 1f, transform.localScale.z);
        transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);

        Vector3 fallScale = new Vector3(Mathf.Abs(difference), transform.localScale.y, transform.localScale.z);

        if (difference > 0)
            newPosX = transform.position.x + transform.localScale.x / 2f + fallScale.x / 2f;
        else
            newPosX = transform.position.x - transform.localScale.x / 2f - fallScale.x / 2f;

        Vector3 fallPos = new Vector3(newPosX, transform.position.y, transform.position.z);
        fallScale.x = Mathf.Abs(fallScale.x);

        fallPlatform = PoolerMono.Instance.Pull(fallPlatformPrefab).GetComponent<FallPlatform>();
        fallPlatform.Init(Color, fallScale, fallPos);
    }

    private void DividePlatformZ()
    {
        float difference = transform.position.z - previousPlatform.position.z;
        float newScaleZ = transform.localScale.z - Mathf.Abs(difference);
        float newPosZ = previousPlatform.position.z + difference / 2f;

        transform.localScale = new Vector3(transform.localScale.x, 1f, newScaleZ);
        transform.position = new Vector3(transform.position.x, transform.position.y, newPosZ);

        Vector3 fallScale = new Vector3(transform.localScale.x, transform.localScale.y, Mathf.Abs(difference));

        if (difference > 0)
            newPosZ = transform.position.z + transform.localScale.z / 2f + fallScale.z / 2f;
        else
            newPosZ = transform.position.z - transform.localScale.z / 2f - fallScale.z / 2f;

        Vector3 fallPos = new Vector3(transform.position.x, transform.position.y, newPosZ);
        fallScale.z = Mathf.Abs(fallScale.z);

        fallPlatform = PoolerMono.Instance.Pull(fallPlatformPrefab).GetComponent<FallPlatform>();
        fallPlatform.Init(Color, fallScale, fallPos);
    }

    IEnumerator StartMovement()
    {
        float offset = 0;
        float travelTime = (startPoint - targetPoint).magnitude / platformMovementSpeed;
        Vector3 tempPoint;

        while (true)
        {
            if ((transform.position - targetPoint).sqrMagnitude < 0.01f)
            {
                tempPoint = startPoint;
                startPoint = targetPoint;
                targetPoint = tempPoint;
                transform.position = startPoint;
                offset = 0f;
            }

            offset += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(startPoint, targetPoint, offset / travelTime);

            yield return null;
        }
    }

    private void StopPlatform()
    {
        TapController.OnTap -= StopPlatform;

        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = null;

        OnPlatformStop?.Invoke(gameObject);
    }

    private void SlicePlatform()
    {
        GameManager.OnPlatformsCollided -= SlicePlatform;
        DivisionMode();
    }

    public void SetColor(Color color)
    {
        Color = color;
        materialPropertyBlock.SetColor("_Color", Color);
        renderer.SetPropertyBlock(materialPropertyBlock);
    }
}

public enum SpawnPoint
{
    NegativeX = 0,
    PositiveX = 1,
    NegativeZ = 2,
    PositiveZ = 3
}
