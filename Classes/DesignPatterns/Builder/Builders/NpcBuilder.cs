using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Builder.Builders
{
    public class NpcBuilder : IGameObjectBuilder
    {
        private GameObject npc;

        public string Name { get; private set; }
        public List<string> Dialog { get; private set; }
        public Quest Quest { get; private set; }
        public bool HasQuest { get; private set; }
        public bool QuestGiven { get; private set; }

        public NpcBuilder()
        {
            npc = new GameObject();
            npc.AddComponent<NpcComponent>();
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
        public IGameObjectBuilder SetDialog(List<string> lines)
        {
            Dialog = lines;
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
            npc.AddComponent<SpriteRenderer>();
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

        public IGameObjectBuilder SetTag(string tag)
        {
            npc.Tag = tag;
            return this;
        }

        public GameObject GetResult()
        {
            return npc;
        }
    }
}
