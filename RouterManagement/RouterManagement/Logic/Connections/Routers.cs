using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouterManagement.Logic.Repositories;
using RouterManagement.Models;
using RouterManagement.Models.Context;

namespace RouterManagement.Logic.Connections
{
    public static class Routers
    {
        private static Dictionary<string, SshConnection> routersDict = new Dictionary<string, SshConnection>();

        public static void CreateNewConnection(RouterAccesData routerAccesData)
        {
            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                if (uow.RouterAccesDatasRepository.Any(r => r.Name.Equals(routerAccesData.Name))) return;

                uow.RouterAccesDatasRepository.Add(routerAccesData);

                uow.Save();
            }

            var connection = new SshConnection(routerAccesData);

            routersDict.Add(routerAccesData.Name, connection);
        }

        public static void Initialize()
        {
            if (routersDict.Count > 0) routersDict = new Dictionary<string, SshConnection>();

            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                if (uow.RouterAccesDatasRepository.Any())
                {
                    Parallel.ForEach(uow.RouterAccesDatasRepository.GetAll(), data =>
                    {
                        var connection = new SshConnection(data);
                        routersDict.Add(data.Name, connection);
                    });
                }
            }
        }

        public static SshConnection GetConnectionByName(string name)
        {
            SshConnection connection = null;
            routersDict.TryGetValue(name, out connection);
            return connection;
        }

        public static void DeleteConnectionByName(string name)
        {
            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                var router = uow.RouterAccesDatasRepository.Find(r => r.Name.Equals(name)).FirstOrDefault();

                if (router == null) return;

                uow.RouterAccesDatasRepository.Delete(router);

                uow.Save();
            }

            routersDict.Remove(name);
        }

        public static ICollection<string> GetRoutersNames()
        {
            return routersDict.Keys.OrderBy(x => x).ToList();
        }

        public static string GetFirstRouterName()
        {
            return routersDict.FirstOrDefault().Key;
        }
    }
}