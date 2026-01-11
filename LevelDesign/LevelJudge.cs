using System;
using System.Collections.Generic;
using System.Linq;
using Splitspace_Podziel_przestrzen.Core;


namespace Splitspace_Podziel_przestrzen.LevelDesign;
public static class LevelJudge
{
    public static ScoreResult EvaluateSplit(List<float> targetPercentages, List<Shape2D> shapes, float totalArea, float tolerance)
    {
        // 1. Sprawdzamy, czy liczba fragmentów na ekranie zgadza się z liczbą celów (N)
        if (shapes.Count != targetPercentages.Count)
        {
            return new ScoreResult { IsPassed = false, Accuracy = 0 };
        }

        // 2. Obliczamy procenty wszystkich aktualnych kawałków i sortujemy malejąco
        List<float> actualPercs = shapes
            .Select(s => (s.CalculateArea() / totalArea) * 100f)
            .OrderByDescending(p => p)
            .ToList();

        // 3. Sortujemy cele malejąco, aby móc je porównać "jeden do jednego"
        List<float> sortedTargets = targetPercentages
            .OrderByDescending(t => t)
            .ToList();

        float totalError = 0;
        bool allMatched = true;

        // 4. Porównujemy pary: największy kawałek z największym celem, itd.
        for (int i = 0; i < sortedTargets.Count; i++)
        {
            float diff = Math.Abs(actualPercs[i] - sortedTargets[i]);
            totalError += diff;

            if (diff > tolerance)
            {
                allMatched = false;
            }
        }

        // Średni błąd na fragment
        float averageDiff = totalError / sortedTargets.Count;
        
        return new ScoreResult
        {
            IsPassed = allMatched,
            Accuracy = 100f - averageDiff,
            Difference = averageDiff,
            // Punkty przyznawane tylko gdy IsPassed jest true
            Points = allMatched ? (int)Math.Max(0, 100 - (totalError * 5)) : 0
        };
    }
}