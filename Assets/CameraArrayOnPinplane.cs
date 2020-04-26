using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArrayOnPinplane : MonoBehaviour
{
    public GameObject OriginCamera;
    public GameObject LensArray;

    /// <summary>
    /// 显示屏参数
    /// </summary>
    //public float m_ActiveSize = 5.7f;
    // [Tooltip("显示屏大小-近裁面（投影面），单位cm")]
    // public float[] Area = { 6.804f, 12.096f };  //0.7英寸大小：0.8716845、1.549661；5.7英寸：7.098002、12.61867
    [Tooltip("显示屏大小-近裁面（投影面），单位cm")]
    public float m_ScreenSize = 2.1f;  //0.7英寸大小：0.8716845、1.549661；5.7英寸：7.098002、12.61867
    [Tooltip("显示屏分辨率")]
    public int[] m_Resolution = { 5760, 5760 };  //LPM055A184A
    [Tooltip("子图分辨率")]
    public int[] subResolution = { 144, 192 };
    [Tooltip("出瞳距离，单位cm")]
    public float m_EyeRelief = 2;

    /// <summary>
    /// 透镜阵列（相机阵列）参数
    /// </summary>
    [Tooltip("小孔阵列到屏的距离")]
    public float m_Gap = 0.498f;
    [Tooltip("小孔间隔")]
    public float m_LensPitch = 0.1f;
    [Tooltip("小孔/相机数量")]
    public int[] m_CameraAmount = { 5, 5 };

    /// <summary>
    /// 人眼模型
    /// </summary>
    [Tooltip("eyebox")]
    [SerializeField]
    private float eyebox;
    [Tooltip("像素尺寸，单位cm")]
    [SerializeField]
    private float pixelsize;

    private GameObject[] cameraArray;
    [SerializeField]
    public Matrix4x4 Projection = new Matrix4x4();
    [SerializeField]
    private Matrix4x4 TempMatrix;
    [SerializeField]
    private float cameraAspect;
    [SerializeField]
    float[] pinArea = new float[2];
    private float[] screenSide= new float[2];

    // Use this for initialization
    void Start()
    {
        //透镜阵列大小:这里没有定义真正的微透镜阵列大小，只是为了求出相机要放置的位置！
        pinArea[0] = m_CameraAmount[0] * m_LensPitch;
        pinArea[1] = m_CameraAmount[1] * m_LensPitch;
        screenSide[0]=m_ScreenSize*m_Resolution[0]/Mathf.Sqrt((float)(m_Resolution[0]*m_Resolution[0]+m_Resolution[1]*m_Resolution[1]));
        screenSide[1]=m_ScreenSize*m_Resolution[1]/Mathf.Sqrt((float)(m_Resolution[0]*m_Resolution[0]+m_Resolution[1]*m_Resolution[1]));
        pixelsize = screenSide[0] / m_Resolution[0];
        eyebox = m_LensPitch * (1 + m_EyeRelief / m_Gap);
        cameraArray = new GameObject[m_CameraAmount[0] * m_CameraAmount[1]];
        Projection = OriginCamera.GetComponent<Camera>().projectionMatrix;
        for (int i = 0; i < m_CameraAmount[0]; i++)
        {
            for (int j = 0; j < m_CameraAmount[1]; j++)
            {
                //////////////////////////////////////////////////////////////////////每个相机的位置
                float y = (m_CameraAmount[0] - 1) / 2 * m_LensPitch - m_LensPitch * i;
                float x = -(m_CameraAmount[1] - 1) / 2 * m_LensPitch + m_LensPitch * j;
                //float y = pinArea[0] / 2 - lensPitch * i;
                //float x = -pinArea[1] / 2 + lensPitch * j;
                //float y = pinArea[0] / 2 - pinArea[0] / (cameraAmount[0] - 1) * i;
                //float x = -pinArea[1] / 2 + pinArea[1] / (cameraAmount[1] - 1) * j;
                //float y = pinArea[1] / 2 - pinArea[1] / (cameraAmount[1] - 1) * j;
                //float x = -pinArea[0] / 2 + pinArea[0] / (cameraAmount[0] - 1) * i;
                //////////////////////////////////////////////相机截屏边界求法:相似三角形
                float Yrange = eyebox / m_EyeRelief * m_Gap;
                float Xrange = eyebox / m_EyeRelief * m_Gap;
                float Ymax = y * m_Gap / m_EyeRelief + eyebox * m_Gap / (2 * m_EyeRelief);
                float Ymin = y * m_Gap / m_EyeRelief - eyebox * m_Gap / (2 * m_EyeRelief);
                float Xmax = x * m_Gap / m_EyeRelief + eyebox * m_Gap / (2 * m_EyeRelief);
                float Xmin = x * m_Gap / m_EyeRelief - eyebox * m_Gap / (2 * m_EyeRelief);

                //float Ymax = y * (1 + Gap / eyeRelief) + (eyebox * Gap) / (2 * eyeRelief);
                //float Ymin = y * (1 + Gap / eyeRelief) - (eyebox * Gap) / (2 * eyeRelief);
                //float Xmax = x * (1 + Gap / eyeRelief) + (eyebox * Gap) / (2 * eyeRelief);
                //float Xmin = x * (1 + Gap / eyeRelief) - (eyebox * Gap) / (2 * eyeRelief);
                cameraArray[i * m_CameraAmount[1] + j] = CreateCam(new Vector3(x, y, 0), new Rect(0, 0, 1, 1), Xmin, Ymin, Xmax, Ymax);
                cameraArray[i * m_CameraAmount[1] + j].transform.parent = LensArray.transform;
                cameraArray[i * m_CameraAmount[1] + j].name = "Camera" + (i * m_CameraAmount[0] + j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < m_CameraAmount[0]; i++)
            {
                for (int j = 0; j < m_CameraAmount[1]; j++)
                {
                    //float Yrange = eyebox / eyeRelief * Gap;
                    //float Xrange = eyebox / eyeRelief * Gap;
                    CaptureCam(cameraArray[i * m_CameraAmount[0] + j].GetComponent<Camera>(), i, j, new Rect(0, 0, subResolution[0], subResolution[1]));//这里的Rect值是指摄像机在游戏界面显示的位置和大小（0为最小，1为最大）


                }
            }
        }
    }

    //生成透视斜投影相机
    GameObject CreateCam(Vector3 position, Rect spectRect, float Xmin, float Ymin, float Xmax, float Ymax)
    {
        GameObject newCamera = Instantiate(OriginCamera);
        newCamera.transform.parent = transform;
        newCamera.transform.localPosition = position;
        // 错误
        // newCamera.GetComponent<Camera>().aspect = Ymax / Xmax;
        newCamera.GetComponent<Camera>().aspect = (Ymax - Ymin) / (Xmax - Xmin);
        ////////////////////////////////////////////////视场角、投影矩阵
        //newCamera.GetComponent<Camera>().fieldOfView = (Mathf.Atan((eyeRelief + Gap) / (Ymin - eyebox / 2)) - Mathf.Atan((eyeRelief + Gap) / (Ymax + eyebox / 2))) * Mathf.Rad2Deg;
        TempMatrix = newCamera.GetComponent<Camera>().projectionMatrix;
        //斜投影
        TempMatrix.m00 = 2 * m_Gap / (Xmax - Xmin);
        TempMatrix.m11 = 2 * m_Gap / (Ymax - Ymin);
        TempMatrix.m02 = (Xmax + Xmin) / (Xmax - Xmin);
        TempMatrix.m12 = (Ymax + Ymin) / (Ymax - Ymin);
        //正投影
        //TempMatrix.m02 = 0;
        //TempMatrix.m12 = 0;
        newCamera.GetComponent<Camera>().projectionMatrix = TempMatrix;

        return newCamera;

    }

    //截图
    Texture2D CaptureCam(Camera camera, int i, int j, Rect rect)
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 1);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
        //ps: camera2.targetTexture = rt;  
        //ps: camera2.Render();  
        //ps: -------------------------------------------------------------------  

        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();

        // // 重置相关参数，以使用camera继续在屏幕上显示  
        // camera.targetTexture = null;
        // //ps: camera2.targetTexture = null;  
        // RenderTexture.active = null; // JC: added to avoid errors  
        Destroy(rt);
        // 最后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.persistentDataPath + (j + i * m_CameraAmount[0]) + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        return screenShot;
    }
}


