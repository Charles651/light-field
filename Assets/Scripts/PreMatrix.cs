using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMatrix : MonoBehaviour {

    public Vector3 offset = new Vector3(0, 1, 0);
    public Matrix4x4 matrixW2C;
    public Matrix4x4 matrixProj;
    private Camera camera;


    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();
        matrixW2C = camera.worldToCameraMatrix;
        matrixProj = camera.projectionMatrix;
    }

    // Update is called once per frame
    void LateUpdate () {
        //originalProjection = camera.projectionMatrix;
        camera.worldToCameraMatrix = matrixW2C;
        camera.projectionMatrix = matrixProj;
    }
    //void LateUpdate()
    //{
    //    Vector3 camoffset = new Vector3(-offset.x, -offset.y, offset.z);
    //    Matrix4x4 m = Matrix4x4.TRS(camoffset, Quaternion.identity, new Vector3(1, 1, -1));
    //    m.m00 = 1;
    //    m.m01 = 0;
    //    m.m02 = 0;
    //    m.m03 = 0;
    //    m.m10 = 0;
    //    m.m11 = 1;
    //    m.m12 = 0;
    //    m.m13 = 0;
    //    m.m20 = 0;
    //    m.m21 = 0;
    //    m.m22 = 1;
    //    m.m23 = 0;
    //    m.m30 = 0;
    //    m.m31 = 0;
    //    m.m32 = 0;
    //    m.m33 = 0;
    //    camera.worldToCameraMatrix = m * transform.worldToLocalMatrix;
    //}

}
