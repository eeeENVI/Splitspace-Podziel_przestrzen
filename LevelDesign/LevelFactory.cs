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
        // Poziomy 0-2: N=2, Poziomy 3-5: N=3, itd.
        int fragmentCount = 2 + (levelNumber / 20); 
        
        // 2. Obliczamy stopień skomplikowania wierzchołków
        int minVertices = 3 + (levelNumber / 2);
        int maxVertices = 6 + levelNumber;
        int vertexCount = _rng.Next(minVertices, maxVertices + 1);

        // 3. Generujemy kształt (na środku ekranu)
        Shape2D shape;
        float radius = 300f;

        if (levelNumber > 0 && levelNumber % 3 == 0) // Częściej dajemy gwiazdy dla N > 2
        {
            shape = CreateStarPolygon(center, radius, vertexCount);
        }
        else
        {
            shape = ShapeFactory.CreateRandomConvexPolygon(center, radius, vertexCount);
        }

        // 4. Generujemy N celów, których suma to 100
        List<float> targets = GenerateRandomTargets(fragmentCount);

        // 5. Tolerancja maleje z czasem
        float tolerance = Math.Max(0.3f, 5.0f - (levelNumber * 0.1f));

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

    private static Shape2D CreateStarPolygon(Vector2 center, float radius, int points)
    {
        List<Vector2> vertices = new List<Vector2>();
        float angleStep = MathHelper.TwoPi / (points * 2);
        for (int i = 0; i < points * 2; i++)
        {
            float r = (i % 2 == 0) ? radius : radius * 0.5f;
            float angle = i * angleStep;
            vertices.Add(new Vector2(center.X + (float)Math.Cos(angle) * r, center.Y + (float)Math.Sin(angle) * r));
        }
        return new Shape2D(vertices, new Color(159, 227, 255));
    }
}