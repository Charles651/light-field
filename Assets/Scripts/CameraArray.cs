using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArray : MonoBehaviour {
    public GameObject m_Origin;
    //public GameObject m_Array;
    private int[] m_Count = { 32,18 };
    
    //定义光学参数，单位cm
    public static float EYEBOX = 1.1f;
    public static float PITCH = 0.16f;
    public static float GAP = 0.5f;
    public static int PPI = 445; 
    private float exitPupilLength;
    private float elementalWidth;
    private float m_fieldOfView;
    public Matrix4x4 tempMatrix;


    // Use this for initialization
    void Start () {
        exitPupilLength  = EYEBOX / PITCH * GAP - GAP;
        elementalWidth = EYEBOX / exitPupilLength * GAP;
        m_fieldOfView = Mathf.Atan(EYEBOX / 2 / exitPupilLength) / Mathf.PI * 360;
        float xMin, yMin, xMax, yMax;

        m_Origin = GameObject.Find("Main Camera");
        m_Origin.GetComponent<Camera>().fieldOfView = m_fieldOfView;
        //m_Array = GameObject.Find("Array");
        Vector3 originPos = m_Origin.transform.localPosition;
        Quaternion originRot = m_Origin.transform.localRotation;
        float x = 1.0f / m_Count[0];
        float y = 1.0f / m_Count[1];
        yMin = -PITCH/2;
        yMax = yMin + elementalWidth;
        for (int i = 0; i < m_Count[1] / 2; i++)
        {
            xMin = -PITCH / 2;
            xMax = xMin + elementalWidth;
            for (int j = 0; j < m_Count[0] / 2; j++)
            {
                CreatCamera(new Vector3(PITCH * (j + 0.5f), PITCH * (i + 0.5f), 0), new Rect(x * j + 0.5f, y * i + 0.5f, x, y), xMin, xMax, yMin, yMax);
                CreatCamera(new Vector3(-PITCH * (j + 0.5f), PITCH * (i + 0.5f), 0), new Rect(-x * j + 0.5f, y * i + 0.5f, x, y), -xMax, -xMin, yMin, yMax);
                CreatCamera(new Vector3(PITCH * (j + 0.5f), -PITCH * (i + 0.5f), 0), new Rect(x * j + 0.5f, -y * i + 0.5f, x, y), xMin, xMax, -yMax, -yMin);
                CreatCamera(new Vector3(-PITCH * (j + 0.5f), -PITCH * (i + 0.5f), 0), new Rect(-x * j + 0.5f, -y * i + 0.5f, x, y), -xMax, -xMin, -yMax, -yMin);
                xMin += elementalWidth - PITCH;
                xMax += elementalWidth - PITCH;
            }
            yMin += elementalWidth - PITCH;
            yMax += elementalWidth - PITCH;
        }


        //m_Origin = GameObject.Find("Main Camera");
        ////m_Array = GameObject.Find("Array");
        //Vector3 originPos = m_Origin.transform.localPosition;
        //float x = 1.0f / m_Count[0];
        //float y = 1.0f / m_Count[1];
        //for (int i = -m_Count[1]/2; i < m_Count[1]/2; i++)
        //{
        //    for(int j = -m_Count[0]/2; j < m_Count[0]/2; j++)
        //    {
        //        GameObject newCamera = Instantiate(m_Origin, originPos + new Vector3(x * i, y * j, 0),m_Origin.transform.localRotation);
        //        newCamera.transform.parent = transform;
        //        newCamera.transform.localPosition = new Vector3(0.1f * j, 0.1f * i, 0);
        //        newCamera.GetComponent<Camera>().rect = new Rect(x * j + 0.5f, y * i + 0.5f, x, y);
        //    }
        //}
        Destroy(m_Origin);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    GameObject CreatCamera(Vector3 position, Rect spectRect, float xMin, float xMax, float yMin, float yMax)
    {
        GameObject newCamera = Instantiate(m_Origin);
        newCamera.transform.parent = transform;
        newCamera.transform.localPosition = position;
        newCamera.GetComponent<Camera>().rect = spectRect;
        tempMatrix = newCamera.GetComponent<Camera>().projectionMatrix;
        tempMatrix.m02 = (xMax + xMin) / elementalWidth;
        tempMatrix.m12 = (yMax + yMin) / elementalWidth;
        newCamera.GetComponent<Camera>().projectionMatrix = tempMatrix;
        return newCamera;
    }
}
