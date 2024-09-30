using HazelClinic3.Models;
using System;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;

namespace HazelClinic3
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer Container => container.Value;

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        public static void RegisterTypes(IUnityContainer container)
        {
            // Register your types
            container.RegisterType<AppointmentService>();
            container.RegisterType<DataContext>(); // Assuming DataContext is your EF context

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        /// <summary>
        /// Configures the dependency injection container.
        /// </summary>
        public static void RegisterComponents()
        {
            RegisterTypes(Container);
        }
    }
}
