#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using CSVData.Extensions;
using UnityEditor;
using Color = System.Drawing.Color;

namespace CSVData {
    
    [CreateAssetMenu(menuName = "CSV/Manager")]
    public class CSVLoadManager: ScriptableObject {

        [Header("Result table path")]
        [SerializeField] private string _DirectoryName;
        [SerializeField] private List<LoaderBase> _loaders;

        public void Load() {
            foreach (var loader in _loaders) {
                loader.Load(_DirectoryName);
            }

            Debug.Log("Complete csv load"
                .SetColor(Color.Red)
                .SetFontSizeByPercent(1.5f)
            );
        }

        public void Generate() {
            foreach (var loader in _loaders) {
                loader.Generate();
            }
            
            AssetDatabase.Refresh();
            Debug.Log("Complete csv type generate"
                .SetColor(Color.Red)
                .SetFontSizeByPercent(1.5f)
            );
        }
    }
}

#endif