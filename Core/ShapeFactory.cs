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

    public static Shape2D CreateStarPolygon(Vector2 center, float radius, int points)
    {
        List<Vector2> vertices = new List<Vector2>();
        float angleStep = MathHelper.TwoPi / (points * 2);
        for (int i = 0; i < points * 2; i++)
        {
            float r = (i % 2 == 0) ? radius : radius * 0.5f;
            float angle = i * angleStep;
            vertices.Add(new Vector2(center.X + (float)Math.Cos(angle) * r, center.Y + (float)Math.Sin(angle) * r));
        }
        return new Shape2D(vertices, new Color(170, 230, 170));
    }

    public static Shape2D CreateAdvancedStar(Vector2 center, float radius, int points, float innerRadiusRatio)
    {
        List<Vector2> vertices = new List<Vector2>();
        float angleStep = MathHelper.TwoPi / (points * 2);
        
        for (int i = 0; i < points * 2; i++)
        {
            float r = (i % 2 == 0) ? radius : radius * innerRadiusRatio;
            float angle = i * angleStep;
            vertices.Add(new Vector2(center.X + (float)Math.Cos(angle) * r, center.Y + (float)Math.Sin(angle) * r));
        }
        return new Shape2D(vertices, new Color(159, 227, 255));
    }

    public static Shape2D CreateIrregularShape(Vector2 center, float baseRadius, int numVertices)
    {
        List<Vector2> vertices = new List<Vector2>();
        Random rnd = new Random();
        float angleStep = MathHelper.TwoPi / numVertices;

        for (int i = 0; i < numVertices; i++)
        {
            float noise = (float)(rnd.NextDouble() * 0.6 + 0.4); 
            float r = baseRadius * noise;
            float angle = i * angleStep;

            vertices.Add(new Vector2(
                center.X + (float)Math.Cos(angle) * r,
                center.Y + (float)Math.Sin(angle) * r
            ));
        }
        return new Shape2D(vertices, new Color(170, 230, 170));
    }

    public static Shape2D CreateLShape(Vector2 center, float size)
    {
        float h = size / 2;
        float q = size / 4; // Szerokość ramienia

        List<Vector2> vertices = new List<Vector2>
        {
            center + new Vector2(-h, -h), // Góra lewo
            center + new Vector2(-h + q, -h), 
            center + new Vector2(-h + q, h - q), // Wewnętrzny róg
            center + new Vector2(h, h - q),
            center + new Vector2(h, h),
            center + new Vector2(-h, h) // Dół lewo
        };

        return new Shape2D(vertices, new Color(200, 150, 255));
    }

    public static Shape2D CreateMutantShape(Vector2 center, float maxRadius)
    {
        Random rnd = new Random();
        int points = rnd.Next(6, 12); 
        List<Vector2> vertices = new List<Vector2>();
        
        float currentAngle = 0;
        for (int i = 0; i < points; i++)
        {
            // Losowy krok kąta 
            currentAngle += (MathHelper.TwoPi / points) * (float)(rnd.NextDouble() * 1.5 + 0.5);
            
            // Losowy promień 
            float r = maxRadius * (float)(rnd.NextDouble() * 0.8 + 0.2);
            
            vertices.Add(new Vector2(
                center.X + (float)Math.Cos(currentAngle) * r,
                center.Y + (float)Math.Sin(currentAngle) * r
            ));
        }
        return new Shape2D(vertices, Color.MediumPurple);
    }

    public static Shape2D CreateCrazyBlockShape(Vector2 center, float size)
    {
        float h = size / 2;
        float q = size / 2.5f; 
        Random rnd = new Random();
        
        List<Vector2> vertices = new List<Vector2>();
        
        int type = rnd.Next(0, 3);
        
        if (type == 0) 
        {
            vertices.Add(new Vector2(-h, -h)); 
            vertices.Add(new Vector2(h, -h));  
            vertices.Add(new Vector2(h, -h + q)); 
            vertices.Add(new Vector2(-h + q, -h + q)); 
            vertices.Add(new Vector2(-h + q, h - q));  
            vertices.Add(new Vector2(h, h - q)); 
            vertices.Add(new Vector2(h, h));  
            vertices.Add(new Vector2(-h, h));  
        }
        else if (type == 1) 
        {
            vertices.Add(new Vector2(-q, -h));
            vertices.Add(new Vector2(q, -h));
            vertices.Add(new Vector2(q, -q));
            vertices.Add(new Vector2(h, -q));
            vertices.Add(new Vector2(h, q));
            vertices.Add(new Vector2(-h, q));
            vertices.Add(new Vector2(-h, -q));
            vertices.Add(new Vector2(-q, -q));
        }
        else 
        {
            vertices.Add(new Vector2(-h, -h));
            vertices.Add(new Vector2(h, -h));
            vertices.Add(new Vector2(h, h));
            vertices.Add(new Vector2(0, h));
            vertices.Add(new Vector2(0, 0));
            vertices.Add(new Vector2(h-q, 0));
            vertices.Add(new Vector2(h-q, h-q));
            vertices.Add(new Vector2(-h, h-q));
        }

        for (int i = 0; i < vertices.Count; i++) vertices[i] += center;

        return new Shape2D(vertices, new Color(rnd.Next(100, 255), rnd.Next(100, 255), 255));
    }
}
