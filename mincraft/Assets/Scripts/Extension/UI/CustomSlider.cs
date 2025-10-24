using Extension.Serialize;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Extension.UI {
    public class CustomSlider: MonoBehaviour {
        [SerializeField] private Image _progressBar;
        [SerializeField] private TMP_Text _progress;
        [SerializeField] private string _progressFormat = "{0}%";
        
        [Space, Header("ProgressValue")]
        [SerializeField, WhenValueChange("Fill")] 
        private float _value;
        
        public float Value {
            get => _value;
            set {
                _value = value;
                Fill();
            }
        }

        private void Fill() {
            _value = Mathf.Clamp01(_value);
            _progressBar.fillAmount = _value;
            
            if(_progress != null)
                _progress.text = string.Format(_progressFormat, (int)(_value * 100f));
        }
    }
}