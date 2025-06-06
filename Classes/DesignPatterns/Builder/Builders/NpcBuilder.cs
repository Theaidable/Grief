﻿using Greif;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Builder.Builders
{
    /// <summary>
    /// Concrete builder for NPC'er
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class NpcBuilder : IGameObjectBuilder
    {
        //Private Fields
        private GameObject npc;

        private string name;
        private List<string> dialogLinesBeforeAccept;
        private List<string> dialogLinesAcceptedNotCompleted;
        private List<string> dialogLinesOnCompleted;
        private List<string> dialogLinesAlreadyCompleted;
        private Quest quest;


        /// <summary>
        /// Constructor
        /// </summary>
        public NpcBuilder()
        {
            npc = new GameObject();
        }

        /// <summary>
        /// Metode til at sætte en NPC's tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public IGameObjectBuilder SetTag(string tag)
        {
            npc.Tag = tag;
            return this;
        }

        /// <summary>
        /// Metode til at sætte en NPC's position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public IGameObjectBuilder SetPosition(Vector2 position)
        {
            npc.Transform.Position = position;
            return this;
        }

        /// <summary>
        /// Metode til at sætte en NPC's navn
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IGameObjectBuilder SetName(string name)
        {
            this.name = name;
            return this;
        }

        /// <summary>
        /// Metode til at sætte en NPC's dialog
        /// </summary>
        /// <param name="linesBeforeAccept"></param> Før man acceptere en mulig quest
        /// <param name="linesAcceptedNotCompleted"></param> Efter man har accepeteret en quest men ikke fuldført den
        /// <param name="linesOnCompleted"></param> Når man fuldfører den
        /// <param name="linesAlreadyCompleted"></param> Efter man har klaret questen og prøver at snakke med ham/hende igen
        /// <returns></returns>
        public IGameObjectBuilder SetDialog(List<string> linesBeforeAccept, List<string> linesAcceptedNotCompleted, List<string> linesOnCompleted, List<string> linesAlreadyCompleted)
        {
            dialogLinesBeforeAccept = linesBeforeAccept;
            dialogLinesAcceptedNotCompleted = linesAcceptedNotCompleted;
            dialogLinesOnCompleted = linesOnCompleted;
            dialogLinesAlreadyCompleted = linesAlreadyCompleted;
            return this;
        }

        /// <summary>
        /// Metode til at sætte en NPC's quest
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public IGameObjectBuilder SetQuest(Quest quest)
        {
            this.quest = quest;
            return this;
        }

        /// <summary>
        /// Metode til at adde komponenten SpriteRenderer
        /// </summary>
        /// <returns></returns>
        public IGameObjectBuilder AddSpriteRenderer()
        {
            var spriteRenderer = npc.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite("NPCs//Dad/Idle/Idle01");
            return this;
        }

        /// <summary>
        /// Metode til at adde komponenten Animator
        /// </summary>
        /// <returns></returns>
        public IGameObjectBuilder AddAnimator()
        {
            npc.AddComponent<Animator>();
            return this;
        }

        /// <summary>
        /// Metode til at adde komponenten Collider
        /// </summary>
        /// <returns></returns>
        public IGameObjectBuilder AddCollider()
        {
            npc.AddComponent<Collider>();
            return this;
        }

        /// <summary>
        /// Metode til at tilføje en bestemt unik komponent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Component AddScriptComponent<T>() where T : Component
        {
            return npc.AddComponent<T>();
        }

        /// <summary>
        /// Returnere et GameObject (En NPC)
        /// </summary>
        /// <returns></returns>
        public GameObject GetResult()
        {
            var npcComponent = npc.GetComponent<NpcComponent>();

            if(npcComponent != null)
            {
                npcComponent.Name = name;
                npcComponent.DialogLinesBeforeAccept = dialogLinesBeforeAccept;
                npcComponent.DialogLinesAcceptedNotCompleted = dialogLinesAcceptedNotCompleted;
                npcComponent.DialogLinesOnCompleted = dialogLinesOnCompleted;
                npcComponent.DialogLinesAlreadyCompleted = dialogLinesAlreadyCompleted;
                npcComponent.QuestToGive = quest;
                npcComponent.Portrait = GameWorld.Instance.Content.Load<Texture2D>($"NPCs/Portraits/Portrait{name}");
            }

            return npc;
        }
    }
}
