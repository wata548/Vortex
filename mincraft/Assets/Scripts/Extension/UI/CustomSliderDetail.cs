using Extension.Serialize;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Extension.UI {
    public class CustomSliderDetail: MonoBehaviour {
        [SerializeField] private Image _progressBar;
        [SerializeField] private TMP_Text _progress;
        [SerializeField] private string _progressFormat = "{0}/{1}";
        public RectTransform RectTransform => (transform as RectTransform)!;
        
        
        [Space, Header("ProgressValue")]
        private float _value;
        private float _maxValue;

        public void Set(float max, float value) {
            _value = value;
            _maxValue = max;
            Fill();
        }

        private void Fill() {
            _progressBar.fillAmount = _value / _maxValue;
            
            if(_progress != null)
                _progress.text = string.Format(_progressFormat, _value, _maxValue);
        }
    }
}