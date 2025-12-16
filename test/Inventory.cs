namespace test
{
    public class Inventory
    {
        public bool HasKey { get; private set; } = false;

        public void AddKey()
        {
            HasKey = true;
        }

        public void RemoveKey()
        {
            HasKey = false;
        }

        
    }
}