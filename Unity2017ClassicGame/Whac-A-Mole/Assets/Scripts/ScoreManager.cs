
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static int score;
    public static void Init()
    {
        score = 0;
    }

    public static void AddScore()
    {
        score++;
    }

    public static int GetScore()
    {
        return score;
    }
}