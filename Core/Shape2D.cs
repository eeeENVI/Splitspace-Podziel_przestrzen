using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Splitspace_Podziel_przestrzen.Core;
public class Shape2D
{
    public List<Vector2> Vertices { get; private set; }
    public Color Color { get; set; }

    public Shape2D(List<Vector2> vertices, Color? color = null)
    {
        Vertices = vertices;
        Color = color ?? new Color(170, 230, 170);
    }

    // metoda Shoelace
    public float CalculateArea()
    {
        float area = 0;
        for (int i = 0; i < Vertices.Count; i++)
        {
            Vector2 v1 = Vertices[i];
            Vector2 v2 = Vertices[(i + 1) % Vertices.Count];
            area += (v1.X * v2.Y) - (v2.X * v1.Y);
        }
        return Math.Abs(area) / 2.0f;
    }

    // Główna logika cięcia linią zdefiniowaną przez dwa punkty.
    public List<Shape2D> Split(Vector2 p1, Vector2 p2)
    {
        List<Vector2> leftSide = new List<Vector2>();
        List<Vector2> rightSide = new List<Vector2>();

        for (int i = 0; i < Vertices.Count; i++)
        {
            Vector2 current = Vertices[i];
            Vector2 next = Vertices[(i + 1) % Vertices.Count];

            float sideCurrent = GetSide(p1, p2, current);
            float sideNext = GetSide(p1, p2, next);

            if (sideCurrent >= 0) leftSide.Add(current);
            if (sideCurrent <= 0) rightSide.Add(current);

            // Sprawdzenie przecięcia krawędzi z linią cięcia
            if (sideCurrent * sideNext < 0)
            {
                Vector2 intersection = GetIntersection(p1, p2, current, next);
                leftSide.Add(intersection);
                rightSide.Add(intersection);
            }
        }

        // Jeśli nie powstały przynajmniej dwa trójkąty, cięcie nie zaszło
        if (leftSide.Count < 3 || rightSide.Count < 3)
            return new List<Shape2D> { this };

        // Zwracamy dwie nowe części (później dodamy tu obsługę wklęsłości)
        return new List<Shape2D> 
        { 
            new Shape2D(SortVertices(leftSide), Color), 
            new Shape2D(SortVertices(rightSide), Color) 
        };
    }

    private float GetSide(Vector2 p1, Vector2 p2, Vector2 v)
    {
        return (p2.X - p1.X) * (v.Y - p1.Y) - (p2.Y - p1.Y) * (v.X - p1.X);
    }

    private Vector2 GetIntersection(Vector2 l1, Vector2 l2, Vector2 a, Vector2 b)
    {
        float a1 = l2.Y - l1.Y;
        float b1 = l1.X - l2.X;
        float c1 = a1 * l1.X + b1 * l1.Y;

        float a2 = b.Y - a.Y;
        float b2 = a.X - b.X;
        float c2 = a2 * a.X + b2 * a.Y;

        float det = a1 * b2 - a2 * b1;
        // W prawdziwym projekcie warto sprawdzić czy det != 0 (linie równoległe)
        return new Vector2((b2 * c1 - b1 * c2) / det, (a1 * c2 - a2 * c1) / det);
    }

    // Pomocnicze sortowanie wierzchołków, by wielokąt się nię gniótł
    private List<Vector2> SortVertices(List<Vector2> points)
    {
        Vector2 center = new Vector2(points.Average(p => p.X), points.Average(p => p.Y));
        return points.OrderBy(p => Math.Atan2(p.Y - center.Y, p.X - center.X)).ToList();
    }

    // Środek figury
    public Vector2 GetCentroid()
    {
        float x = Vertices.Average(v => v.X);
        float y = Vertices.Average(v => v.Y);
        return new Vector2(x, y);
    }
}