using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.Quests
{
    public abstract class Quest
    {
        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public bool IsAccepted { get; protected set; }
        public bool IsCompleted { get; protected set; }

        public virtual void Accept()
        {
            IsAccepted = true;
        }
        public virtual void Complete()
        {
            IsCompleted = true;
        }

        public abstract bool CanComplete(InventoryComponent inventory);
        public abstract void GrantReward(InventoryComponent inventory);
    }
}
