namespace DevourDev.Database.Interfaces
{
    public enum ReplaceEntityMode
    {
        /// <summary>
        /// value = requestedValue.
        /// </summary>
        Full,
        /// <summary>
        /// value.contentA = requestedValue.contentA.
        /// Поля, отсутствующие в requestedValue, не
        /// изменяются.
        /// </summary>
        OnlyRequestingContent,
    }
}
