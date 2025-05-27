using Greif;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Grief.Classes.Dialog
{
    public class DialogSystem
    {
        private Queue<string> currentLines;
        private Texture2D npcPortrait;
        private string currentLine;
        private Action<bool> onDialogFinished;
        private bool wasEnterPressed;

        public bool IsActive { get; private set; }

        public void StartDialog(List<string> lines, Texture2D portrait, Action<bool> onFinished = null)
        {
            if(lines == null || lines.Count == 0)
            {
                return;
            }

            Debug.WriteLine($"Starting dialog with {lines.Count} lines.");

            currentLines = new Queue<string>(lines);
            npcPortrait = portrait;
            IsActive = true;
            this.onDialogFinished = onFinished;
            NextLine();
        }

        public void Update()
        {
            if(IsActive == true)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Enter) && wasEnterPressed == false)
                {
                    if (currentLines.Count > 0)
                    {
                        NextLine();
                        wasEnterPressed = true;
                    }
                    else
                    {
                        ShowChoice();
                    }
                }
                else if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                {
                    wasEnterPressed = false;
                }
            }
        }

        private void NextLine()
        {
            currentLine = currentLines.Dequeue();
        }

        private void ShowChoice()
        {
            //Dette er midlertidig måde at accepetere eller benægte quests
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                onDialogFinished?.Invoke(true);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                onDialogFinished?.Invoke(false);
            }

            IsActive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(IsActive == true)
            {
                Rectangle box = new Rectangle(90,200,300,50);
                spriteBatch.Draw(GameWorld.Instance.Pixel, box, Color.Black * 0.8f);

                if(npcPortrait != null)
                {
                    spriteBatch.Draw(npcPortrait, new Rectangle(90, 200, 40, 40), Color.White);
                }

                if(GameWorld.Instance.DefaultFont != null)
                {
                    spriteBatch.DrawString(GameWorld.Instance.DefaultFont,currentLine,new Vector2(130,215), Color.White);
                }
            }
        }
    }
}
