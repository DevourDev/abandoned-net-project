namespace FD.Units
{
    public enum UnitAllyMode : byte
    {
        /// <summary>
        /// any
        /// </summary>
        None,
        /// <summary>
        /// Side.UniqueID ==
        /// </summary>
        OnePlayer,
        /// <summary>
        /// Side.TeamID ==
        /// </summary>
        OneTeam,
        /// <summary>
        /// Side.TeamID !=
        /// </summary>
        Enemy,
    }

}
