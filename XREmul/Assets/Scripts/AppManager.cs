using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class AppManager : MonoBehaviour
{
    public enum CollidingState
    {
        ManualMouse,
        CameraDirection
    }

    private readonly List<CameraTransformLog> _cameraTransformLogs = new List<CameraTransformLog>();

    private Camera _activeCamera;

    private Renderer _lastColliderRenderer;

    private double _recordingTimer;
    private double _replayingTimer;

    public CollidingState currentCollidingState = CollidingState.CameraDirection;

    [FormerlySerializedAs("activeCamera")] public Camera recordingCamera;
    public Camera replayingCamera;

    public bool IsRecording { get; set; }
    public bool IsReplaying { get; set; }

    private void Start()
    {
        // Look at sphere position
        recordingCamera.transform.LookAt(Vector3.zero);
    }

    private void Update()
    {
        OnCollideWithSphereOrNot();

        if (!IsRecording && Input.GetKeyDown("r"))
        {
            ResetRecordingTimer();
            IsRecording = true;
        }

        if (!IsReplaying && Input.GetKeyDown("space"))
        {
            ResetReplayingTimer();
            IsReplaying = true;
        }

        if (IsRecording)
            OnRecord();
        if (IsReplaying)
            OnReplay();
    }

    private void OnCollideWithSphereOrNot()
    {
        if (!_activeCamera)
            return;

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

        var ray = getRayFromFunc(_activeCamera);

        if (Physics.Raycast(ray, out var hitInfo))
        {
            var colliderGameObject = hitInfo.collider.gameObject;

            var colliderRenderer = colliderGameObject.GetComponent<Renderer>();

            colliderRenderer.material.color = Color.red; // 아예 이런 종류의 Renderer를 굽는 방법도 있음.

            _lastColliderRenderer = colliderRenderer;
        }
        else
        {
            if (_lastColliderRenderer)
                _lastColliderRenderer.material.color = Color.white;
        }
    }

    public void OnRecord()
    {
        double kFixedRecordingTime = 10;

        var cameraTransform = recordingCamera.transform;

        _cameraTransformLogs.Add(new CameraTransformLog
        {
            timestamp = _recordingTimer,
            position = cameraTransform.position,
            rotation = cameraTransform.rotation
        });

        if (_recordingTimer > kFixedRecordingTime)
        {
            ResetRecordingTimer();
            IsRecording = false;

            print("Recording Is Over");
        }

        _recordingTimer += Time.deltaTime;
    }

    public void OnReplay()
    {
        var kThreshold = 0.1;

        var matchedLog = _cameraTransformLogs
            .FirstOrDefault(log => log.timestamp > _replayingTimer);

        if (matchedLog != null)
        {
            replayingCamera.transform.position = matchedLog.position;
            replayingCamera.transform.rotation = matchedLog.rotation;
        }
        else
        {
            ResetReplayingTimer();
            IsReplaying = false;

            print("Replaying Is Over");
        }

        _replayingTimer += Time.deltaTime;
    }

    public void ResetRecordingTimer()
    {
        recordingCamera.enabled = true;
        replayingCamera.enabled = false;

        _activeCamera = recordingCamera;

        _recordingTimer = 0.0f;
    }

    public void ResetReplayingTimer() // FIXME: Generalize for the file based log. This is only for memory based log.
    {
        recordingCamera.enabled = false;
        replayingCamera.enabled = true;

        _activeCamera = replayingCamera;

        _replayingTimer = 0.0f;
    }

    private class CameraTransformLog
    {
        public Vector3 position;
        public Quaternion rotation;
        public double timestamp;
    }

    private delegate Ray GetRayFrom(Camera camera);
}