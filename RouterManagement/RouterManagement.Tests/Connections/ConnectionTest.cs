using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RouterManagement.Logic.Connections;
using RouterManagement.Models;

namespace RouterManagement.Tests.Connections
{
    [TestClass]
    public class ConnectionTest
    {
        [TestMethod]
        public void Connect_Succes()
        {
            var test1 = new SshConnection("192.168.2.1", "root", "konopie1");
            var test2 = new SshConnection("192.168.2.1", 22, "root", "konopie1");

            var ip = IPAddress.Parse("192.168.2.1");
            var test3 = new SshConnection(ip, "root", "konopie1");

            var router = new RouterAccesData
            {
                Login = "root",
                Password = "konopie1",
                RouterIp = ip,
                Port = 22
            };
            var test4 = new SshConnection(router);
        }

        [TestMethod]
        [ExpectedException(typeof(SocketException))]
        public void Connect_Failed()
        {
            var test1 = new SshConnection("192.168.5.1", "root", "konopie1");
            var test2 = new SshConnection("192.168.5.1", "login", "konopie1");
            var test3 = new SshConnection("192.168.2.1", 22, "root", "aaa");
            var test4 = new SshConnection("192.168.2.1", 33, "root", "aaa");
        }

        [TestMethod]
        public void SendAndGetMessage_Succes()
        {
            var test1 = new SshConnection("192.168.2.1", "root", "konopie1");
            var answer = test1.SendCommand("uci show");

            Assert.IsFalse(string.IsNullOrEmpty(answer));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendAndGetMessage_Failed()
        {
            var test1 = new SshConnection("192.168.2.1", "root", "konopie1");
            var answer = test1.SendCommand("cecjnw");

            Assert.IsFalse(string.IsNullOrEmpty(answer));
        }

        [TestMethod]
        public void Send_UciShow_Succes()
        {
            var test1 = new SshConnection("192.168.2.1", "root", "konopie1");
            var answer = test1.Send_UciShow();

            Assert.IsTrue(answer != null);
            Assert.IsTrue(answer.Count > 0);
        }
    }
}
