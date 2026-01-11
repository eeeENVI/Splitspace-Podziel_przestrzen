using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Splitspace_Podziel_przestrzen.Managers;
using Splitspace_Podziel_przestrzen.Globals;
using Splitspace_Podziel_przestrzen.States;

namespace Splitspace_Podziel_przestrzen;

public class Game1 : Game
{


    public Game1()
    {
        GlobalData.Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        GlobalData.StateManager = new StateManager();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        GlobalData.Graphics.PreferredBackBufferWidth = 1280;
        GlobalData.Graphics.PreferredBackBufferHeight = 1024;
        GlobalData.Graphics.ApplyChanges(); 

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
        GlobalData.GraphicsDevice = GraphicsDevice;

        GlobalData.SpriteBatch = new SpriteBatch(GlobalData.GraphicsDevice);

        GlobalData.Content = Content;

        // Entry Point
        GlobalData.StateManager.addState(new MainMenuState());
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        GlobalData.StateManager.Update(gameTime);

        if(GlobalData.StateManager.IsEmpty) Exit();

        base.Update(gameTime);

    }

    protected override void Draw(GameTime gameTime)
    {
        // TODO: Add your drawing code here
        if(IsActive)
        {
            GlobalData.Graphics.GraphicsDevice.Clear(Color.Black);

            GlobalData.SpriteBatch.Begin();

            GlobalData.StateManager.Draw();

            GlobalData.SpriteBatch.End();
        }

        base.Draw(gameTime);
    }
}
