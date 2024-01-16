namespace AmbientSoundsTuner2.CommonShared
{
    /// <summary>
    /// An interface that contains the methods for plugin state change events.
    /// To be used by a class that implements IUserMod.
    /// </summary>
    public interface IUserModStateChangeEvents
    {
        /// <summary>
        /// Called when the mod is enabled.
        /// </summary>
        void OnEnabled();

        /// <summary>
        /// Called when the mod is disabled.
        /// </summary>
        void OnDisabled();
    }
}
