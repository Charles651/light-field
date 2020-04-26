using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DrawFrustumRefined : MonoBehaviour {

    private Camera Cam = null;
    public bool ShowCamGizmo = true;

    void Awake()
    {
        Cam = GetComponent<Camera>();
    }

    void OnDrawGizmos()
    {
        if (!ShowCamGizmo)
            return;
        Vector2 v = GetGameViewSize();
        float GameAspect = v.x / v.y;
        float FinalAspect = GameAspect / Cam.aspect;

        Matrix4x4 LocalToWorld = transform.localToWorldMatrix;
        Matrix4x4 ScaleMatrix = Matrix4x4.Scale(new Vector3(Cam.aspect * (Cam.rect.width / Cam.rect.height), FinalAspect, 1));
        Gizmos.matrix = LocalToWorld * ScaleMatrix;
        Gizmos.DrawFrustum(transform.position, Cam.fieldOfView, Cam.nearClipPlane, Cam.farClipPlane, FinalAspect);
        Gizmos.matrix = Matrix4x4.identity;
    }

    public static Vector2 GetGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView, UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (Vector2)GetSizeOfMainGameView.Invoke(null, null);
    }
}
