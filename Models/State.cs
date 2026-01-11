using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using Splitspace_Podziel_przestrzen.Globals;

namespace Splitspace_Podziel_przestrzen.Models;
public abstract class State
{
    public bool quit {get; protected set;}
    protected ContentManager Content;
    
    public State() 
    { 
        quit = false; 
        Content = new ContentManager(GlobalData.Content.ServiceProvider,"Content");
        Console.WriteLine("Starting State" + GlobalData.StateManager.Count.ToString());
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
            Console.WriteLine("Clearing State content" + (GlobalData.StateManager.Count-1).ToString());
        }
    }

    public virtual void Update(GameTime gameTime){}
    public virtual void Draw(){}

    public virtual void End()
    {
        UnloadContent();

        Console.WriteLine("Ending State" + (GlobalData.StateManager.Count-1).ToString());
    }
}