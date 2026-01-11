using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Splitspace_Podziel_przestrzen.Stats;
using Splitspace_Podziel_przestrzen.Managers;
using Splitspace_Podziel_przestrzen.Models;
using Splitspace_Podziel_przestrzen.Gui;
using Splitspace_Podziel_przestrzen.Core;
using Splitspace_Podziel_przestrzen.Globals;
using Splitspace_Podziel_przestrzen;

namespace Splitspace_Podziel_przestrzen.States;
public class GlobalStatsState : State
{
    private SpriteFont font;
    private Rectangle _bgContainer;

    public GlobalStatsState()
    {
        LoadContent();
    }

    public override void LoadContent()
    {
        font = Content.Load<SpriteFont>("Fonts/testFont");

        // Tło statystyk - wyśrodkowany kontener
        _bgContainer = new Rectangle(340, 150, 600, 700);

        if (GlobalData.GlobalStats == null)
        {
            GlobalData.GlobalStats = StatsService.Load();
        }
    }

    public override void Update(GameTime gameTime)
    {
        InputManager.Update();
        if(InputManager.WasKeyTriggered(Keys.Escape))
        {
            quit = true;
        }
    }

    public override void Draw()
    {
        var sb = GlobalData.SpriteBatch;
        var stats = GlobalData.GlobalStats;

        // 1. Ciemne tło całego ekranu (używamy DrawOutline z dużą grubością jako wypełnienie)
        ShapeRenderer.DrawOutline(sb, new Rectangle(0, 0, 1280, 1024), 1024, new Color(10, 10, 15));
        
        // 2. Kontener statystyk (wypełnienie)
        ShapeRenderer.DrawOutline(sb, _bgContainer, _bgContainer.Height, new Color(25, 25, 35));
        
        // Opcjonalnie: subtelna ramka wokół kontenera, żeby wyglądało pro
        ShapeRenderer.DrawOutline(sb, _bgContainer, 2, Color.DimGray * 0.5f);

        // 3. Nagłówek
        string title = "GLOBALNE STATYSTYKI";
        Vector2 titleSize = font.MeasureString(title);
        sb.DrawString(font, title, new Vector2(640 - titleSize.X / 2, 80), Color.Gold);

        // 4. Wyświetlanie danych
        int startX = _bgContainer.X + 50;
        int startY = _bgContainer.Y + 60;
        int spacing = 55;

        DrawStatLine(sb, "Rozegrane mecze:", stats.TotalGamesPlayed.ToString(), startX, startY, Color.Cyan);
        DrawStatLine(sb, "Ukonczone poziomy:", stats.TotalLevelsCompleted.ToString(), startX, startY + spacing, Color.White);
        DrawStatLine(sb, "Czas w grze:", $"{stats.TotalTimePlayed:F1} sek", startX, startY + spacing * 2, Color.White);
        DrawStatLine(sb, "Wykonane ciecia:", stats.TotalCutsPerformed.ToString(), startX, startY + spacing * 3, Color.White);
        DrawStatLine(sb, "Najwyzszy poziom:", stats.HighestLevelReached.ToString(), startX, startY + spacing * 4, Color.LimeGreen);
        
        // 5. Elegancki Separator przy użyciu DrawLine
        ShapeRenderer.DrawLine(sb, 
            new Vector2(startX, startY + spacing * 5 - 10), 
            new Vector2(_bgContainer.Right - 50, startY + spacing * 5 - 10), 
            Color.DimGray, 1);

        DrawStatLine(sb, "Srednia precyzja:", $"{stats.AverageAccuracy:F2}%", startX, startY + spacing * 5, Color.Gold);
        DrawStatLine(sb, "Sredni czas / LVL:", $"{stats.AverageTimePerLevel:F1}s", startX, startY + spacing * 6, Color.White);
        DrawStatLine(sb, "Srednia ciec / LVL:", $"{stats.AverageCutsPerLevel:F2}", startX, startY + spacing * 7, Color.White);
        DrawStatLine(sb, "Win / Loss Ratio:", $"{stats.WinLossRatio:F1}%", startX, startY + spacing * 8, Color.Cyan);

        // Stopka
        string footer = "[ESC] Powrot do menu";
        sb.DrawString(font, footer, new Vector2(640 - font.MeasureString(footer).X / 2, 920), Color.DarkGray);
    }

    private void DrawStatLine(SpriteBatch sb, string label, string value, int x, int y, Color valueColor)
    {
        sb.DrawString(font, label, new Vector2(x, y), Color.LightGray);
        Vector2 valSize = font.MeasureString(value);
        sb.DrawString(font, value, new Vector2(_bgContainer.Right - 50 - valSize.X, y), valueColor);
    }
}