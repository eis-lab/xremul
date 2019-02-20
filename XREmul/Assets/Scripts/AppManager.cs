using UnityEngine;

public class AppManager : MonoBehaviour
{
    private Renderer _lastColliderRenderer;
    public Camera activeCamera;

    private void Start()
    {
        // Look at sphere position
        activeCamera.transform.LookAt(Vector3.zero);
    }

    private void Update()
    {
        var ray = activeCamera.ScreenPointToRay(Input.mousePosition);

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