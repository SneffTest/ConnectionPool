using ConnectionPool.Common.Exceptions;
using ConnectionPool.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Oracle.ManagedDataAccess.Client;
using System;

namespace ConnectionPool.Test
{
    [TestFixture]
    public class ConnectionPool_Tests
    {
        private ServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public void Init()
        {
            var connectionFactory= new Mock<IConnectionFactory<Connection, string, OracleConnection>>();
            connectionFactory.Setup(x => x.CreateConnection()).Returns(() =>
            {
                return new Connection()
                {
                    ConnID = Guid.NewGuid().ToString()
                };
            });

            _serviceProvider = new ServiceCollection()
              .AddLogging()
              .AddSingleton<IConnectionPool<Connection, string, OracleConnection>, ConnectionPool>()
              .AddSingleton<IConnectionFactory<Connection, string, OracleConnection>>(connectionFactory.Object)
              .BuildServiceProvider();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void OpenConnections(int connectionCount)
        {
            var connPool = _serviceProvider.GetService<IConnectionPool<Connection, string, OracleConnection>>();
            var count = connPool.GetConnectionCount();

            for (int i = 0; i < connectionCount; i++)
            {
                connPool.checkOutNew();
            }

            Assert.That(count + connectionCount, Is.EqualTo(connPool.GetConnectionCount()));
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void OpenAndCloseConnections(int connectionCount)
        {
            var connPool = _serviceProvider.GetService<IConnectionPool<Connection, string, OracleConnection>>();
            var count = connPool.GetConnectionCount();

            for (int i = 0; i < connectionCount; i++)
            {
                var conn = connPool.checkOutNew();
                connPool.checkIn(conn);
                connPool.CloseAndRemoveConn(conn);
            }

            Assert.That(count, Is.EqualTo(connPool.GetConnectionCount()));
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void SimulateUsageConnections(int connectionCount)
        {
            var connPool = _serviceProvider.GetService<IConnectionPool<Connection, string, OracleConnection>>();
            var count = connPool.GetConnectionCount();

            System.Collections.Generic.List<Connection> connList = new System.Collections.Generic.List<Connection>();
            for (int i = 0; i < connectionCount; i++)
            {
                var conn = connPool.checkOutNew();
                connPool.checkIn(conn);
                connList.Add(conn);
            }

            foreach(var conn in connList)
            {
                connPool.checkOut(conn.ConnID);
                connPool.checkIn(conn);
            }

            foreach (var conn in connList)
            {
                connPool.CloseAndRemoveConn(conn);
            }

            Assert.That(count, Is.EqualTo(connPool.GetConnectionCount()));
        }

        [Test]
        [TestCase("dummyConnectionID1")]
        [TestCase("dummyConnectionID2")]
        [TestCase("dummyConnectionID3")]
        public void CheckInNotExistendConnections(string connID)
        {
            Assert.Throws<ConnectionPoolNotFoundException>(() =>
            {
                var connPool = _serviceProvider.GetService<IConnectionPool<Connection, string, OracleConnection>>();
                connPool.checkIn(new Connection() { ConnID = connID });
            });
        }

        [Test]
        [TestCase("dummyConnectionID1")]
        [TestCase("dummyConnectionID2")]
        [TestCase("dummyConnectionID3")]
        public void CheckOutNotExistendConnections(string connID)
        {
            Assert.Throws<ConnectionPoolNotFoundException>(() =>
            {
                var connPool = _serviceProvider.GetService<IConnectionPool<Connection, string, OracleConnection>>();
                connPool.checkOut(connID);
            });
        }
    }
}