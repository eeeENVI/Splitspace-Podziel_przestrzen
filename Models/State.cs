using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using Splitspace_Podziel_przestrzen.Globals;

// Base Class for Variety of States in App: MainMenu,Options,Game,Camera,Editor,Playground,Pause menu etc..
// Simple for handling and seperating diffrent parts of application
// Maybe in future we will need StateData class
namespace Splitspace_Podziel_przestrzen.Models;
public abstract class State
{
    public bool quit {get; protected set;}
    protected ContentManager Content;
    
    public State() 
    { 
        quit = false; 
        Content = new ContentManager(GlobalData.Content.ServiceProvider,"Content");
        Console.WriteLine("Starting State" + Game1.StateManager.Count.ToString());
    }

    public virtual void LoadContent(){}
    protected virtual void UnloadContent()
    {
        // need more testing
        if (Content != null)
        {
            Content.Unload(); 
            Content.Dispose(); 
            Content = null;
            Console.WriteLine("Clearing State content" + (Game1.StateManager.Count-1).ToString());
        }
    }

    public virtual void Update(GameTime gameTime){}
    public virtual void Draw(){}

    public virtual void End()
    {
        UnloadContent();

        Console.WriteLine("Ending State" + (Game1.StateManager.Count-1).ToString());
    }
}