#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace CSVData {
    
    [CustomEditor(typeof(CSVLoadManager))]
    public class CSVManagerEditor: Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUILayout.Label("Make csv type class and table");
            GUILayout.Label("(ResultPath: Scripts/AutoCSVOutputScript)");
            if (GUILayout.Button("Generate")) {
                (target as CSVLoadManager)!.Generate();
            }
            
            GUILayout.Label("Sync data from csv data");
            if (GUILayout.Button("SyncData")) {
                (target as CSVLoadManager)!.Load();
            }
        }
    }
}
#endif