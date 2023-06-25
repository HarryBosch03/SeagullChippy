using System;
using ShootingRangeGame.Saves;
using ShootingRangeGame.Session;
using UnityEditor;
using UnityEngine;

namespace ShootingRangeGameEditor.Inspectors.Session
{
    [CustomEditor(typeof(GameSession))]
    public class GameSessionEditor : Editor<GameSession>
    {
        private int pointsToAward = 1;
        private int pointsToSet;
        private SaveData overrideSaveData;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Div();

            if (GUILayout.Button("Start Round")) Target.StartRound();
            if (GUILayout.Button("End Round")) Target.EndRound();

            Div();
            
            void intSet(string label, ref int val, Action<int> callback)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(label))
                    {
                        callback(pointsToAward);
                    }
                    val = EditorGUILayout.IntField(val);
                }
            }
            
            intSet("Award Points", ref pointsToAward, GameSession.AwardPoint);
            intSet("Set Points", ref pointsToSet, GameSession.SetScore);

            if (GUILayout.Button("Reset Score"))
            {
                GameSession.ResetScore();
            }

            Div();
            
            
            
            pointsToAward = Mathf.Max(pointsToAward, 1);
        }
    }
}
