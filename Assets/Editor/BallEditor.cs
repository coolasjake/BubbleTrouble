using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ball))]
public class BallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Ball ball = (Ball)target;

        GUILayout.Space(2);

        int size = ball.size;
        ball.size = (int)GUILayout.HorizontalSlider(ball.size, 1, 6);
        if (size != ball.size)
            ball.InspectorUpdateSize();

        GUILayout.Space(18);
        if (GUILayout.Button("Save"))
        {
            Undo.RecordObject(ball.gameObject, "Ball Size");

            ball.LC.balls.Remove(ball);
            ball.LC.balls.Add(ball);

            EditorUtility.SetDirty(ball);
            PrefabUtility.RecordPrefabInstancePropertyModifications(ball);
        }
        if (GUILayout.Button("Copy"))
        {
            Undo.RecordObject(ball.gameObject, "Ball Duplicated");

            ball.LC.balls.Add(Instantiate(ball.gameObject, ball.transform.position + new Vector3(1, 0, 0), ball.transform.rotation, ball.LC.transform).GetComponent<Ball>());

            EditorUtility.SetDirty(ball);
            PrefabUtility.RecordPrefabInstancePropertyModifications(ball);
        }

        if (ball.deleteConfirmation)
        {
            if (GUILayout.Button("Yes Delete It"))
            {
                Undo.RecordObject(ball.gameObject, "Ball Deleted");

                LevelController LC = ball.LC;
                ball.LC.balls.Remove(ball);
                DestroyImmediate(ball.gameObject);

                PrefabUtility.RecordPrefabInstancePropertyModifications(LC);
            }
            else if (GUILayout.Button("Actually No Keep it"))
                ball.deleteConfirmation = false;
        }
        else
        {
            if (GUILayout.Button("Delete"))
                ball.deleteConfirmation = true;
        }
    }
}
