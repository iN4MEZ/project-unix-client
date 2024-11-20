using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class CameraZoom : MonoBehaviour
    {
        private CinemachineFramingTransposer framingTransponser;

        private CinemachineVirtualCamera cineMachineVirtualCam;

        private CinemachineInputProvider inputProvider;

        public static CameraZoom instance;

        public Transform Follow;
        public Transform LookAt;

        private float currentTargetDistance;

        [field: SerializeField] public CameraData cameraData;

        private GameObject camCenter;

        private void Awake()
        {

            instance = this;

            DontDestroyOnLoad(gameObject);


            cineMachineVirtualCam = GetComponent<CinemachineVirtualCamera>();

            framingTransponser = cineMachineVirtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();

            inputProvider = GetComponent<CinemachineInputProvider>();

            currentTargetDistance = cameraData.defaultDistance;
        }

        private void Start()
        {
        }

        public void AddCamRef()
        {
            camCenter = GameObject.FindGameObjectWithTag("CameraCenter").gameObject;

            cineMachineVirtualCam.LookAt = camCenter.gameObject.transform;
            cineMachineVirtualCam.Follow = camCenter.gameObject.transform;
        }

        public void SetCamCenterParent(Transform newParent)
        {
            if(newParent != null)
            {
                camCenter.transform.SetParent(newParent);
            }
        }

        private void Update()
        {
            Zoom();
        }

        private void Zoom()
        {
            float zoomValue = inputProvider.GetAxisValue(2) * cameraData.zoomSensitivity;

            currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomValue, cameraData.minimumDistance, cameraData.maximumDistance);

            float currentDistance = framingTransponser.m_CameraDistance;

            if (currentDistance == currentTargetDistance) { return; }

            float lerpedZoomValue = Mathf.Lerp(currentDistance, currentTargetDistance, cameraData.smoothing * Time.deltaTime);

            framingTransponser.m_CameraDistance = lerpedZoomValue;
        }
    }
}
