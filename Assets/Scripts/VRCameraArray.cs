﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCameraArray : MonoBehaviour
{
    #region 公有变量
    [Tooltip("相机单元（该相机的视场角为所有相机的基准）")]
    public GameObject m_Origin;
    [Tooltip("elemental image 在 x 和 y 方向的数量")]
    public int[] m_Count = { 8, 6 };
    [Tooltip("Eyebox 直径，单位 cm")]
    public float m_EyeBox = 1.04f;
    [Tooltip("透镜单元的水平中心距，单位 cm")]
    public float m_Pitch = 0.08f;
    [Tooltip("物距，单位 cm")]
    public float m_Gap = 0.50f;
    [Tooltip("显示屏对角线尺寸，单位 inch")]
    public float m_ScreenSize = 0.7f;
    // public static float PUPIL_DISTANCE = 3.0f;
    // [Tooltip("PPI")]
    // public int m_PPI = 3147;
    [Tooltip("显示屏分辨率")]
    public int[] m_Resolution = { 1920, 1080 };
    #endregion
    #region 私有变量
    private float imgWidth;
    private float fieldOfView;
    [SerializeField]
    private float exitPupilLength;
    private Matrix4x4 tempMatrix;
    #endregion

    // Use this for initialization

    void Start()
    {
        //float pupDis = PUPIL_DISTANCE / (SCREEN_SIZE / Mathf.Sqrt(Mathf.Pow(16, 2) + Mathf.Pow(9, 2)) * 16 * 2.54f) / 2;
        float pixelSize = m_ScreenSize * 2.54f / Mathf.Sqrt(Mathf.Pow(m_Resolution[0], 2) + Mathf.Pow(m_Resolution[1], 2));
        // float pixelSize = 0.000825f;
        exitPupilLength = m_EyeBox / m_Pitch * m_Gap - m_Gap;
        Debug.Log("Exit pupil length: " + exitPupilLength);
        imgWidth = m_EyeBox / exitPupilLength * m_Gap;
        Debug.Log("image width = " + imgWidth);
        fieldOfView = Mathf.Atan(m_EyeBox / 2 / exitPupilLength) / Mathf.PI * 360;
        Debug.Log("FOV: " + fieldOfView);

        // float xMin, yMin, xMax, yMax;
        // m_Origin = GameObject.Find("Main Camera");
        m_Origin.GetComponent<Camera>().fieldOfView = fieldOfView;
        //m_Array = GameObject.Find("Array");
        Vector3 originPos = m_Origin.transform.localPosition;
        // Quaternion originRot = m_Origin.transform.localRotation;
        float elementalRectWidth = imgWidth / pixelSize / m_Resolution[0];
        float elementalRectHeight = imgWidth / pixelSize / m_Resolution[1];
        float posYMin = -m_Pitch / 2 - (imgWidth - m_Pitch) * m_Count[1];
        float posYMax = posYMin + imgWidth;
        for (int i = -m_Count[1]; i < m_Count[1]; i++)
        {
            float posXMin = -m_Pitch / 2 - (imgWidth - m_Pitch) * m_Count[0];
            float posXMax = posXMin + imgWidth;
            for (int j = -m_Count[0]; j < m_Count[0]; j++)
            {
                CreateCamera(new Vector3(m_Pitch * (j + 0.5f), m_Pitch * (i + 0.5f), 0), new Rect(elementalRectWidth * j + 0.5f, elementalRectHeight * i + 0.5f, elementalRectWidth, elementalRectHeight), posXMin, posXMax, posYMin, posYMax);
                // CreatCamera(new Vector3(-m_Pitch * (j + 0.5f), m_Pitch * (i + 0.5f), 0), new Rect(-elementalRectWidth * (j + 1) + 0.5f, elementalRectHeight * i + 0.5f, elementalRectWidth, elementalRectHeight), -posXMax, -posXMin, posYMin, posYMax);
                // CreatCamera(new Vector3(m_Pitch * (j + 0.5f), -m_Pitch * (i + 0.5f), 0), new Rect(elementalRectWidth * j + 0.5f, -elementalRectHeight * (i + 1) + 0.5f, elementalRectWidth, elementalRectHeight), posXMin, posXMax, -posYMax, -posYMin);
                // CreatCamera(new Vector3(-m_Pitch * (j + 0.5f), -m_Pitch * (i + 0.5f), 0), new Rect(-elementalRectWidth * (j + 1) + 0.5f, -elementalRectHeight * (i + 1) + 0.5f, elementalRectWidth, elementalRectHeight), -posXMax, -posXMin, -posYMax, -posYMin);
                posXMin += imgWidth - m_Pitch;
                posXMax += imgWidth - m_Pitch;
            }
            posYMin += imgWidth - m_Pitch;
            posYMax += imgWidth - m_Pitch;
        }
        m_Origin.SetActive(false);
    }

    GameObject CreateCamera(Vector3 position, Rect spectRect, float xMin, float xMax, float yMin, float yMax)
    {
        GameObject newCamera = Instantiate(m_Origin);
        newCamera.transform.parent = transform;
        newCamera.transform.localPosition = position;
        newCamera.GetComponent<Camera>().rect = spectRect;
        tempMatrix = newCamera.GetComponent<Camera>().projectionMatrix;
        tempMatrix.m02 = (xMax + xMin) / imgWidth;
        tempMatrix.m12 = (yMax + yMin) / imgWidth;
        newCamera.GetComponent<Camera>().projectionMatrix = tempMatrix;
        return newCamera;
    }
}