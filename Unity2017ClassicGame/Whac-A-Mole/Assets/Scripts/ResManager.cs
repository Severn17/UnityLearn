using UnityEngine;

public class ResManager : MonoBehaviour
{
    public static void LoadPrefab(string path)
    {
        Resources.Load<GameObject>(path);
    }
}