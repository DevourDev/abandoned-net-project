namespace DevourDev.Database.Interfaces
{
    public enum GetEntityMode
    {
        /// <summary>
        /// Response should contain every requested entity.     
        /// Otherwise - response failure.
        /// </summary>
        AllOrNothing,
        /// <summary>
        /// Response contains available entities (from requested list).
        /// If nothing available - response failure.
        /// </summary>
        AllAvailable,
    }
}
