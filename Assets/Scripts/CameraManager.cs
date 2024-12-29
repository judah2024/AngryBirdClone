using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineVirtualCamera followCamera;
    [SerializeField] private CinemachineVirtualCamera mainCamera;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeFollowCamera(Transform followTransform)
    {
        followCamera.Priority = 11;
        followCamera.Follow = followTransform;
    }

    public void ChangeMainCamera()
    {
        followCamera.Priority = 0;
    }
}
