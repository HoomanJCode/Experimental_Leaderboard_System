using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Singletone
    {
        get
        {
            if (_instance == null)
            {
                // Create a GameObject dynamically for running coroutines
                var runnerObject = new GameObject("CoroutineRunner");
                _instance = runnerObject.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(runnerObject); // Ensure it persists across scenes
            }
            return _instance;
        }
    }
}