namespace SORANO.CORE
{
    /// <summary>
    /// Base abstract class for all the entities of the domain model
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Unique identifier of the entity
        /// </summary>
        public int ID { get; set; }
    }
}