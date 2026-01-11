using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Splitspace_Podziel_przestrzen.Core;

public static class ShapeRenderer
{
    private static Texture2D _pixel;

    private static void CreatePixel(GraphicsDevice graphicsDevice)
    {
        if (_pixel == null)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
    }

    public static void DrawShape(SpriteBatch spriteBatch, Shape2D shape, bool debugMode)
    {
        CreatePixel(spriteBatch.GraphicsDevice);

        // Zbieramy wszystkie krawędzie ze wszystkich trójkątów
        var edges = new List<(Vector2 p1, Vector2 p2)>();
        foreach (var tri in shape.Triangles)
        {
            edges.Add(NormalizeEdge(tri[0], tri[1]));
            edges.Add(NormalizeEdge(tri[1], tri[2]));
            edges.Add(NormalizeEdge(tri[2], tri[0]));
        }

        // Krawędź zewnętrzna to taka, która występuje w liście tylko RAZ
        var boundaryEdges = edges
            .GroupBy(e => e)
            .Where(g => g.Count() == 1)
            .Select(g => g.Key);

        foreach (var edge in boundaryEdges)
        {
            DrawLine(spriteBatch, edge.p1, edge.p2, shape.Color, 2);
        }

        if (debugMode)
        {
            foreach (var tri in shape.Triangles)
            {
                foreach (var v in tri)
                {
                    spriteBatch.Draw(_pixel, new Rectangle((int)v.X - 3, (int)v.Y - 3, 6, 6), Color.Red);
                }
                
                // Rysujemy siatkę trójkątów
                DrawLine(spriteBatch, tri[0], tri[1], Color.White * 0.2f, 1);
                DrawLine(spriteBatch, tri[1], tri[2], Color.White * 0.2f, 1);
                DrawLine(spriteBatch, tri[2], tri[0], Color.White * 0.2f, 1);
            }
        }
    }

    // funkcja do wypełnienia
    public static void DrawShapeFilled(SpriteBatch spriteBatch, Shape2D shape)
    {
        CreatePixel(spriteBatch.GraphicsDevice);
        
        foreach (var tri in shape.Triangles)
        {
            // mono basiceffect sprawdz 
            DrawLine(spriteBatch, tri[0], tri[1], shape.Color * 0.3f, 1);
            DrawLine(spriteBatch, tri[1], tri[2], shape.Color * 0.3f, 1);
            DrawLine(spriteBatch, tri[2], tri[0], shape.Color * 0.3f, 1);
        }
    }

    public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
    {
        CreatePixel(spriteBatch.GraphicsDevice);

        Vector2 edge = end - start;
        float angle = (float)System.Math.Atan2(edge.Y, edge.X);

        spriteBatch.Draw(_pixel,
            new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
            null,
            color,
            angle,
            Vector2.Zero,
            SpriteEffects.None,
            0);
    }

    public static void DrawOutline(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color)
    {
        CreatePixel(spriteBatch.GraphicsDevice);

        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color); 
        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color); 
        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color); 
    }

    private static (Vector2, Vector2) NormalizeEdge(Vector2 v1, Vector2 v2)
    {
        if (v1.X < v2.X || (v1.X == v2.X && v1.Y < v2.Y))
            return (v1, v2);
        return (v2, v1);
    }
}