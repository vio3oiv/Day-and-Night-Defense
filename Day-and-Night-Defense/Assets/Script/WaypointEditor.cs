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
            // 1. 변경 체크 시작
            EditorGUI.BeginChangeCheck();

            // 2. 핸들 드로우
            Vector3 currentWaypointPoint = Waypoint.CurrentPosition + Waypoint.Points[i];
            Vector3 newWaypointPoint = Handles.FreeMoveHandle(
        currentWaypointPoint,            // 위치
        0.7f,                            // 핸들 크기 (size)
        new Vector3(0.3f, 0.3f, 0.3f),   // 스냅 간격 (snap)
        Handles.SphereHandleCap          // 드로잉 함수
    ); ;

            // 3. 인덱스 라벨
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

            // 4. 변경 확인 및 적용
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Free Move Handle");
                Waypoint.Points[i] = newWaypointPoint - Waypoint.CurrentPosition;
            }
        }
    }
}
