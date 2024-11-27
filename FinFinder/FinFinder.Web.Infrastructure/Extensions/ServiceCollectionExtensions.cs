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
                Type repositoryInterface = typeof(IRepository<,>);
                Type repositoryInstanceType = typeof(BaseRepository<,>);

                PropertyInfo idProperty  =modelType.GetProperties()
                    .Where(m=> Attribute
                    .IsDefined(m, typeof(KeyAttribute)))
                    .SingleOrDefault();

                Type[] constructArgs = new Type[2];
                constructArgs[0] = modelType;
                if (idProperty == null)
                {
                    constructArgs[1] = typeof(object);
                   
                }
                else
                {
                    constructArgs[1] = idProperty.PropertyType;
                    
                }
               
                    repositoryInterface = repositoryInterface.MakeGenericType(constructArgs);
                repositoryInstanceType = repositoryInstanceType.MakeGenericType(constructArgs);

                services.AddScoped(repositoryInterface, repositoryInstanceType);


            }
            
        }
    }
}
