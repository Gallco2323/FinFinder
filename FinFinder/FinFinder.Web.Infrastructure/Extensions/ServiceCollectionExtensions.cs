using FinFinder.Data.Models;
using FinFinder.Data.Repository;
using FinFinder.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterRepositories(this IServiceCollection services, Assembly modelAssembly)
        {
            Type[] typesToExclude = new[]
            {
                typeof(ApplicationUser),
               
            };
            Type[] modelTypes = modelAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface && !typesToExclude.Contains(t) && !t.Name.ToLower().EndsWith("attribute"))
                .ToArray();
              
            foreach (var modelType in modelTypes)
            {
                var keyProperties = modelType.GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
            .ToArray();

                if (keyProperties.Length == 1)
                {
                    // Single primary key
                    RegisterSingleKeyRepository(services, modelType, keyProperties[0].PropertyType);
                }
                else if (keyProperties.Length > 1)
                {
                    // Composite primary key
                    RegisterCompositeKeyRepository(services, modelType, keyProperties.Select(p => p.PropertyType).ToArray());
                }
                else
                {
                    // No primary key defined
                    RegisterSingleKeyRepository(services, modelType, typeof(object));
                }

            }
            
        }
        private static void RegisterSingleKeyRepository(IServiceCollection services, Type modelType, Type idType)
        {
            Type repositoryInterface = typeof(IRepository<,>).MakeGenericType(modelType, idType);
            Type repositoryImplementation = typeof(BaseRepository<,>).MakeGenericType(modelType, idType);

            services.AddScoped(repositoryInterface, repositoryImplementation);
        }

        private static void RegisterCompositeKeyRepository(IServiceCollection services, Type modelType, Type[] keyTypes)
        {
            if (keyTypes.Length != 2)
                throw new InvalidOperationException($"CompositeKeyRepository supports exactly 2 key properties. Found {keyTypes.Length} for {modelType.Name}.");

            Type repositoryInterface = typeof(ICompositeKeyRepository<,,>).MakeGenericType(modelType, keyTypes[0], keyTypes[1]);
            Type repositoryImplementation = typeof(CompositeKeyRepository<,,>).MakeGenericType(modelType, keyTypes[0], keyTypes[1]);

            services.AddScoped(repositoryInterface, repositoryImplementation);
        }
    }
}
