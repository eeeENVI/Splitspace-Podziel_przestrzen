using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Splitspace_Podziel_przestrzen.Core
{
    public class Shape2D
    {
        public List<Vector2[]> Triangles { get; private set; }
        public Color Color { get; set; }

        public Shape2D(List<Vector2> vertices, Color? color = null)
        {
            Color = color ?? new Color(170, 230, 170);
            Triangles = Triangulate(new List<Vector2>(vertices));
        }

        public Shape2D(List<Vector2[]> triangles, Color color)
        {
            Triangles = triangles;
            Color = color;
        }

        public float CalculateArea()
        {
            return Triangles.Sum(tri => 
                Math.Abs((tri[0].X * (tri[1].Y - tri[2].Y) + 
                          tri[1].X * (tri[2].Y - tri[0].Y) + 
                          tri[2].X * (tri[0].Y - tri[1].Y)) / 2f));
        }

        public Vector2 GetCentroid()
        {
            float totalArea = 0;
            Vector2 weightedSum = Vector2.Zero;

            foreach (var tri in Triangles)
            {
                float area = Math.Abs((tri[0].X * (tri[1].Y - tri[2].Y) + 
                                       tri[1].X * (tri[2].Y - tri[0].Y) + 
                                       tri[2].X * (tri[0].Y - tri[1].Y)) / 2f);
                Vector2 center = (tri[0] + tri[1] + tri[2]) / 3f;
                weightedSum += center * area;
                totalArea += area;
            }

            return totalArea > 0 ? weightedSum / totalArea : Vector2.Zero;
        }

        public List<Shape2D> Split(Vector2 p1, Vector2 p2)
        {
            List<Vector2[]> leftSideTriangles = new List<Vector2[]>();
            List<Vector2[]> rightSideTriangles = new List<Vector2[]>();

            foreach (var tri in Triangles)
            {
                var (left, right) = SplitSingleTriangle(tri, p1, p2);
                leftSideTriangles.AddRange(left);
                rightSideTriangles.AddRange(right);
            }

            if (leftSideTriangles.Count == 0 || rightSideTriangles.Count == 0)
                return new List<Shape2D> { this };

            return new List<Shape2D> 
            { 
                new Shape2D(leftSideTriangles, Color), 
                new Shape2D(rightSideTriangles, Color) 
            };
        }

        // UUUFF wersja 13.37
        private (List<Vector2[]> left, List<Vector2[]> right) SplitSingleTriangle(Vector2[] tri, Vector2 p1, Vector2 p2)
        {
            var left = new List<Vector2[]>();
            var right = new List<Vector2[]>();

            // leczenie mikro guzow
            Vector2 v0 = Snap(tri[0]);
            Vector2 v1 = Snap(tri[1]);
            Vector2 v2 = Snap(tri[2]);

            float d0 = GetSide(p1, p2, v0);
            float d1 = GetSide(p1, p2, v1);
            float d2 = GetSide(p1, p2, v2);

            // EPSILON - tolerancja bledu
            float eps = 0.01f; 

            if (d0 >= -eps && d1 >= -eps && d2 >= -eps) { left.Add(new[] { v0, v1, v2 }); return (left, right); }
            if (d0 <= eps && d1 <= eps && d2 <= eps) { right.Add(new[] { v0, v1, v2 }); return (left, right); }

            // Sortowanie dla samotnego wierzchołka
            Vector2[] v;
            if ((d0 > eps && d1 <= eps && d2 <= eps) || (d0 < -eps && d1 >= -eps && d2 >= -eps)) v = new[] { v0, v1, v2 };
            else if ((d1 > eps && d0 <= eps && d2 <= eps) || (d1 < -eps && d0 >= -eps && d2 >= -eps)) v = new[] { v1, v2, v0 };
            else v = new[] { v2, v0, v1 };

            Vector2 i1 = Snap(GetIntersection(p1, p2, v[0], v[1]));
            Vector2 i2 = Snap(GetIntersection(p1, p2, v[0], v[2]));

            // Tworzymy trójkąty
            var triA = new[] { v[0], i1, i2 };
            var triB = new[] { i1, v[1], v[2] };
            var triC = new[] { i1, v[2], i2 };

            // Dodaj tylko jeśli trójkąt ma sensowne pole 
            if (GetSide(p1, p2, v[0]) > 0) {
                AddIfValid(left, triA);
                AddIfValid(right, triB);
                AddIfValid(right, triC);
            } else {
                AddIfValid(right, triA);
                AddIfValid(left, triB);
                AddIfValid(left, triC);
            }

            return (left, right);
        }

        private Vector2 Snap(Vector2 v) => new Vector2((float)Math.Round(v.X, 1), (float)Math.Round(v.Y, 1));

        private void AddIfValid(List<Vector2[]> list, Vector2[] tri) {
            // Sprawdzamy czy trójkąt nie jest linią (pole > 0.1)
            float area = Math.Abs((tri[0].X * (tri[1].Y - tri[2].Y) + tri[1].X * (tri[2].Y - tri[0].Y) + tri[2].X * (tri[0].Y - tri[1].Y)) / 2f);
            if (area > 0.5f) list.Add(tri);
        }

        private List<Vector2[]> Triangulate(List<Vector2> vertices)
        {
            List<Vector2[]> triangles = new List<Vector2[]>();
            List<Vector2> verts = new List<Vector2>(vertices);
            
            int safetyCounter = 1000; // Maksymalna liczba prób

            while (verts.Count >= 3 && safetyCounter > 0)
            {
                safetyCounter--;
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
                if (!earFound) break; 
            }
            return triangles;
        }

        private bool IsEar(Vector2 a, Vector2 b, Vector2 c, List<Vector2> allVerts)
        {
            if (GetSide(a, c, b) >= 0) return false;
            foreach (var p in allVerts)
            {
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
            return (d1 <= 0 && d2 <= 0 && d3 <= 0) || (d1 >= 0 && d2 >= 0 && d3 >= 0);
        }

        private float GetSide(Vector2 p1, Vector2 p2, Vector2 v) => (p2.X - p1.X) * (v.Y - p1.Y) - (p2.Y - p1.Y) * (v.X - p1.X);

        private Vector2 GetIntersection(Vector2 l1, Vector2 l2, Vector2 a, Vector2 b)
        {
            float a1 = l2.Y - l1.Y;
            float b1 = l1.X - l2.X;
            float c1 = a1 * l1.X + b1 * l1.Y;

            float a2 = b.Y - a.Y;
            float b2 = a.X - b.X;
            float c2 = a2 * a.X + b2 * a.Y;

            float det = a1 * b2 - a2 * b1;

            // Jeśli linie są równoległe
            if (Math.Abs(det) < 0.0001f) return a;

            return new Vector2((b2 * c1 - b1 * c2) / det, (a1 * c2 - a2 * c1) / det);
        }
    }
}