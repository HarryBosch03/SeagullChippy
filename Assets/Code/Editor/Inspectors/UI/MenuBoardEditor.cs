using ShootingRangeGame.UI;
using UnityEditor;

namespace ShootingRangeGameEditor.Inspectors.UI
{
    [CustomEditor(typeof(MenuBoard))]
    public class MenuBoardEditor : Editor<MenuBoard>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Div();
            
        }
    }
}