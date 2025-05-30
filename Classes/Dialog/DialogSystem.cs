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
    /// <summary>
    /// System for dialog
    /// </summary>
    public class DialogSystem
    {
        //Fields
        private Queue<string> currentLines;
        private Texture2D npcPortrait;
        private string currentLine;
        private Action<bool> onDialogFinished;
        private bool wasEnterPressed;
        private bool awaitingResponse;

        //Properties
        public bool IsActive { get; private set; }

        /// <summary>
        /// Start dialog
        /// </summary>
        /// <param name="lines"></param> Linjer
        /// <param name="portrait"></param> Billede
        /// <param name="onFinished"></param> Hvad der sker når man er færdig
        public void StartDialog(List<string> lines, Texture2D portrait, Action<bool> onFinished = null)
        {
            //His ikke der er nogle dialog lines, så sker der ingenting
            if(lines == null || lines.Count == 0)
            {
                return;
            }

            currentLines = new Queue<string>(lines);
            npcPortrait = portrait;
            IsActive = true;
            this.onDialogFinished = onFinished;
            NextLine();
        }

        /// <summary>
        /// Logikken for dialog
        /// </summary>
        public void Update()
        {

            if(IsActive == true)
            {
                //Hvis der ventes på svar
                if (awaitingResponse == true)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Y))
                    {
                        awaitingResponse = false;
                        IsActive = false;
                        //Quest accepteres
                        onDialogFinished?.Invoke(true);
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.N))
                    {
                        awaitingResponse = false;
                        IsActive = false;
                        //Quests declines
                        onDialogFinished?.Invoke(false);
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && wasEnterPressed == false)
                    {
                        if (currentLines.Count > 0)
                        {
                            NextLine();
                        }
                        else if(onDialogFinished != null)
                        {
                            awaitingResponse = true;
                            currentLine = "Y for accept, N for decline";
                        }
                        else
                        {
                            IsActive = false;
                        }

                        wasEnterPressed = true;
                    }
                    else if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                    {
                        wasEnterPressed = false;
                    }
                }
            }
        }

        /// <summary>
        /// Hjælpemetode til at køre næste linje
        /// </summary>
        private void NextLine()
        {
            currentLine = currentLines.Dequeue();
        }

        /// <summary>
        /// Tegn dialogboks, portræt af den som snakker samt dialog
        /// </summary>
        /// <param name="spriteBatch"></param>
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
