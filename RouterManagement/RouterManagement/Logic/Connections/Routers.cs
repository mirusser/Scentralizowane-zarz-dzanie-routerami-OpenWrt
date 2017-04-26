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
        private static List<RouterOnListViewModel> routerDataList = new List<RouterOnListViewModel>();

        public static void CreateNewConnection(RouterAccesData routerAccesData)
        {
            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                if (uow.RouterAccesDatasRepository.Any(r => r.Name.Equals(routerAccesData.Name))) return;

                uow.RouterAccesDatasRepository.Add(routerAccesData);

                uow.Save();
            }

            var connection = new SshConnection(routerAccesData);

            RouterOnListViewModel routerData = new RouterOnListViewModel();
            routerData.IsActive = connection.IsConnected();
            routerData.Name = routerAccesData.Name;
            routerData.SshConnection = connection;
        }

        public static void Initialize()
        {
            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                if (uow.RouterAccesDatasRepository.Any())
                {
                    Parallel.ForEach(uow.RouterAccesDatasRepository.GetAll(), data =>
                    {
                        var connection = new SshConnection(data);

                        RouterOnListViewModel routerData = new RouterOnListViewModel();
                        routerData.IsActive = connection.IsConnected();
                        routerData.Name = data.Name;
                        routerData.SshConnection = connection;
                    });
                }
            }
        }

        public static List<RouterActivityDataViewModel> GetRoutersAsRouterAccesDataViewModel()
        {
            var allRouters = new List<RouterActivityDataViewModel>();

            using (var uow = new DataContextUoW(new RouterManagementEntities()))
            {
                var allRoutersFromDb = uow.RouterAccesDatasRepository.GetAll();
                foreach (var r in allRoutersFromDb)
                {
                    allRouters.Add(new RouterActivityDataViewModel
                    {
                        Name = r.Name,
                        IsActive = (routerDataList.FirstOrDefault(it =>it.Name == r.Name) == null) ? false : true,
                        RouterIp = r.RouterIp,
                        Port = r.Port,
                        Login = r.Login,
                        Password = r.Password
                    });
                }
            }

            return allRouters;
        }

        public static void UpdateRoutersInfo()
        {
            routerDataList.Clear();
            Initialize();
        }

        public static SshConnection GetConnectionByName(string name)
        {
            var searchedRouter = routerDataList.FirstOrDefault(it => it.Name == name);
            if(searchedRouter != null)
            {
                return searchedRouter.SshConnection;
            }
            else
            {
                return null;
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

            var routerToRemove = routerDataList.FirstOrDefault(it => it.Name == name);
            routerDataList.Remove(routerToRemove);
        }

        public static ICollection<string> GetRoutersNames()
        {
            return routerDataList.Select(it => it.Name).OrderBy(x => x).ToList();
        }

        public static string GetFirstRouterName()
        {
            var searchedRouter = routerDataList.FirstOrDefault(it => it.IsActive == true);

            if(searchedRouter == null)
            {
                return null;
            }
            else
            {
                return searchedRouter.Name;
            }
        }
    }
}