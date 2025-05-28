using Greif;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using System.Collections.Generic;
using System.Drawing;

namespace Grief.Classes.DesignPatterns.Builder.Builders
{
    public class NpcBuilder : IGameObjectBuilder
    {
        private GameObject npc;

        public string Name { get; private set; }
        public List<string> DialogLinesBeforeAccept { get; private set; }
        public List<string> DialogLinesAcceptedNotCompleted { get; private set; }
        public List<string> DialogLinesOnCompleted { get; private set; }
        public List<string> DialogLinesAlreadyCompleted { get; private set; }
        public Quest Quest { get; private set; }
        public bool HasQuest { get; private set; }
        public bool QuestGiven { get; private set; }

        public NpcBuilder()
        {
            npc = new GameObject();
        }

        public IGameObjectBuilder SetTag(string tag)
        {
            npc.Tag = tag;
            return this;
        }

        public IGameObjectBuilder SetPosition(Vector2 position)
        {
            npc.Transform.Position = position;
            return this;
        }

        public IGameObjectBuilder SetName(string name)
        {
            Name = name;
            return this;
        }
        public IGameObjectBuilder SetDialog(List<string> linesBeforeAccept, List<string> linesAcceptedNotCompleted, List<string> linesOnCompleted, List<string> linesAlreadyCompleted)
        {
            DialogLinesBeforeAccept = linesBeforeAccept;
            DialogLinesAcceptedNotCompleted = linesAcceptedNotCompleted;
            DialogLinesOnCompleted = linesOnCompleted;
            DialogLinesAlreadyCompleted = linesAlreadyCompleted;
            return this;
        }
        public IGameObjectBuilder SetQuest(Quest quest)
        {
            Quest = quest;
            HasQuest = quest != null;
            return this;
        }

        public IGameObjectBuilder AddSpriteRenderer()
        {
            var spriteRenderer = npc.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite("NPCs//Dad/Idle/Idle01");
            return this;
        }

        public IGameObjectBuilder AddAnimator()
        {
            npc.AddComponent<Animator>();
            return this;
        }

        public IGameObjectBuilder AddCollider()
        {
            npc.AddComponent<Collider>();
            return this;
        }

        public Component AddScriptComponent<T>() where T : Component
        {
            return npc.AddComponent<T>();
        }

        public GameObject GetResult()
        {
            var npcComponent = npc.GetComponent<NpcComponent>();

            if(npcComponent != null)
            {
                npcComponent.Name = Name;
                npcComponent.DialogLinesBeforeAccept = DialogLinesBeforeAccept;
                npcComponent.DialogLinesAcceptedNotCompleted = DialogLinesAcceptedNotCompleted;
                npcComponent.DialogLinesOnCompleted = DialogLinesOnCompleted;
                npcComponent.DialogLinesAlreadyCompleted = DialogLinesAlreadyCompleted;
                npcComponent.QuestToGive = Quest;
                npcComponent.Portrait = GameWorld.Instance.Content.Load<Texture2D>($"NPCs/Portraits/Portrait{Name}");
            }

            return npc;
        }
    }
}
