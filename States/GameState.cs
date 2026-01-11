using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

using Splitspace_Podziel_przestrzen.Globals;
using Splitspace_Podziel_przestrzen.Managers;
using Splitspace_Podziel_przestrzen.Models;
using Splitspace_Podziel_przestrzen.Gui;
using Splitspace_Podziel_przestrzen.Core;
using Splitspace_Podziel_przestrzen.LevelDesign;
using Splitspace_Podziel_przestrzen;
using Splitspace_Podziel_przestrzen.Stats;

//using System.Security.Cryptography; skad tutajto robi XD


namespace Splitspace_Podziel_przestrzen.States;
public class GameState : State
{
    SpriteFont font;

    private List<Shape2D> _shapes;
    private Vector2? _cutStart = null;
    private LevelData _currentLevelData;
    private int _levelCounter = 1;
    private float _totalInitialArea;
    private int _cutsPerformed = 0;
    private float _rotation = 0f;


    /// <summary>
    ///  Timer + Stats
    /// </summary>
    private float _levelTimer = 0f; 
    private bool _pause = false;

    /// <summary>
    /// Konterer + UI
    /// </summary>
    private Rectangle _gameContainer;
    private Vector2 _containerCenter; 
    private bool _debugMode = true; 
    private bool _showGrid = false;

    public GameState()
    {
        LoadContent();
    }

    public override void LoadContent()
    {
        font = Content.Load<SpriteFont>("Fonts/testFont");

        _gameContainer = new Rectangle(190, 162, 900, 700); 
    
        _containerCenter = new Vector2(
        _gameContainer.X + _gameContainer.Width / 2f,
        _gameContainer.Y + _gameContainer.Height / 2f
        );

        LoadLevel(_levelCounter);
    }

    private void LoadLevel(int number)
    {
        _currentLevelData = LevelFactory.GenerateNextLevel(number,_containerCenter);
        _shapes = new List<Shape2D> { _currentLevelData.StartingShape };
        _totalInitialArea = _shapes[0].CalculateArea();
        _cutsPerformed = 0; 
        _rotation = 0f;    
        _levelTimer = 0f; 
    }

    private void OnSplit(Vector2 start, Vector2 end)
    {
        if (Vector2.Distance(start, end) < 20f) return;
        if (!_gameContainer.Contains(start) || !_gameContainer.Contains(end)) return;

        // 1. Sprawdź, czy gracz może jeszcze ciąć
        if (_cutsPerformed >= _currentLevelData.MaxCuts)
        {
            Console.WriteLine("Brak pozostalych ciec! Resetowanie poziomu...");
            LoadLevel(_levelCounter); // Reset
            return;
        }
 
        List<Shape2D> resultParts = new List<Shape2D>();
        bool anyShapeSplit = false;

        foreach (var shape in _shapes)
        {
            var splitResult = shape.Split(start, end);
            if (splitResult.Count > 1) anyShapeSplit = true;
            resultParts.AddRange(splitResult);
        }

        // 3. Jeśli faktycznie coś zostało przecięte, zwiększ licznik
        if (anyShapeSplit)
        {
            _cutsPerformed++;
            _shapes = resultParts;
            Console.WriteLine($"Wykonano ciecie: {_cutsPerformed} / {_currentLevelData.MaxCuts}");

            var score = LevelJudge.EvaluateSplit(
                _currentLevelData.TargetPercentages, 
                resultParts, 
                _totalInitialArea, 
                _currentLevelData.Tolerance
            );

            if (score.IsPassed)
            {
                /// --- LOGIKA STATYSTYK ---
                // 1. Aktualizujemy
                GlobalData.GlobalStats.UpdateWithLevel(
                    score.Accuracy, 
                    _levelTimer, 
                    _cutsPerformed, 
                    _levelCounter + 1
                );

                GlobalData.GlobalStats.TotalLevelsCompleted++;

                // 2. Zapisujemy
                
                StatsService.Save(GlobalData.GlobalStats);

                Console.WriteLine($"Poziom zaliczony! Celność: {score.Accuracy}%");

                _levelCounter++;
                LoadLevel(_levelCounter);
            }
            else
            {
                if (_cutsPerformed >= _currentLevelData.MaxCuts)
                {
                    GlobalData.GlobalStats.UpdateWithLevel(
                    score.Accuracy, 
                    _levelTimer, 
                    _cutsPerformed, 
                    _levelCounter
                    );

                    StatsService.Save(GlobalData.GlobalStats);

                    Console.WriteLine("Limit ciec osiagniety, a cel nie zostal spelniony. Reset!");
                    LoadLevel(_levelCounter);
                }
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        InputManager.Update();

        if(!_pause) _levelTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
       
        // Sterowanie rotacja figury - do dokonczenia ( macierz transformacji)
        float rotationSpeed = 1.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (InputManager.IsKeyPressed(Keys.Left)) _rotation -= rotationSpeed;
        if (InputManager.IsKeyPressed(Keys.Right)) _rotation += rotationSpeed;


        // Przelaczenie pauzy klawiszem Escape
        if(InputManager.WasKeyTriggered(Keys.Escape)) 
        {
            _pause = !_pause;
        }

        // Przełączanie trybu debug klawiszem F3
        if (InputManager.WasKeyTriggered(Keys.F3))
        {
            _debugMode = !_debugMode;
        }

        // Przełączanie trybu grid klawiszem G
        if (InputManager.WasKeyTriggered(Keys.G))
        {
           _showGrid = !_showGrid;
        }
        
        // 1. Rozpoczęcie cięcia
        if (InputManager.MouseLeftTriggered())
        {
            _cutStart = InputManager.MousePos;
        }

        // 2. Finalizacja cięcia
        if (InputManager.MouseLeftReleased() && _cutStart != null)
        {
            // 3. Wywołanie mechaniki cięcia i punktacji
            OnSplit(_cutStart.Value, InputManager.MousePos);
            
            _cutStart = null;
        }
    }   

    private void DrawGrid(SpriteBatch sb)
    {
        if (!_showGrid) return;

        int gridStep = 10; 

        for (int x = _gameContainer.X; x <= _gameContainer.Right; x += gridStep)
        {
            // Co 10 rysujemy mocniejszą linię
            bool isMajor = (x - _gameContainer.X) % 100 == 0;
            float alpha = isMajor ? 0.2f : 0.05f;
            
            ShapeRenderer.DrawLine(sb, 
            new Vector2(x, _gameContainer.Top), 
            new Vector2(x, _gameContainer.Bottom), 
            Color.White * alpha, 
            1);
        }

        for (int y = _gameContainer.Y; y <= _gameContainer.Bottom; y += gridStep)
        {
            bool isMajor = (y - _gameContainer.Y) % 100 == 0;
            float alpha = isMajor ? 0.2f : 0.05f;

            ShapeRenderer.DrawLine(sb, 
            new Vector2(_gameContainer.Left, y), 
            new Vector2(_gameContainer.Right, y), 
            Color.White * alpha, 
            1);
        }

    }

    public override void Draw()
    {
        var sb = GlobalData.SpriteBatch;

        ShapeRenderer.DrawOutline(sb, _gameContainer,2, new Color(18, 18, 22));

        DrawGrid(sb);

        foreach (var shape in _shapes)
        {
            ShapeRenderer.DrawShape(sb, shape, _debugMode);
            
            // Obliczanie i rysowanie % powierzchni każdego kawałka
            float currentArea = shape.CalculateArea();
            float perc = (currentArea / _totalInitialArea) * 100f;
            
            if(_debugMode) 
            {
                ShapeRenderer.DrawShapeFilled(sb, shape);
                string label = $"{Math.Round(perc, 1)}%";
                Vector2 labelSize = font.MeasureString(label);
                sb.DrawString(font, label, shape.GetCentroid() - (labelSize / 2), Color.White);
            }
        }

        // Rysowanie aktualnej linii cięcia
        if (_cutStart != null)
        {
            var mouse = Mouse.GetState();
            ShapeRenderer.DrawLine(sb, _cutStart.Value, new Vector2(mouse.X, mouse.Y), Color.White * 0.5f, 1);
        }

        // info
        string levelInfo = $"POZIOM: {_levelCounter}";
        string goalInfo = "CELE: " + string.Join("% | ", _currentLevelData.TargetPercentages) + "%";
        string toleranceInfo = $" (Tolerancja: {_currentLevelData.Tolerance}%)";
        int remainingCuts = _currentLevelData.MaxCuts - _cutsPerformed;
        string cutsInfo = $"Pozostale ciecia: {remainingCuts}";

        sb.DrawString(font, levelInfo, new Vector2(50, 50), Color.White);
        sb.DrawString(font, goalInfo + toleranceInfo, new Vector2(500, 50), Color.Gold);
        sb.DrawString(font, cutsInfo, new Vector2(20, 70), remainingCuts > 0 ? Color.White : Color.Red);
    }


}