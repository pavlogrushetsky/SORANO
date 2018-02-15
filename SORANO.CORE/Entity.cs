namespace SORANO.CORE
{
    public abstract class Entity
    {
        public int ID { get; set; }

        public bool IsDeleted { get; set; }
    }
}