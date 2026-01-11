using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Splitspace_Podziel_przestrzen.Core;

namespace Splitspace_Podziel_przestrzen.LevelDesign;
public static class LevelFactory
{
    private static Random _rng = new Random();

    public static LevelData GenerateNextLevel(int levelNumber, Vector2 center)
    {
        // 1. Określamy liczbę fragmentów (N)
        int fragmentCount = 2 + (levelNumber / 25); 
        
        // 2. Obliczamy stopień skomplikowania wierzchołków
        int minVertices = 3 + (levelNumber / 10);
        int maxVertices = 6 + (levelNumber / 5);
        int vertexCount = _rng.Next(minVertices, maxVertices + 1);

        // 3. Generujemy kształt (na środku ekranu)
        Shape2D shape;
        float radius = 300f;

       Random rnd = new Random();

        if (levelNumber <= 5)
        {
            // Poziomy 1-5: Tylko figury wypukłe (nauka podstaw)
            shape = ShapeFactory.CreateRandomConvexPolygon(center, radius, vertexCount);
        }
        else
        {
            // Poziom 6+: Losowanie z całej puli
            int roll = rnd.Next(100); 

            if (roll < 5)
            {
                shape = ShapeFactory.CreateMutantShape(center, 180);
            }
            else if (roll < 15)
            {
                shape = ShapeFactory.CreateCrazyBlockShape(center, 250); 
            }
            else if (roll < 25) 
            {
                shape = ShapeFactory.CreateLShape(center, radius * 1.5f);
            }
            else if (roll < 50) // ziemniak hehe
            {
                shape = ShapeFactory.CreateIrregularShape(center, radius, vertexCount);
            }
            else if (roll < 75) 
            {
                float innerRatio = (float)(rnd.NextDouble() * 0.4 + 0.3); 
                shape = ShapeFactory.CreateAdvancedStar(center, radius, rnd.Next(5, 9), innerRatio);
            }
            else 
            {
                shape = ShapeFactory.CreateRandomConvexPolygon(center, radius, vertexCount);
            }
        }

        // 4. Generujemy N celów, których suma to 100
        List<float> targets = GenerateRandomTargets(fragmentCount);

        // 5. Tolerancja maleje z czasem
        float tolerance = Math.Max(0.3f, 10.0f - (levelNumber * 0.1f));

        return new LevelData
        {
            StartingShape = shape,
            TargetPercentages = targets,
            Tolerance = tolerance,
            LevelNumber = levelNumber
        };
    }

    private static List<float> GenerateRandomTargets(int n)
    {
        if (n <= 1) return new List<float> { 100f };

        // Losujemy (n-1) punktów cięcia na skali 0-100
        List<int> cuts = new List<int>();
        for (int i = 0; i < n - 1; i++)
        {
            cuts.Add(_rng.Next(10, 91)); // kawałki nie mniejsze niż 10%
        }
        cuts.Sort();

        List<float> targets = new List<float>();
        int lastCut = 0;

        foreach (var cut in cuts)
        {
            targets.Add(cut - lastCut);
            lastCut = cut;
        }
        targets.Add(100 - lastCut);

        // Zaokrąglamy do pełnych liczb dla czytelności
        return targets.Select(t => (float)Math.Round(t)).ToList();
    }

    
}