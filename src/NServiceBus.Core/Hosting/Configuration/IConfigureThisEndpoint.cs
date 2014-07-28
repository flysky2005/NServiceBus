﻿namespace NServiceBus
{
    /// <summary>
    /// Indicate that the implementing class will specify configuration.
    /// </summary>
    public interface IConfigureThisEndpoint
    {
        /// <summary>
        /// Allows to override default settings.
        /// </summary>
        /// <param name="builder">Endpoint configuration builder.</param>
// ReSharper disable once UnusedParameter.Global
        void Customize(ConfigurationBuilder builder);
    }
}