using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;

namespace RedLetterDays.HubLibraries.HtmlHelpers.Utilities
{
    public static class PermissionsHelpers
    {
        private const string ControllerTypeCacheKey = "ControllerTypeCache";

        /// <summary>
        /// Checks if the current user has permission to run an action.
        /// </summary>
        /// <param name="viewContext">ViewContext where the permission must be checked.</param>
        /// <param name="actionName">Name of the action to be checked.</param>
        /// <param name="controllerName">Name of the controller where the action is located.</param>
        /// <param name="areaName">Name of the area where the controller is located.</param>
        /// <returns>True or false if the user has permission or not.</returns>
        public static bool CheckPermission(ViewContext viewContext, string actionName, string controllerName = null, string areaName = null)
        {
            try
            {
                ControllerBase controllerBase = string.IsNullOrEmpty(controllerName)
                                                    ? viewContext.Controller
                                                    : GetControllerByName(areaName, controllerName, viewContext.RequestContext);
                var controllerContext = new ControllerContext(viewContext.RequestContext, controllerBase);
                ControllerDescriptor controllerDescriptor =
                    new ReflectedControllerDescriptor(controllerContext.Controller.GetType());
                ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

                if (actionDescriptor == null)
                    return false;

                var filters =
                    new FilterInfo(FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor));

                var authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor);
                foreach (IAuthorizationFilter authorizationFilter in filters.AuthorizationFilters)
                {
                    authorizationFilter.OnAuthorization(authorizationContext);
                    if (authorizationContext.Result != null)
                        return false;
                }
                return true;
            }
            catch(Exception exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a controller by name based on the request context.
        /// </summary>
        /// <param name="areaName">The name of the area to be found</param>
        /// <param name="controllerName">The name of the controller to be found.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns>The controller found.</returns>
        private static ControllerBase GetControllerByName(string areaName, string controllerName, RequestContext requestContext)
        {
            IController controller;
            if ((!string.IsNullOrEmpty(areaName)))
            {
                controller = CreateControllerFromArea(controllerName, areaName);
            }
            else
            {
                IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
                controller = factory.CreateController(requestContext, controllerName);

                if (controller == null)
                {
                    throw new InvalidOperationException(String.Format("The IControllerFactory '{0}' did not return a controller for the name '{1}'.", factory, controllerName));
                }    
            }
           
            return (ControllerBase)controller;
        }

        #region Create Controller Helper

        private static IController CreateControllerFromArea(string controllerName, string areaName)
        {
            if (String.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("Value cannot be null or empty", "controllerName");
            }
             
            Type controllerType = GetControllerTypeWithinArea(controllerName, areaName);

            try 
            {
                return DependencyResolver.Current.GetService(controllerType) as IController;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(
                        String.Format("An error occurred when trying to create a controller of type '{0}'. Make sure that the controller has a parameterless public constructor.",
                            controllerType.Namespace),
                        ex);
            }
        }

        private static Type GetControllerTypeWithinArea(string controllerName, string areaName)
        {
            IEnumerable<Type> matchingTypes = GetMatchingType(controllerName, areaName).ToList();

            switch (matchingTypes.Count())
            {
                case 0:
                    return null;

                case 1:
                    return matchingTypes.First();

                default:
                    throw new Exception("Ambiguous Controller Exception");
            }

        }

        private static IEnumerable<Type> GetMatchingType(string controllerName, string areaName)
        {
            var matchingTypes = new List<Type>();
            var controllers = GetControllerTypesDictionary();
            ILookup<string, Type> nsLookup;

            if(controllers.TryGetValue(controllerName, out nsLookup))
            {
                foreach (var nsGroup in nsLookup)
                {
                    if(nsGroup.Key.ToUpper().Contains(areaName.ToUpper()))
                        matchingTypes.AddRange(nsGroup);
                }
            }

            return matchingTypes;
        }

        private static Dictionary<string, ILookup<string, Type>> GetControllerTypesDictionary()
        {
            // Retrieve from cache if exist

            var controllerTypes = GetControllerTypesWithReflection();
            var groupedByName = controllerTypes.GroupBy(
                    t=> t.Name.Substring(0, t.Name.Length - "Controller".Length),
                            StringComparer.OrdinalIgnoreCase);

            var controllerTypesDictionary = groupedByName.ToDictionary(
                            g => g.Key,
                            g => g.ToLookup(t => t.Namespace ?? String.Empty, StringComparer.OrdinalIgnoreCase),
                            StringComparer.OrdinalIgnoreCase);


            return controllerTypesDictionary;
        }

        private static IEnumerable<Type> GetControllerTypesWithReflection()
        {
            ICollection<Type> typesSoFar = Type.EmptyTypes;
            var assemblies = BuildManager.GetReferencedAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] typesInAsm;
                try
                {
                    typesInAsm = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    typesInAsm = ex.Types;
                }
                typesSoFar = typesSoFar.Concat(typesInAsm).ToArray();
            }

            return typesSoFar
                .Where(t => t != null &&
                            t.IsPublic &&
                            !t.IsAbstract &&
                            t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) &&
                            typeof(IController).IsAssignableFrom(t)
                )
                .ToArray();
        }

        #endregion
    }
}
