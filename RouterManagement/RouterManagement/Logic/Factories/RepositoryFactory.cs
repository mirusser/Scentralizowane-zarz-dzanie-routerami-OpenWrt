using System;
using System.Collections.Generic;
using System.Linq;
using RouterManagement.Logic.Factories.Interfaces;
using RouterManagement.Models.Context;

namespace RouterManagement.Logic.Factories
{
    public class RepositoryFactory : Factory, IRepositoryFactory
    {
        private readonly Dictionary<Type, object> initializedRepositories = new Dictionary<Type, object>();

        /// <summary>
        /// Get repository instance
        /// </summary>
        /// <typeparam name="T">Repository class</typeparam>
        /// <returns></returns>
        public override T Get<T>()
        {
            var repository = initializedRepositories.SingleOrDefault(r => r.Key == typeof(T)).Value;

            if (repository == null)
            {
                repository = (T)Activator.CreateInstance(typeof(T), null);
                initializedRepositories.Add(typeof(T), repository);
            }

            return (T)repository;
        }

        public T Get<T>(RouterManagementEntities context)
        {
            var repository = initializedRepositories.SingleOrDefault(r => r.Key == typeof(T)).Value;

            if (repository == null)
            {
                repository = (T)Activator.CreateInstance(typeof(T), context);
                initializedRepositories.Add(typeof(T), repository);
            }

            return (T)repository;
        }
    }
}