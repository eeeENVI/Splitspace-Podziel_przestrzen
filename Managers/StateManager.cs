using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Splitspace_Podziel_przestrzen.Models;


namespace Splitspace_Podziel_przestrzen.Managers;
public class StateManager
{
    private readonly Stack<State> StatesStack;
    // Returns current number of States
    public int Count {get {return StatesStack.Count;}}
    public bool IsEmpty {get {return Count <= 0;}}
    public StateManager()
    {
        StatesStack = new Stack<State>();
    }

    public void addState(State State)
    {
        StatesStack.Push(State);
    }

    public void removeState()
    {
        StatesStack.Pop();
    }

    public State getCurrentState()
    {
        return StatesStack.Peek();
    }

    public void Update(GameTime gameTime)
    {
        if(!IsEmpty)
        {
            getCurrentState().Update(gameTime);     
            
            if(getCurrentState().quit)
            {
                getCurrentState().End();
                removeState();
            }
        }
    }

    public void Draw()
    {
        if(!IsEmpty)
        {
            getCurrentState().Draw();
        }
    }
}
