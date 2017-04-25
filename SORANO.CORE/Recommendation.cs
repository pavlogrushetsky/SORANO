namespace SORANO.CORE
{
    /// <summary>
    /// Recommendation entity which can be attached to any antity
    /// </summary>
    public class Recommendation : StockEntity
    {
        /// <summary>
        /// Unique identifier of the entity to be attached with the recommendation
        /// </summary>
        public int ParentEntityID { get; set; }

        /// <summary>
        /// Recommended value
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Recommendation comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Entity to be attached with the recommendation
        /// </summary>
        public virtual StockEntity ParentEntity { get; set; }
    }
}