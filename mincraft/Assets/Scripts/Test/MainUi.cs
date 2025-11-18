using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Test {
    public class MainUi : MonoBehaviour {
        [SerializeField] private GameObject _tutorial;
        [SerializeField] private Image _tutorialcontext;

        public void Tutorial() {
            _tutorial.SetActive(true);
        }

        public void StartGame() {
            SceneManager.LoadScene("Game");
        }
        
        public void Quit() {
            Application.Quit();
        }

        public void CloseTutorial() {
            _tutorial.SetActive(false);
        }

        public void ShowTutorial(Sprite pSprite) {
            _tutorialcontext.sprite = pSprite;
        }
    }
}