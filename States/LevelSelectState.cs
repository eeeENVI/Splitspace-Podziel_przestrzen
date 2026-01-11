using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

using Splitspace_Podziel_przestrzen.Core;
using Splitspace_Podziel_przestrzen.Managers;
using Splitspace_Podziel_przestrzen.Models;
using Splitspace_Podziel_przestrzen.Gui;
using Splitspace_Podziel_przestrzen.Globals;
using Splitspace_Podziel_przestrzen;
using Splitspace_Podziel_przestrzen.LevelDesign;
using Splitspace_Podziel_przestrzen.Stats;

namespace Splitspace_Podziel_przestrzen.States;
public class LevelSelectState : State
{
    SpriteFont font;
    private List<Button> _levelButtons = new List<Button>();
    private Rectangle _scrollContainer;
    private float _scrollOffset = 0;
    private int _maxScroll;
    private int _lastScrollValue;

    // Parametry siatki
    private const int Cols = 5;
    private const int BtnSize = 100;
    private const int Padding = 20;

    public LevelSelectState()
    {
        LoadContent();
    }

    public override void LoadContent()
    {
        font = Content.Load<SpriteFont>("Fonts/testFont");

        _scrollContainer = new Rectangle(340, 200, 600, 600);
        _lastScrollValue = Mouse.GetState().ScrollWheelValue;

        int highestLevel = GlobalData.GlobalStats.HighestLevelReached;
        int levelsToGenerate = highestLevel;

        // Paleta kolorów
        Color offWhite   = new Color(255, 231, 231); 
        Color gold       = new Color(255, 210, 143); 
        Color softRed    = new Color(255, 157, 157); 
        Color skyBlue    = new Color(159, 227, 255); 
        Color mintGreen  = new Color(170, 230, 170); 
        Color darkSlate  = new Color(43, 55, 65);   

        for (int i = 1; i <= levelsToGenerate; i++)
        {
            var btn = new Button(

                Vector2.Zero, 
                new Vector2(BtnSize, BtnSize),
                font, i.ToString(),
                darkSlate,       
                mintGreen,   
                skyBlue,     
                gold         
            );
            _levelButtons.Add(btn);
        }

        // Obliczamy zasięg scrolla
        int rows = (int)Math.Ceiling((double)levelsToGenerate / Cols);
        int totalHeight = rows * (BtnSize + Padding) + Padding;
        _maxScroll = Math.Max(0, totalHeight - _scrollContainer.Height);
    }

    public override void Update(GameTime gameTime)
    {
        InputManager.Update();
        if(InputManager.WasKeyTriggered(Keys.Escape)) quit = true;

        var mState = Mouse.GetState();
        
        int scrollDelta = mState.ScrollWheelValue - _lastScrollValue;
        _lastScrollValue = mState.ScrollWheelValue;

        if (_scrollContainer.Contains(mState.Position))
        {
            _scrollOffset -= (scrollDelta * 0.8f);
            _scrollOffset = MathHelper.Clamp(_scrollOffset, 0, _maxScroll);
        }

        int totalSpacing = _scrollContainer.Width - (Cols * BtnSize);
        int dynamicPadding = totalSpacing / (Cols + 1); 

        for (int i = 0; i < _levelButtons.Count; i++)
        {
            int row = i / Cols;
            int col = i % Cols;

            float x = _scrollContainer.X + dynamicPadding + (col * (BtnSize + dynamicPadding));
            
            float y = _scrollContainer.Y + dynamicPadding + (row * (BtnSize + dynamicPadding)) - _scrollOffset;

            _levelButtons[i].position = new Vector2(x, y);
            _levelButtons[i].UpdateTextPosition(); 
            
            if (_scrollContainer.Contains(mState.Position))
                _levelButtons[i].Update();

            if (_levelButtons[i].Clicked())
            {
                // Przekazujemy i-ty poziom do gry
            }
        }
    }

    public override void Draw()
    {
        var sb = GlobalData.SpriteBatch;

        ShapeRenderer.DrawOutline(sb, new Rectangle(0,0,1280,1024), 1024, new Color(15, 15, 20));
        ShapeRenderer.DrawOutline(sb, _scrollContainer, _scrollContainer.Height, new Color(25, 25, 30));

        sb.End(); 
        RasterizerState rs = new RasterizerState { ScissorTestEnable = true };
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, rs);
        
        sb.GraphicsDevice.ScissorRectangle = _scrollContainer;

        foreach (var btn in _levelButtons)
        {
            if (btn.position.Y + BtnSize > _scrollContainer.Top && btn.position.Y < _scrollContainer.Bottom)
            {
                btn.Draw();
            }
        }

        sb.End();
        sb.Begin(); // Powrót do normalnego trybu
        ShapeRenderer.DrawOutline(sb, _scrollContainer, 3, Color.Gold * 0.5f);
    }
}