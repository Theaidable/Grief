using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.Quests
{
    /// <summary>
    /// Superklasse for alle typer af quests
    /// </summary>
    public abstract class Quest
    {
        //Properties
        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public bool IsAccepted { get; protected set; }
        public bool IsCompleted { get; protected set; }

        /// <summary>
        /// Metode til at acceptere en quest
        /// </summary>
        public virtual void Accept()
        {
            IsAccepted = true;
        }

        /// <summary>
        /// metode til at fuldføre en quest
        /// </summary>
        public virtual void Complete()
        {
            IsCompleted = true;
        }

        //Metoder som skal tilføjes at nedarvninger
        public abstract bool CanComplete(InventoryComponent inventory);
        public abstract void GrantReward(InventoryComponent inventory);
    }
}
