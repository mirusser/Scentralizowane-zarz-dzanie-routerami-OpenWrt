using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouterManagement.Logic.Repositories;
using RouterManagement.Models;
using RouterManagement.Models.Context;
using RouterManagement.Models.ViewModels;

namespace RouterManagement.Logic.Connections
{
    public static class Routers
    {
        private static readonly List<RouterOnListViewModel> routerDataList = new List<RouterOnListViewModel>();

        public static void CreateNewConnection(RouterAccesData routerAccesData)
        {
            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                if (uow.RouterAccesDatasRepository.Any(r => r.Name.Equals(routerAccesData.Name))) return;

                uow.RouterAccesDatasRepository.Add(routerAccesData);

                uow.Save();
            }

            var connection = new SshConnection(routerAccesData);

            var routerData = new RouterOnListViewModel
            {
                IsActive = connection.IsConnected,
                Name = routerAccesData.Name,
                SshConnection = connection
            };

            routerDataList.Add(routerData);
        }

        public static void Initialize()
        {
            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                if (!uow.RouterAccesDatasRepository.Any()) return;

                var sync = new object();
                Parallel.ForEach(uow.RouterAccesDatasRepository.GetAll(), data =>
                {
                    var connection = new SshConnection(data);

                    var routerData = new RouterOnListViewModel
                    {
                        IsActive = connection.IsConnected,
                        Name = data.Name,
                        SshConnection = connection
                    };

                    lock (sync)
                    {
                        routerDataList.Add(routerData);
                    }
                });
            }
        }

        public static List<RouterActivityDataViewModel> GetRoutersAsRouterAccesDataViewModel()
        {
            var allRouters = new List<RouterActivityDataViewModel>();

            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                var sync = new object();
                Parallel.ForEach(uow.RouterAccesDatasRepository.GetAll(), r =>
                {
                    var currentRouter = routerDataList.FirstOrDefault(it => it.Name == r.Name);

                    var toAdd = new RouterActivityDataViewModel
                    {
                        Name = r.Name,
                        IsActive = currentRouter != null && currentRouter.IsActive,
                        RouterIp = r.RouterIp,
                        Port = r.Port,
                        Login = r.Login,
                    };

                    lock (sync)
                    {
                        allRouters.Add(toAdd);
                    }
                });
            }

            return allRouters;
        }

        public static void ReconnectAllRouters()
        {
            routerDataList.Clear();
            Initialize();
        }

        public static SshConnection GetConnectionByName(string name)
        {
            var searchedRouter = routerDataList.FirstOrDefault(it => it.Name == name);
            return searchedRouter?.SshConnection;
        }

        public static RouterAccesData GetRouterAccesDataByName(string name)
        {
            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                return uow.RouterAccesDatasRepository.FirstOrDefault(it => it.Name == name);
            }
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

            var routerToRemove = routerDataList.First(it => it.Name == name);
            routerDataList.Remove(routerToRemove);
        }

        public static IEnumerable<string> GetAllRoutersNames()
        {
            return routerDataList.Select(it => it.Name);
        }

        public static IEnumerable<string> GetOnlineRoutersNames()
        {
            return routerDataList.Where(it => it.IsActive).Select(it => it.Name);
        }

        public static bool CheckIfRouterIsConnected(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var router = routerDataList.FirstOrDefault(it => it.Name == name);
            return router != null && router.IsActive;
        }

        public static string GetFirstRouterName()
        {
            var searchedRouter = routerDataList.FirstOrDefault(it => it.IsActive);

            return searchedRouter?.Name;
        }
    }
}