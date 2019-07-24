namespace DCS.Alternative.Launcher.ServiceModel
{
    /// <summary>
    ///     Resolution settings
    /// </summary>
    public sealed class ResolveOptions
    {
        public UnregisteredResolutionActions UnregisteredResolutionAction { get; set; } =
            UnregisteredResolutionActions.AttemptResolve;

        public NamedResolutionFailureActions NamedResolutionFailureAction { get; set; } =
            NamedResolutionFailureActions.Fail;

        /// <summary>
        ///     Gets the default options (attempt resolution of unregistered types, fail on named resolution if name not found)
        /// </summary>
        public static ResolveOptions Default { get; } = new ResolveOptions();

        /// <summary>
        ///     Preconfigured option for attempting resolution of unregistered types and failing on named resolution if name not
        ///     found
        /// </summary>
        public static ResolveOptions FailNameNotFoundOnly { get; } = new ResolveOptions
        {
            NamedResolutionFailureAction = NamedResolutionFailureActions.Fail,
            UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve
        };

        /// <summary>
        ///     Preconfigured option for failing on resolving unregistered types and on named resolution if name not found
        /// </summary>
        public static ResolveOptions FailUnregisteredAndNameNotFound { get; } = new ResolveOptions
        {
            NamedResolutionFailureAction = NamedResolutionFailureActions.Fail,
            UnregisteredResolutionAction = UnregisteredResolutionActions.Fail
        };

        /// <summary>
        ///     Preconfigured option for failing on resolving unregistered types, but attempting unnamed resolution if name not
        ///     found
        /// </summary>
        public static ResolveOptions FailUnregisteredOnly { get; } = new ResolveOptions
        {
            NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution,
            UnregisteredResolutionAction = UnregisteredResolutionActions.Fail
        };
    }
}