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
            var test1 = new SshConnection("192.168.1.1", "root", "konopie");
            var test2 = new SshConnection("192.168.1.1", 22, "root", "konopie");

            var test3 = new SshConnection("192.168.1.1", "root", "konopie");

            var router = new RouterAccesData
            {
                Login = "root",
                Password = "konopie",
                RouterIp = "192.168.1.1",
                Port = 22
            };
            var test4 = new SshConnection(router);
        }

        [TestMethod]
        [ExpectedException(typeof(SocketException))]
        public void Connect_Failed()
        {
            var test1 = new SshConnection("192.168.5.1", "root", "konopie");
            var test2 = new SshConnection("192.168.5.1", "login", "konopie");
            var test3 = new SshConnection("192.168.1.1", 22, "root", "aaa");
            var test4 = new SshConnection("192.168.1.1", 33, "root", "aaa");
        }

        [TestMethod]
        public void SendAndGetMessage_Succes()
        {
            var test1 = new SshConnection("192.168.1.1", "root", "konopie");
            var answer = test1.Send_CustomCommand("uci show");

            Assert.IsFalse(string.IsNullOrEmpty(answer));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendAndGetMessage_Failed()
        {
            var test1 = new SshConnection("192.168.1.1", "root", "konopie");
            var answer = test1.Send_CustomCommand("cecjnw");

            Assert.IsFalse(string.IsNullOrEmpty(answer));
        }

        [TestMethod]
        public void Get_FullConfiguration_Succes()
        {
            var test1 = new SshConnection("192.168.1.1", "root", "konopie");
            var answer = test1.Get_FullConfiguration();

            Assert.IsTrue(answer != null);
            Assert.IsTrue(answer.Count > 0);
        }

        [TestMethod]
        public void Get_WirelessWireless_Succes()
        {
            var test1 = new SshConnection("192.168.1.1", "root", "konopie");
            var answer = test1.Get_Wireless();

            Assert.IsTrue(answer != null);
            Assert.IsTrue(answer.Count > 0);
        }
    }
}
