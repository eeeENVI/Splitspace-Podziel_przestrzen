using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;

using Splitspace_Podziel_przestrzen.Managers;
using Splitspace_Podziel_przestrzen.Models;
using Splitspace_Podziel_przestrzen.Gui;
using Splitspace_Podziel_przestrzen.Globals;
using Splitspace_Podziel_przestrzen;

namespace Splitspace_Podziel_przestrzen.States;
public class MainMenuState : State
{
    Button btn_graj,btn_statystyki,btn_wyjdz;
    SpriteFont font;
    public MainMenuState()
    {
        LoadContent();
    }

    public override void LoadContent()
    {
        font = Content.Load<SpriteFont>("Fonts/testFont");

        // Paleta kolorów
        Color offWhite   = new Color(255, 231, 231); 
        Color gold       = new Color(255, 210, 143); 
        Color softRed    = new Color(255, 157, 157); 
        Color skyBlue    = new Color(159, 227, 255); 
        Color mintGreen  = new Color(170, 230, 170); 
        Color darkSlate  = new Color(43, 55, 65);   

        // Parametry wspólne
        int buttonWidth = 300;
        int buttonHeight = 50;
        int spacing = 70; 
        float centerX = (1280 - buttonWidth) / 2f; 
        float startY = 600; // Pozycja pierwszego przycisku

        // Konfiguracja przycisków
        btn_graj = new Button(
            new Vector2(centerX, startY), 
            new Vector2(buttonWidth, buttonHeight), 
            font, "GRAJ", 
            darkSlate, // Kolor czcionki
            mintGreen,  // Normalny
            skyBlue,    // Po najechaniu (Hover)
            gold      // Po kliknięciu    
        );

        btn_statystyki = new Button(
            new Vector2(centerX, startY + spacing), 
            new Vector2(buttonWidth, buttonHeight), 
            font, "STATYSTYKI", 
            darkSlate, mintGreen, skyBlue, gold
        );

        btn_wyjdz = new Button(
            new Vector2(centerX, startY + spacing*2), 
            new Vector2(buttonWidth, buttonHeight), 
            font, "WYJDZ", 
            darkSlate, softRed, skyBlue, gold
        );
    }

    public override void Update(GameTime gameTime)
    {
        InputManager.Update();
        if(InputManager.WasKeyTriggered(Keys.Escape)) quit = true;

        btn_graj.Update();
        btn_statystyki.Update();
        btn_wyjdz.Update();
        
        if(btn_graj.Clicked())
        {
            Console.WriteLine(btn_graj.Clicked().ToString());

            GlobalData.StateManager.addState(new GameState());
        } 

        if(btn_statystyki.Clicked())
        {
            Console.WriteLine(btn_statystyki.Clicked().ToString());
        } 

        if(btn_wyjdz.Clicked())
        {
            Console.WriteLine(btn_wyjdz.Clicked().ToString());
            quit = true;
        } 
    }

    public override void Draw()
    {
        btn_graj.Draw();   
        btn_statystyki.Draw();  
        btn_wyjdz.Draw();    
    }
}