namespace VladislavTsurikov.ComponentStack.Scripts
{
    public interface ITab
    {
        bool Selected
        {
            get; set;
        }
        
        string GetName();
        void SetName(string newName);
    }
}