using System;
using ShootingRangeGame.UI;
using UnityEditor;
using UnityEngine;

namespace ShootingRangeGameEditor.Inspectors.UI
{
    [CustomEditor(typeof(MenuBoard))]
    public class MenuBoardEditor : Editor<MenuBoard>
    {
        private int switchGroup;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Div();

            using (new EditorGUILayout.HorizontalScope())
            {
                switchGroup = EditorGUILayout.IntField(switchGroup);
                if (GUILayout.Button("Switch Menu"))
                {
                    SwitchMenu(i => switchGroup == i);
                }
            }

            if (GUILayout.Button("Show All Menus"))
            {
                SwitchMenu(_ => true);
            }
            
            if (GUILayout.Button("Hide All Menus"))
            {
                SwitchMenu(_ => false);
            }
        }

        public void SwitchMenu(Func<int, bool> predicate)
        {
            var groups = Target.GetComponentsInChildren<CanvasGroup>(true);
            for (var i = 0; i < groups.Length; i++)
            {
                var group = groups[i];
                var s = predicate(i);
                
                group.gameObject.SetActive(true);
                
                group.alpha = s ? 1.0f : 0.0f;
                group.interactable = s;
                group.blocksRaycasts = s;
            }
        }
    }
}