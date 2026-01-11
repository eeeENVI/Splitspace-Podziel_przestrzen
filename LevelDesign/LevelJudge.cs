using System;
using System.Collections.Generic;
using System.Linq;
using Splitspace_Podziel_przestrzen.Core;


namespace Splitspace_Podziel_przestrzen.LevelDesign;
public static class LevelJudge
{
    public static ScoreResult EvaluateSplit(float targetPerc, List<Shape2D> shapes, float totalArea, float tolerance)
    {
        if (shapes.Count < 2) return new ScoreResult { IsPassed = false };

        // Szukamy kawałka, który jest najbliżej celu - narazie testowo 
        float bestDiff = float.MaxValue;
        float actualPerc = 0;

        foreach (var shape in shapes)
        {
            float currentPerc = (shape.CalculateArea() / totalArea) * 100f;
            float diff = Math.Abs(targetPerc - currentPerc);
            
            if (diff < bestDiff)
            {
                bestDiff = diff;
                actualPerc = currentPerc;
            }
        }

        bool passed = bestDiff <= tolerance;
        
        // bazowo 100 pkt, odejmujemy punkty za błąd
        int points = passed ? (int)Math.Max(0, 100 - (bestDiff * 10)) : 0;

        return new ScoreResult
        {
            IsPassed = passed,
            Accuracy = 100f - bestDiff,
            Difference = bestDiff,
            Points = points
        };
    }
}