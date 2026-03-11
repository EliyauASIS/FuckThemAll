using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager instance;
    public TMP_Text pointsText;
    public int points = 0;
    public int bestPersonalPoints = 0;
    private void Awake()
    {
        instance = this;
        bestPersonalPoints = PlayerPrefs.GetInt("BestScore", 0);
    }
    public void UpdatePoints(int value)
    {
        points += value;
        ShowPointsUI(points);
    }
    public void ResetPoints()
    {
        points = 0;
        ShowPointsUI(points);
    }
    void ShowPointsUI(int pointsValue)
    {
        pointsText.text = pointsValue.ToString();
    }
    public int GetPoints() => points;
    public int GetBestPersonalPoints() => bestPersonalPoints;

    public void UpdateBestPoints()
    {
        bestPersonalPoints = points;
        PlayerPrefs.SetInt("BestScore", bestPersonalPoints);
    }
}
