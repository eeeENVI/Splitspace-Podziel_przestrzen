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

namespace Splitspace_Podziel_przestrzen.States;
public class GameState : State
{
    SpriteFont font;

    private List<Shape2D> _shapes;
    private Vector2? _cutStart = null;
    private bool _debugMode = true; 

    private LevelData _currentLevelData;
    private int _levelCounter = 5;

    private float _totalInitialArea;
    private float _targetPercentage = 50f; // podziel na pół

    public GameState()
    {
        LoadContent();
    }

    public override void LoadContent()
    {
        font = Content.Load<SpriteFont>("Fonts/testFont");

        /*_shapes = new List<Shape2D>();
        var startShape = ShapeFactory.CreateRandomConvexPolygon(new Vector2(640, 512), 300, 6);
        _shapes.Add(startShape);
        _totalInitialArea = startShape.CalculateArea();*/

        LoadLevel(_levelCounter);
    }

    private void LoadLevel(int number)
    {
        _currentLevelData = LevelFactory.GenerateNextLevel(number);
        _shapes = new List<Shape2D> { _currentLevelData.StartingShape };
        _totalInitialArea = _currentLevelData.StartingShape.CalculateArea();

        _targetPercentage = _currentLevelData.TargetPercentage;
    }

    private void OnSplit(Vector2 start, Vector2 end)
    {
        // Zabezpieczenie przed kliknięciem w miejscu
        if (Vector2.Distance(start, end) < 10f) return;

        List<Shape2D> resultParts = new List<Shape2D>();
        foreach (var shape in _shapes)
        {
            resultParts.AddRange(shape.Split(start, end));
        }

        // Sprawdzamy, czy cokolwiek zostało przecięte
        if (resultParts.Count > _shapes.Count)
        {
            var score = LevelJudge.EvaluateSplit(
                _currentLevelData.TargetPercentage, 
                resultParts, 
                _totalInitialArea, 
                _currentLevelData.Tolerance
            );

            if (score.IsPassed)
            {
                _levelCounter++;
                LoadLevel(_levelCounter);
            }
            else
            {
                _shapes = resultParts;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        InputManager.Update();

        if(InputManager.WasKeyTriggered(Keys.Escape)) quit = true;

        // Przełączanie trybu debug klawiszem F3
        if (InputManager.WasKeyTriggered(Keys.F3))
        {
            _debugMode = !_debugMode;
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

    public override void Draw()
    {
        foreach (var shape in _shapes)
        {
            ShapeRenderer.DrawShape(GlobalData.SpriteBatch, shape, _debugMode);
            
            // Obliczanie i rysowanie % powierzchni każdego kawałka
            float currentArea = shape.CalculateArea();
            float perc = (currentArea / _totalInitialArea) * 100f;
            
            if(_debugMode) 
            {
                string label = $"{Math.Round(perc, 1)}%";
                Vector2 labelSize = font.MeasureString(label);
                GlobalData.SpriteBatch.DrawString(font, label, shape.GetCentroid() - (labelSize / 2), Color.White);
            }
        }

        // Rysowanie aktualnej linii cięcia
        if (_cutStart != null)
        {
            var mouse = Mouse.GetState();
            ShapeRenderer.DrawLine(GlobalData.SpriteBatch, _cutStart.Value, new Vector2(mouse.X, mouse.Y), Color.White * 0.5f, 1);
        }

        // info
        string levelInfo = $"POZIOM: {_levelCounter}";
        string goalInfo = $"CEL: {_targetPercentage}% / {100 - _targetPercentage}%";
        string toleranceInfo = _debugMode ? $" (Tolerancja: {_currentLevelData.Tolerance}%)" : "";

        GlobalData.SpriteBatch.DrawString(font, levelInfo, new Vector2(50, 50), Color.White);
        GlobalData.SpriteBatch.DrawString(font, goalInfo + toleranceInfo, new Vector2(500, 50), Color.Gold);
    }
}