using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    [SerializeField] float deactivateHeightDifference = 50f;

    new Rigidbody rigidbody;
    new Renderer renderer;
    MaterialPropertyBlock materialPropertyBlock;
    new Transform transform;

    Coroutine deactivateCoroutine;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
        materialPropertyBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(materialPropertyBlock);
    }

    public void Init(Color color, Vector3 scale, Vector3 position)
    {
        materialPropertyBlock.SetColor("_Color", color);
        renderer.SetPropertyBlock(materialPropertyBlock);

        rigidbody.velocity = Vector3.zero;
        transform.localScale = scale;
        transform.position = position;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);

        if (deactivateCoroutine == null)
        {
            deactivateCoroutine = StartCoroutine(StartCheckForDeactivate());
        }
    }

    IEnumerator StartCheckForDeactivate()
    {
        var cameraTransform = GameManager.Instance.MainCamera.transform;
        var waitFor = new WaitForSeconds(2f);

        while (gameObject.activeInHierarchy)
        {
            yield return waitFor;

            if (transform.position.y + deactivateHeightDifference < cameraTransform.position.y)
            {
                gameObject.SetActive(false);
                StopCoroutine(deactivateCoroutine);
                deactivateCoroutine = null;
            }
        }

        deactivateCoroutine = null;
    }
}
