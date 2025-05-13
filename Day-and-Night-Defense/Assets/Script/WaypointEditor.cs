using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    Waypoint Waypoint => target as Waypoint;

    private void OnSceneGUI()
    {
        Handles.color = Color.cyan;
        for (int i = 0; i < Waypoint.Points.Length; i++)
        {
            // 1. ���� üũ ����
            EditorGUI.BeginChangeCheck();

            // 2. �ڵ� ��ο�
            Vector3 currentWaypointPoint = Waypoint.CurrentPosition + Waypoint.Points[i];
            Vector3 newWaypointPoint = Handles.FreeMoveHandle(
        currentWaypointPoint,            // ��ġ
        0.7f,                            // �ڵ� ũ�� (size)
        new Vector3(0.3f, 0.3f, 0.3f),   // ���� ���� (snap)
        Handles.SphereHandleCap          // ����� �Լ�
    ); ;

            // 3. �ε��� ��
            GUIStyle textStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 16,
                normal = { textColor = Color.white }
            };
            Vector3 textAlignment = Vector3.down * 0.35f + Vector3.right * 0.35f;
            Handles.Label(
                currentWaypointPoint + textAlignment,
                $"{i + 1}",
                textStyle
            );

            // 4. ���� Ȯ�� �� ����
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Free Move Handle");
                Waypoint.Points[i] = newWaypointPoint - Waypoint.CurrentPosition;
            }
        }
    }
}
