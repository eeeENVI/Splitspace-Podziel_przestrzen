using Splitspace_Podziel_przestrzen.Core;
using System.Collections.Generic;
using System.Linq;

namespace Splitspace_Podziel_przestrzen.LevelDesign;
public class LevelData
{
    public Shape2D StartingShape { get; set; }
    public List<float> TargetPercentages { get; set; } = new List<float>(); 
    public float Tolerance { get; set; }
    public int LevelNumber { get; set; }
    public int MaxCuts => TargetPercentages.Count - 1;

    // Dynamiczny opis celu
    public string LevelGoalDescription => 
        $"Podziel na {TargetPercentages.Count} części: " + 
        string.Join("% / ", TargetPercentages) + "%";
}