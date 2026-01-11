using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splitspace_Podziel_przestrzen.Core;

namespace Splitspace_Podziel_przestrzen.LevelDesign;
public static class LevelFactory
{
    private static Random _rng = new Random();

    public static LevelData GenerateNextLevel(int levelNumber)
    {
        // 1. Obliczamy stopień skomplikowania
        int minVertices = 3 + (levelNumber / 3);
        int maxVertices = 5 + (levelNumber / 2);
        int vertexCount = _rng.Next(minVertices, maxVertices + 1);

        // 2. Generujemy kształt
        Shape2D shape;
        Vector2 center = new Vector2(640, 512); 
        float radius = 300f;

        if (levelNumber % 5 == 0 && levelNumber > 0)
        {
            shape = CreateStarPolygon(center, radius, vertexCount);
        }
        else
        {
            shape = ShapeFactory.CreateRandomConvexPolygon(center, radius, vertexCount);
        }

        // 3. Ustalamy cel i tolerancję
        float target = _rng.Next(20, 51); 
        float tolerance = Math.Max(0.5f, 5.0f - (levelNumber * 0.3f));

        return new LevelData
        {
            StartingShape = shape,
            TargetPercentage = target,
            Tolerance = tolerance,
            LevelNumber = levelNumber
        };
    }

    // testowe by sprawdzic czy dziala clampowanie po podziale - do przeniesienia i zmiany do Shape2D
    private static Shape2D CreateStarPolygon(Vector2 center, float radius, int points)
    {
        List<Vector2> vertices = new List<Vector2>();
        float angleStep = MathHelper.TwoPi / (points * 2);

        for (int i = 0; i < points * 2; i++)
        {
            float r = (i % 2 == 0) ? radius : radius * 0.5f;
            float angle = i * angleStep;

            vertices.Add(new Vector2(
                center.X + (float)Math.Cos(angle) * r,
                center.Y + (float)Math.Sin(angle) * r
            ));
        }
        return new Shape2D(vertices, new Color(159, 227, 255));
    }
}