using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splitspace_Podziel_przestrzen;
using Splitspace_Podziel_przestrzen.Managers;
using Splitspace_Podziel_przestrzen.Globals;

namespace Splitspace_Podziel_przestrzen.Gui
{
    public static class GuiManager
    {
        //scalingFactor {get; set;}
        //public int count {get; set;}
    }

    public enum button_states {IDLE = 0, HOVER, ACTIVE};
    public class Button
    {
        private int _buttonState;
        public Vector2 position;
        public Vector2 resolution;
        private Texture2D texture;
        public Color idle_color, hover_color, active_color;
        private Color mask_color;
        private SpriteFont font;
        private string text;
        private Vector2 textPos;
        public Color text_color;
        
        private Rectangle rect
        {
            get
            {
                return new Rectangle((int)position.X,(int)position.Y,(int)resolution.X,(int)resolution.Y);
            }
        }

        public void UpdateTextPosition()
        {
            textPos = position + (resolution - font.MeasureString(text)) / 2;
        }

        // No Texture pass
        public Button(Vector2 position, Vector2 resolution,
        SpriteFont font, string text, Color text_color,
        Color idle_color,Color hover_color,Color active_color)
        {
            // Button coord
            _buttonState = (int)button_states.IDLE;
            this.position = position;
            this.resolution = resolution;

            this.font = font;
            this.text = text;

            textPos = position + (resolution - font.MeasureString(text))/2;

            this.text_color = text_color;
            this.idle_color = idle_color;
            this.hover_color =hover_color;
            this.active_color = active_color;

            texture = new Texture2D(GlobalData.GraphicsDevice,(int)resolution.X,(int)resolution.Y);
            Color[] data = new Color[(int)resolution.X*(int)resolution.Y];
            for(int i=0; i < data.Length; ++i) data[i] = Color.White;
            texture.SetData(data);
        }

        // Texture pass
        public Button(Vector2 position, Vector2 resolution, Texture2D texture,
        SpriteFont font, string text, Color text_color,
        Color idle_color,Color hover_color,Color active_color)
        {
            // Button coord
            _buttonState = (int)button_states.IDLE;
            this.position = position;
            this.resolution = resolution;

            // Texts
            this.font = font;
            this.text = text;

            textPos = position + (resolution - font.MeasureString(text))/2;

            // Colors & Textures
            this.text_color = text_color;
            this.idle_color = idle_color;
            this.hover_color =hover_color;
            this.active_color = active_color;

            this.texture = texture;
        }



        public bool Clicked()
        {
            return _buttonState == (int)button_states.ACTIVE;
        }

        public void Update()
        {
            _buttonState = (int)button_states.IDLE;

            if (InputManager.MouseCursor.Intersects(rect))
            {
                _buttonState = (int)button_states.HOVER;
                if(InputManager.MouseClicked)
                {
                    _buttonState = (int)button_states.ACTIVE;
                }
            }

            switch(_buttonState)
            {
                case (int)button_states.IDLE:
                mask_color = idle_color;
                break;
                case (int)button_states.HOVER:
                mask_color = hover_color;
                break;
                case (int)button_states.ACTIVE:
                mask_color = active_color;
                break;
                default: 
                //ERROR
                mask_color = Color.Black; 
                break;
            }
            
        }

        public void Draw()
        {
           // GlobalData.SpriteBatch.Begin();
            GlobalData.SpriteBatch.Draw(texture,rect,mask_color);
            GlobalData.SpriteBatch.DrawString(font,text,textPos,text_color);
            //GlobalData.SpriteBatch.End();
        }
    }
}

   
