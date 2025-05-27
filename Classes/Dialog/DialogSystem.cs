using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;

namespace Grief.Classes.Dialog
{
    public class DialogSystem
    {
        private Queue<string> currentLiners;
        private Texture2D npcPortrait;
        private string currentLine;
        private Action<bool> onDialogFinished;

        public bool IsActive { get; private set; }


    }
}
