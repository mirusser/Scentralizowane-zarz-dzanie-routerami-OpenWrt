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
    public static class RoutersConnections
    {
        private static readonly List<RouterOnListViewModel> RouterDataList = new List<RouterOnListViewModel>();

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
                        RouterDataList.Add(routerData);
                    }
                });
            }
        }

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

            RouterDataList.Add(routerData);
        }

        public static bool CheckIfRouterIsConnected(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var router = RouterDataList.FirstOrDefault(it => it.Name == name);
            return router != null && router.IsActive;
        }

        public static void ReconnectAllRouters()
        {
            RouterDataList.Clear();
            Initialize();
        }

        public static List<RouterActivityDataViewModel> GetRoutersAsRouterAccesDataViewModel()
        {
            var allRouters = new List<RouterActivityDataViewModel>();

            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                var sync = new object();
                Parallel.ForEach(uow.RouterAccesDatasRepository.GetAll(), r =>
                {
                    var currentRouter = RouterDataList.FirstOrDefault(it => it.Name == r.Name);

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

        public static SshConnection GetConnectionByName(string name)
        {
            var searchedRouter = RouterDataList.FirstOrDefault(it => it.Name == name);
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

            var routerToRemove = RouterDataList.First(it => it.Name == name);
            RouterDataList.Remove(routerToRemove);
        }

        public static IEnumerable<string> GetAllRoutersNames()
        {
            return RouterDataList.Select(it => it.Name);
        }

        public static IEnumerable<string> GetOnlineRoutersNames()
        {
            return RouterDataList.Where(it => it.IsActive).Select(it => it.Name);
        }

        public static string GetFirstRouterName()
        {
            var searchedRouter = RouterDataList.FirstOrDefault(it => it.IsActive);

            return searchedRouter?.Name;
        }
    }
}