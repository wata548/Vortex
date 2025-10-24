using System;
using System.Collections.Generic;
using Extension.Scene;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Extension.UI {
    public class LoadingScene: MonoBehaviour {

        [SerializeField] private string _tipFormat = "Tip: {0}";
        [SerializeField] private TMP_Text _tipBox;
        [SerializeField] private TMP_Text _progressShower;
        [SerializeField] private CustomSlider _slider;
        [SerializeField] private List<string> _tips;

        private void Awake() {
            if(_tipBox != null)
                _tipBox.text = string.Format(_tipFormat, _tips[Random.Range(0, _tips.Count)]);
            StartCoroutine(SceneManager.LoadScene());
        }

        private void Update() {

            var progress = SceneManager.Progress;

            _slider.Value = progress;
            if (_progressShower != null)
                _progressShower.text = $"{progress:N0}%";
        }
    }
}