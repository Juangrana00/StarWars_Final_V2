using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FOV))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FOV fov = (FOV)target;

        Vector3 origin = fov.transform.position;
        float radiusDistance = fov.viewRadius;
        float halfAngleRad = Mathf.Deg2Rad * (fov.viewAngle / 2);
        float finalRingRadius = Mathf.Tan(halfAngleRad) * radiusDistance;
        Quaternion orientationRot = Quaternion.LookRotation(fov.transform.forward, fov.transform.up);
        Vector3[] ringVertices = new Vector3[fov.segments];

        for (int i = 0; i < fov.segments; i++)
        {
            float segmentOverSegments = (float) i / fov.segments;
            float plusPiPlusTwo = segmentOverSegments * Mathf.PI * 2;
            Vector3 localVertice = Vector3.forward * radiusDistance + (Mathf.Cos(plusPiPlusTwo) * Vector3.right + Mathf.Sin(plusPiPlusTwo) * Vector3.up) * finalRingRadius;
            ringVertices[i] = origin + orientationRot * localVertice;
        }

        Handles.color = Color.white;
        for (int i = 0; i < fov.segments; i++)
        {
            Vector3 point = ringVertices[i];
            Vector3 nextPoint = ringVertices[(i + 1) % fov.segments];
            Handles.DrawLine(point, nextPoint);
        }

        Handles.color = Color.white;
        for (int i = 0; i < fov.segments; i++) 
        {
            Handles.DrawLine(origin, ringVertices[i]);
        }

        Handles.color = Color.gray;
        Handles.DrawWireDisc(origin, fov.transform.forward, 0.0f);

        Handles.color = Color.white;
        Handles.DrawWireArc(origin, fov.transform.up, fov.transform.forward, 360, fov.viewRadius);

        /*Handles.color = Color.red;
        foreach(Transform visibleTarget in fov.fov.visibleTargets)
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
            Handles.SphereHandleCap(0, visibleTarget.position, Quaternion.identity, 0.1f, EventType.Repaint);
        }*/
    }
}
