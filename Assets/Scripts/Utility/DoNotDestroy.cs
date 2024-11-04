using UnityEngine;

/// <summary>
/// Prevents the GameObject this script is attached to from being destroyed when loading a new scene
/// </summary>
public class DoNotDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
