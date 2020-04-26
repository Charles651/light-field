using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraArray : MonoBehaviour
{
    #region 公有变量
    [Tooltip("相机单元（该相机的视场角为所有相机的基准）")]
    public GameObject m_Origin;
    [Tooltip("elemental image 在 x 和 y 方向的数量")]
    public int[] m_Count = { 8, 6 };
    //定义光学参数，单位cm
    // public static float DLENS = 0.055f;   //透镜孔径，没用
    // [Tooltip("Eyebox 直径，单位 cm")]
    // public float m_EyeBox = 0.7f;
    [Tooltip("出瞳距离，单位 cm")]
    public float m_ExitPupilLength = 2.5f;
    [Tooltip("透镜单元的水平中心距，单位 cm")]
    public float m_Pitch = 0.16f;
    [Tooltip("物距，单位 cm")]
    public float m_Gap = 0.25f;
    [Tooltip("屏幕尺寸，单位 inch")]
    public float m_ScreenSize = 0.7f;
    // public static float PUPIL_DISTANCE = 6.5f;
    [Tooltip("显示屏分辨率")]
    public int[] m_Resolution = { 1920, 1080 };
    // [Tooltip("PPI")]
    // public float m_PPI = 5080;   //应该是3147，但是3168对OLED来说更准
    [Tooltip("子图像尺寸，根据计算或仿真自定，单位 cm")]
    public float m_ImgWidth = 0.08f; //子图像宽度
    #endregion
    #region 私有变量
    // private float exitPupilLength;
    private float imgPeriod; //子图像间距
    private float fieldOfView;
    public Matrix4x4 tempMatrix;
    #endregion

    // Use this for initialization
    void Start()
    {
        float pixelSize = m_ScreenSize * 2.54f / Mathf.Sqrt(Mathf.Pow(m_Resolution[0], 2) + Mathf.Pow(m_Resolution[1], 2));
        imgPeriod = m_Pitch / m_ExitPupilLength * (m_ExitPupilLength + m_Gap);
        Debug.Log("imgPeriod: " + imgPeriod);//观察平面上的子图像周期，单位厘米
        fieldOfView = Mathf.Atan(m_ImgWidth / 2 / m_Gap) / Mathf.PI * 360; //单个camera的视场角，其实设置成多少都可以
        Debug.Log("FOV: " + fieldOfView);

        m_Origin.GetComponent<Camera>().fieldOfView = fieldOfView;
        Vector3 originPos = m_Origin.transform.localPosition;

        float elementalRectWidth = m_ImgWidth / pixelSize / m_Resolution[0]; //归一化的子图像大小，无量纲
        float elementalRectHeight = m_ImgWidth / pixelSize / m_Resolution[1]; //归一化的子图像大小，无量纲
        float xPeriod = imgPeriod / pixelSize / m_Resolution[0]; //归一化的子图像间距，无量纲
        float yPeriod = imgPeriod / pixelSize / m_Resolution[1]; //归一化的子图像间距，无量纲
        float posYMin = -m_ImgWidth / 2 - (imgPeriod - m_Pitch) * (m_Count[1] - 1) / 2;
        float posYMax = posYMin + m_ImgWidth;

        for (int i = 0; i < m_Count[1]; i++)
        {
            float posXMin = -m_ImgWidth / 2 - (imgPeriod - m_Pitch) * (m_Count[0] - 1) / 2;
            float posXMax = posXMin + m_ImgWidth;
            for (int j = 0; j < m_Count[0]; j++)
            {
                if (0 == (i + j) % 2)
                    CreateCamera(new Vector3(m_Pitch * (j - ((float)m_Count[1] - 1) / 2), m_Pitch * (i - ((float)m_Count[0] - 1) / 2), 0), new Rect(0.5f + xPeriod * (j - ((float)m_Count[1] - 1) / 2) - elementalRectWidth / 2, 0.5f + yPeriod * (i - ((float)m_Count[0] - 1) / 2) - elementalRectHeight / 2, elementalRectWidth, elementalRectHeight), posXMin, posXMax, posYMin, posYMax);
                posXMin += imgPeriod - m_Pitch;
                posXMax += imgPeriod - m_Pitch;
            }
            posYMin += imgPeriod - m_Pitch;
            posYMax += imgPeriod - m_Pitch;
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
        tempMatrix.m02 = (xMax + xMin) / m_ImgWidth;
        tempMatrix.m12 = (yMax + yMin) / m_ImgWidth;
        newCamera.GetComponent<Camera>().projectionMatrix = tempMatrix;
        return newCamera;
    }
}