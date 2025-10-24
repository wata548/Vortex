using System.Collections;
using UnityEngine;

namespace Extension.Scene {
    public static class SceneManager {

		private static string sceneNameFormat = "{0}";
        public static float Progress => _process?.progress ?? 0f;
        private static AsyncOperation _process;
        public static Scene Destination;

        public static void LoadScene(Scene target) {

            Destination = target;
			var sceneName = string.Format(sceneNameFormat, nameof(Scene.Loading));
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public static IEnumerator LoadScene() {

            yield return null;
            
			var sceneName = string.Format(sceneNameFormat, Destination.ToString());
            _process = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            _process!.allowSceneActivation = false;

            while (!_process!.isDone) {

                if (_process.progress >= 0.9f)
                    _process.allowSceneActivation = true;

                yield return null;
            }
        }
    }
}