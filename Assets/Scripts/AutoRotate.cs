using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] private GameObject rotationObject;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            rotationObject.transform.rotation = mainCamera.transform.rotation;
        }
    }
}
