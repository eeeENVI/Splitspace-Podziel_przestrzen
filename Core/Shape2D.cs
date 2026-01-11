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

    /*
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

        // Zwracamy dwie nowe części 
        return new List<Shape2D> 
        { 
            new Shape2D(SortVertices(leftSide), Color), 
            new Shape2D(SortVertices(rightSide), Color) 
        };
    }*/

    // Nowy split do wkleslych - Sutherland-Hodgman
    public List<Shape2D> Split(Vector2 p1, Vector2 p2)
    {
        List<Vector2> part1 = new List<Vector2>();
        List<Vector2> part2 = new List<Vector2>();

        for (int i = 0; i < Vertices.Count; i++)
        {
            Vector2 current = Vertices[i];
            Vector2 next = Vertices[(i + 1) % Vertices.Count];

            float sideCurrent = GetSide(p1, p2, current);
            float sideNext = GetSide(p1, p2, next);

            if (sideCurrent >= 0) part1.Add(current);
            if (sideCurrent <= 0) part2.Add(current);

            // Sprawdzenie przecięcia krawędzi z linią cięcia
            if (sideCurrent * sideNext < 0)
            {
                Vector2 intersection = GetIntersection(p1, p2, current, next);
                part1.Add(intersection);
                part2.Add(intersection);
            }
        }

        // nie ma trojkatow, nie ma ciecia
        if (part1.Count < 3 || part2.Count < 3)
            return new List<Shape2D> { this };

        return new List<Shape2D> 
        { 
            new Shape2D(part1, Color), 
            new Shape2D(part2, Color) 
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
        // NAPEWNO dodac sprawdzenie czy wymair sie nie zapada (det=0)
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

    public List<Vector2[]> GetTriangles()
    {
        List<Vector2[]> triangles = new List<Vector2[]>();
        List<Vector2> verts = new List<Vector2>(Vertices);

        // Algorytm Ear Clipping 
        while (verts.Count >= 3)
        {
            bool earFound = false;
            for (int i = 0; i < verts.Count; i++)
            {
                int prev = (i == 0) ? verts.Count - 1 : i - 1;
                int next = (i == verts.Count - 1) ? 0 : i + 1;

                if (IsEar(verts[prev], verts[i], verts[next], verts))
                {
                    triangles.Add(new Vector2[] { verts[prev], verts[i], verts[next] });
                    verts.RemoveAt(i);
                    earFound = true;
                    break;
                }
            }
            if (!earFound) break; // infinity loop break
        }
        return triangles;
    }

    // Czy moge uciac uszko wielokącikowi :3
    private bool IsEar(Vector2 a, Vector2 b, Vector2 c, List<Vector2> allVerts)
    {
        // Czy wypukły
        if (GetSide(a, c, b) >= 0) return false;

        // Czy jakikolwiek inny punkt leży wewnątrz tego trójkąta?
        for (int i = 0; i < allVerts.Count; i++)
        {
            Vector2 p = allVerts[i];
            if (p == a || p == b || p == c) continue;
            if (IsPointInTriangle(p, a, b, c)) return false;
        }
        return true;
    }

    private bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = GetSide(a, b, p);
        float d2 = GetSide(b, c, p);
        float d3 = GetSide(c, a, p);
        return (d1 < 0 && d2 < 0 && d3 < 0) || (d1 > 0 && d2 > 0 && d3 > 0);
    }

}