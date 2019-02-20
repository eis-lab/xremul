using System;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public enum CollidingState
    {
        ManualMouse,
        CameraDirection
    }

    private Renderer _lastColliderRenderer;
    public Camera activeCamera;

    public CollidingState currentCollidingState = CollidingState.ManualMouse;

    private void Start()
    {
        // Look at sphere position
        activeCamera.transform.LookAt(Vector3.zero);
    }

    private void Update()
    {
        GetRayFrom getRayFromFunc;

        switch (currentCollidingState)
        {
            case CollidingState.ManualMouse:
                getRayFromFunc = cam => cam.ScreenPointToRay(Input.mousePosition);
                break;
            case CollidingState.CameraDirection:
                getRayFromFunc = cam =>
                {
                    var transform = cam.transform;

                    return new Ray(transform.position, transform.forward);
                };
                break;
            default:
                throw new Exception("You're not written all of the enum config");
        }

        var ray = getRayFromFunc(activeCamera);

        if (Physics.Raycast(ray, out var hitInfo))
        {
            var colliderGameObject = hitInfo.collider.gameObject;

            var colliderRenderer = colliderGameObject.GetComponent<Renderer>();

            colliderRenderer.material.color = Color.red;

            _lastColliderRenderer = colliderRenderer;
        }
        else
        {
            if (_lastColliderRenderer)
                _lastColliderRenderer.material.color = Color.white;
        }
    }
}