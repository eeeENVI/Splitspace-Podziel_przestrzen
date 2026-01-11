using System;
using Splitspace_Podziel_przestrzen.Core;

namespace Splitspace_Podziel_przestrzen.LevelDesign;
public class ScoreResult
{
    public bool IsPassed { get; set; }
    public float Accuracy { get; set; } // 0 - 100%
    public float Difference { get; set; } // O ile % się pomylił
    public int Points { get; set; }
}