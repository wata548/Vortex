using System;
using UnityEngine;

namespace Extension {
    public abstract class MonoSingleton<T> : MonoBehaviour 
        where T: MonoBehaviour {

        private static bool exitMyGame = false;
        public static T Instance { get; private set; }
        /// <summary>
        /// Do you use this single on just one scene?
        /// </summary>
        protected abstract bool IsNarrowSingleton { set; get; }

        private static void EnterMyGame() => exitMyGame = false;
        private static void ExitMyGame() => exitMyGame = true;
        
        protected void Awake() {
            
            if (Instance != null) {

                if (Instance != this as T) {

                    Debug.Log(
                        $"{gameObject.name} was deleted,\n" 
                        + "singleton can exist just one\n"
                        + $"(Current Singleton: {Instance.gameObject})"
                    );
                    Destroy(gameObject);
                }
                       
                return;
            }

            var type = GetType();
            if (type != typeof(T))
                throw new ArgumentException($"T must be {type}");
            Instance = this as T;
            if(!IsNarrowSingleton)
                DontDestroyOnLoad(gameObject);
        }

        protected void Update() {
            if(exitMyGame)
                Destroy(gameObject);
        }
    }
}