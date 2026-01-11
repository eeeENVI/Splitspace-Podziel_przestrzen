using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Splitspace_Podziel_przestrzen.Core;

public static class ShapeRenderer
{
    private static Texture2D _pixel;

    // Inicjalizacja tekstury
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

        var vertices = shape.Vertices;
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 start = vertices[i];
            Vector2 end = vertices[(i + 1) % vertices.Count];

            // Rysowanie krawędzi
            DrawLine(spriteBatch, start, end, shape.Color, 2);

            // Tryb Debug: Rysowanie kropek w miejscach wierzchołków
            if (debugMode)
            {
                spriteBatch.Draw(_pixel, new Rectangle((int)start.X - 3, (int)start.Y - 3, 6, 6), Color.Red);
            }
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
            new Vector2(0, 0),
            SpriteEffects.None,
            0);
    }
}
