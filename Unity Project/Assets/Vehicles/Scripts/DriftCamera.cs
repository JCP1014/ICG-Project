using System;
using UnityEngine;

public class DriftCamera : MonoBehaviour
{
    [Serializable]
    public class AdvancedOptions
    {
        public bool updateCameraInUpdate;
        public bool updateCameraInFixedUpdate = true;
        public bool updateCameraInLateUpdate;
        public KeyCode switchViewKey = KeyCode.Space;
        public KeyCode switchOverViewKey = KeyCode.Alpha1;
    }

    public float smoothing = 6f;
    public Transform lookAtTarget;
    public Transform positionTarget;
    public Transform sideView;
    public Transform overView;
    public AdvancedOptions advancedOptions;

    public float speedH = 10.0f;
    public float speedV = 10.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    bool m_ShowingSideView;
    bool m_ShowingOverView;

    private void FixedUpdate()
    {
        if (advancedOptions.updateCameraInFixedUpdate)
            UpdateCamera();
    }

    private void Update()
    {
        if (Input.GetKeyDown(advancedOptions.switchViewKey))
            m_ShowingSideView = !m_ShowingSideView;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_ShowingOverView = !m_ShowingOverView;
        }

        if (advancedOptions.updateCameraInUpdate)
            UpdateCamera();
    }

    private void LateUpdate()
    {
        if (advancedOptions.updateCameraInLateUpdate)
            UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (m_ShowingSideView)
        {
            transform.position = sideView.position;
            transform.rotation = sideView.rotation;
        }
        else if (m_ShowingOverView)
        {
            transform.position = overView.position;
            transform.rotation = overView.rotation;
        }
        else
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");
            transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing);
            // transform.LookAt(lookAtTarget);
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
