using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Splitspace_Podziel_przestrzen.Core;
public static class ShapeFactory
{
    private static Random _rng = new Random();

    public static Shape2D CreateRandomConvexPolygon(Vector2 center, float radius, int vertexCount)
    {
        List<float> angles = new List<float>();
        for (int i = 0; i < vertexCount; i++)
        {
            angles.Add((float)(_rng.NextDouble() * Math.PI * 2));
        }
        angles.Sort();

        List<Vector2> vertices = new List<Vector2>();
        foreach (var angle in angles)
        {
            vertices.Add(new Vector2(
                center.X + (float)Math.Cos(angle) * radius,
                center.Y + (float)Math.Sin(angle) * radius
            ));
        }

        return new Shape2D(vertices, new Color(170, 230, 170));
    }
}