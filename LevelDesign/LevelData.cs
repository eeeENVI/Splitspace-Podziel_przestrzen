using Splitspace_Podziel_przestrzen.Core;

namespace Splitspace_Podziel_przestrzen.LevelDesign;
public class LevelData
{
    public Shape2D StartingShape { get; set; }
    public float TargetPercentage { get; set; }
    public float Tolerance { get; set; }
    public int LevelNumber { get; set; }
    public string LevelGoalDescription => $"Podziel na: {TargetPercentage}% / {100 - TargetPercentage}%";
}