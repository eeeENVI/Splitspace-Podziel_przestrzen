using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace States;
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
                //dispose of State data inside End maybe add destructor later
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
