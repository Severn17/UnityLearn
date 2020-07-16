
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    private static int score;
    private static UnityAction<int> _action;

    public static void AddScore()
    {
        score++;
        _action(score);
    }

    public static int GetScore()
    {
        return score;
    }

    public static void AddListener(UnityAction<int> action)
    {
        _action = action;
        _action(score);
    }

    public static void Restart()
    {
        score = 0;
        _action(score);
    }
}