using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using KlaudWerk.Common;
using KlaudWerk.Security.Claims;
using Microsoft.IdentityModel.Claims;
using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using ClaimsIdentity = KlaudWerk.Security.Claims.ClaimsIdentity;
using ClaimsPrincipal = KlaudWerk.Security.Claims.ClaimsPrincipal;

namespace KlaudWerk.Security.Test
{
    [TestFixture]
    public class ClaimsIdentityTest
    {
        private IWindsorContainer _container;
        [SetUp]
        public void Setup()
        {
            WindsorServiceLocator locator = new WindsorServiceLocator(_container = new WindsorContainer());
            ServiceLocator.SetLocatorProvider(() => locator); 
        }
        [Test]
        public void TestIdentityIdIEquatableNotEqual()
        {
            IdentityId one=new IdentityId();    
            IdentityId two=new IdentityId();
            Assert.IsFalse(one.Equals(two));
            Assert.IsFalse(two.Equals(one));
            Assert.IsFalse(one==two);
            Assert.IsFalse(two==one);
            Assert.IsTrue(one!=two);
            Assert.IsTrue(two != one);
        }

        [Test]
        public void TestIdentityIdIEquatableEqual()
        {
            Guid id = Guid.NewGuid();
            IdentityId one = new IdentityId(id);
            IdentityId two = new IdentityId(id);
            Assert.IsTrue(one.Equals(two));
            Assert.IsTrue(two.Equals(one));
            Assert.IsTrue(one == two);
            Assert.IsTrue(two == one);
            Assert.IsFalse(one != two);
            Assert.IsFalse(two != one);
        }

        #region Test Identity Surrogate and Principal Extensions
        [Test]
        public void TestCreateSurrogateGetId()
        {
            IdentityId id=new IdentityId();
            IdentitySurrogate surrogate=new IdentitySurrogate(id);
            IEnumerable<string> values =surrogate.GetValuesOfClaim(ClaimType.MappedId.ToString());
            Assert.IsNotNull(values);
            Assert.AreEqual(1,values.Count());
            Assert.AreEqual(id.ToString(),values.ElementAt(0));
        }

        [Test]
        public void TestIdentitySurrogateCheckGroupIdentityUserAccount()
        {
            IdentityId id = new IdentityId();
            Mock<IAccountFactory> mock=new Mock<IAccountFactory>(MockBehavior.Strict);
            mock.Setup(m => m.GetAccount(It.IsAny<IdentityId>())).
                Returns(() => new Account(id, "nativeId-user", "test", AccountType.User)).Verifiable();
            _container.Register(Component.For(typeof(IAccountFactory)).UsingFactoryMethod(() => mock.Object));
            IdentitySurrogate surrogate = new IdentitySurrogate(id);
            Assert.IsFalse(surrogate.IsGroupIdentity());
            mock.Verify(m => m.GetAccount(It.IsAny<IdentityId>()),Times.Once());
        }

        [Test]
        public void TestIdentitySurrogateCheckGroupIdentityGroupAccount()
        {
            IdentityId id = new IdentityId();
            Mock<IAccountFactory> mock = new Mock<IAccountFactory>(MockBehavior.Strict);
            mock.Setup(m => m.GetAccount(It.IsAny<IdentityId>())).
                Returns(() => new Account(id, "nativeId-group", "test", AccountType.Group)).Verifiable();
            _container.Register(Component.For(typeof(IAccountFactory)).UsingFactoryMethod(() => mock.Object));
            IdentitySurrogate surrogate = new IdentitySurrogate(id);
            Assert.IsTrue(surrogate.IsGroupIdentity());
            mock.Verify(m => m.GetAccount(It.IsAny<IdentityId>()), Times.Once());
        }
        #endregion

        #region Test Principal Extensions methods
        [Test]
        public void TestIdentityGetMappedIdFromSurrogate()
        {
            IdentityId id = new IdentityId();
            Mock<IAccountFactory> mock = new Mock<IAccountFactory>(MockBehavior.Strict);
            mock.Setup(m => m.GetAccount(It.IsAny<IdentityId>())).
                Returns(() => new Account(id, "nativeId-group", "test", AccountType.Group)).Verifiable();
            _container.Register(Component.For(typeof(IAccountFactory)).UsingFactoryMethod(() => mock.Object));
            IdentitySurrogate surrogate = new IdentitySurrogate(id);
            IdentityId mappedId = surrogate.GetMappedId();
            Assert.IsNotNull(mappedId);
            Assert.AreEqual(id,mappedId);
            mock.Verify(m => m.GetAccount(It.IsAny<IdentityId>()), Times.Never());
        }

        [Test]
        public void TestIdentityGetMappedIdFromClaimsIdentity()
        {
            IdentityId id = new IdentityId();
            Mock<IAccountFactory> mock = new Mock<IAccountFactory>(MockBehavior.Strict);
            mock.Setup(m => m.GetAccount(It.IsAny<IdentityId>())).
                Returns(() => new Account(id, "nativeId-group", "test", AccountType.Group)).Verifiable();
            _container.Register(Component.For(typeof(IAccountFactory)).UsingFactoryMethod(() => mock.Object));
            IClaimsIdentity surrogate = new ClaimsIdentity(id);
            IdentityId mappedId = surrogate.GetMappedId();
            Assert.IsNotNull(mappedId);
            Assert.AreEqual(id, mappedId);
            mock.Verify(m => m.GetAccount(It.IsAny<IdentityId>()), Times.Never());
        }

        [Test]
        public void TestIdentitySurrogateGetRoles()
        {
            IdentityId id = new IdentityId();
            Mock<IAccountFactory> mock = new Mock<IAccountFactory>(MockBehavior.Strict);
            mock.Setup(m => m.GetAccount(It.IsAny<IdentityId>())).
                Returns(() => new Account(id, "nativeId-group", "test", AccountType.Group)).Verifiable();
            mock.Setup(m => m.GetRoles(It.IsAny<IdentityId>())).
                Returns(() => new[]
                                  {
                                      new Account(new IdentityId(), "nativeId-ROLE-1", "role1", AccountType.Role),
                                      new Account(new IdentityId(), "nativeId-ROLE-2", "role2", AccountType.Role),
                                      new Account(new IdentityId(), "nativeId-ROLE-3", "role3", AccountType.Role),
                                      new Account(new IdentityId(), "nativeId-ROLE-4", "role4", AccountType.Role)
                                     }).Verifiable();
            mock.Setup(m=>m.GetGroups(It.IsAny<IdentityId>())).Verifiable();
            _container.Register(Component.For(typeof(IAccountFactory)).UsingFactoryMethod(() => mock.Object));
            IdentitySurrogate surrogate = new IdentitySurrogate(id);
            IEnumerable<IIdentity> roles = surrogate.InRoles();
            Assert.IsNotNull(roles);
            Assert.AreEqual(4,roles.Count());
            foreach (IIdentity identity in roles)
            {
                Assert.IsTrue(identity is IdentitySurrogate);   
            }
            mock.Verify(m => m.GetRoles(It.IsAny<IdentityId>()), Times.Once());
            mock.Verify(m => m.GetAccount(It.IsAny<IdentityId>()), Times.Never());
            mock.Verify(m => m.GetGroups(It.IsAny<IdentityId>()), Times.Never());
        }

        [Test]
        public void TestClaimsIdentityGetRoles()
        {
            IdentityId id = new IdentityId();
            Mock<IAccountFactory> mock = new Mock<IAccountFactory>(MockBehavior.Strict);
            mock.Setup(m => m.GetAccount(It.IsAny<IdentityId>())).
                Returns(() => new Account(id, "nativeId-group", "test", AccountType.Group)).Verifiable();
            mock.Setup(m => m.GetRoles(It.IsAny<IdentityId>())).Verifiable();
            _container.Register(Component.For(typeof(IAccountFactory)).UsingFactoryMethod(() => mock.Object));
            IClaimsIdentity claimsIdentity = new ClaimsIdentity(id);
            claimsIdentity.Claims.Add(new Claim(ClaimType.Role.ToString(),new IdentityId().ToString()));
            claimsIdentity.Claims.Add(new Claim(ClaimType.Role.ToString(), new IdentityId().ToString()));
            claimsIdentity.Claims.Add(new Claim(ClaimType.Role.ToString(), new IdentityId().ToString()));
            claimsIdentity.Claims.Add(new Claim(ClaimType.Role.ToString(), new IdentityId().ToString()));
            IEnumerable<IIdentity> roles = claimsIdentity.InRoles();
            Assert.IsNotNull(roles);
            Assert.AreEqual(4, roles.Count());
            foreach (IIdentity identity in roles)
            {
                Assert.IsTrue(identity is IdentitySurrogate);
            }
            mock.Verify(m => m.GetRoles(It.IsAny<IdentityId>()), Times.Never());
            mock.Verify(m => m.GetAccount(It.IsAny<IdentityId>()), Times.Never());

        }

        [Test]
        public void TestIdentitySurrogateGetGroups()
        {
            IdentityId id = new IdentityId();
            Mock<IAccountFactory> mock = new Mock<IAccountFactory>(MockBehavior.Strict);
            mock.Setup(m => m.GetAccount(It.IsAny<IdentityId>())).
                Returns(() => new Account(id, "nativeId-group", "test", AccountType.Group)).Verifiable();
            mock.Setup(m => m.GetGroups(It.IsAny<IdentityId>())).
                Returns(() => new[]
                                  {
                                      new Account(new IdentityId(), "nativeId-ROLE-1", "role1", AccountType.Group),
                                      new Account(new IdentityId(), "nativeId-ROLE-2", "role2", AccountType.Group),
                                      new Account(new IdentityId(), "nativeId-ROLE-3", "role3", AccountType.Group),
                                      new Account(new IdentityId(), "nativeId-ROLE-4", "role4", AccountType.Group)
                                     }).Verifiable();
            mock.Setup(m => m.GetRoles(It.IsAny<IdentityId>())).Verifiable();
            _container.Register(Component.For(typeof(IAccountFactory)).UsingFactoryMethod(() => mock.Object));
            IdentitySurrogate surrogate = new IdentitySurrogate(id);
            IEnumerable<IIdentity> groups = surrogate.MemberOf();
            Assert.IsNotNull(groups);
            Assert.AreEqual(4, groups.Count());
            foreach (IIdentity identity in groups)
            {
                Assert.IsTrue(identity is IdentitySurrogate);
            }
            mock.Verify(m => m.GetRoles(It.IsAny<IdentityId>()), Times.Never());
            mock.Verify(m => m.GetAccount(It.IsAny<IdentityId>()), Times.Never());
            mock.Verify(m => m.GetGroups(It.IsAny<IdentityId>()), Times.Once());
        }

        [Test]
        public void TestClaimsIdentityGetGroups()
        {
            IdentityId id = new IdentityId();
            Mock<IAccountFactory> mock = new Mock<IAccountFactory>(MockBehavior.Strict);
            mock.Setup(m => m.GetAccount(It.IsAny<IdentityId>())).
                Returns(() => new Account(id, "nativeId-group", "test", AccountType.Group)).Verifiable();
            mock.Setup(m => m.GetGroups(It.IsAny<IdentityId>())).Verifiable();
            mock.Setup(m => m.GetRoles(It.IsAny<IdentityId>())).Verifiable();
            _container.Register(Component.For(typeof(IAccountFactory)).UsingFactoryMethod(() => mock.Object));
            IClaimsIdentity claimsIdentity = new ClaimsIdentity(id) ;
            claimsIdentity.Claims.Add(new Claim(ClaimType.Group.ToString(), new IdentityId().ToString()));
            claimsIdentity.Claims.Add(new Claim(ClaimType.Group.ToString(), new IdentityId().ToString()));
            claimsIdentity.Claims.Add(new Claim(ClaimType.Group.ToString(), new IdentityId().ToString()));
            claimsIdentity.Claims.Add(new Claim(ClaimType.Group.ToString(), new IdentityId().ToString()));
            IEnumerable<IIdentity> groups = claimsIdentity.MemberOf();
            Assert.IsNotNull(groups);
            Assert.AreEqual(4, groups.Count());
            foreach (IIdentity identity in groups)
            {
                Assert.IsTrue(identity is IdentitySurrogate);
            }
            mock.Verify(m => m.GetRoles(It.IsAny<IdentityId>()), Times.Never());
            mock.Verify(m => m.GetAccount(It.IsAny<IdentityId>()), Times.Never());
            mock.Verify(m => m.GetGroups(It.IsAny<IdentityId>()), Times.Never());

        }


        #endregion

        [Test]
        public void TestCreateClaimsIdentity()
        {
            Mock<IAccountFactory> mock=new Mock<IAccountFactory>();
            mock.Setup(f => f.GetMappedAccountId(It.IsAny<string>(), It.IsAny<string>())).Returns(
               ()=> new IdentityId(Guid.NewGuid()));
            WindowsIdentity winIdentity = WindowsIdentity.GetCurrent();
            WindowsIdentityTransformer transformer=new WindowsIdentityTransformer(mock.Object);
            IClaimsIdentity claimsIdentity = transformer.Transform(winIdentity);
            Assert.IsNotNull(claimsIdentity);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            Thread.CurrentPrincipal = claimsPrincipal;
        }
        [Test]
        public void TestCreateClaimsIdentityWithWindowsAccountFactory()
        {
            WindowsIdentity winIdentity = WindowsIdentity.GetCurrent();
            WindowsIdentityTransformer transformer = new WindowsIdentityTransformer(new WindowsAccountFactory(
                winIdentity.AuthenticationType, 
                new MockStorageInteraction()));
            IClaimsIdentity claimsIdentity = transformer.Transform(winIdentity);
            Assert.IsNotNull(claimsIdentity);
        }
        
        private class MockStorageInteraction:IAccountStorageInteraction
        {
            private readonly Dictionary<string ,IdentityId> _store=new Dictionary<string, IdentityId>();
            /// <summary>
            /// Maps the account.
            /// </summary>
            /// <param name="authType">Type of the auth.</param>
            /// <param name="nativeId">The native id.</param>
            /// <param name="accountName">Name of the account.</param>
            /// <param name="displayName">The display name.</param>
            /// <returns></returns>
            public IdentityId MapAccount(string authType, string nativeId, string accountName, string displayName)
            {
                IdentityId id=new IdentityId();
                _store[nativeId] = id;
                return id;
            }

            /// <summary>
            /// Gets the mapped account.
            /// </summary>
            /// <param name="authType">Type of the auth.</param>
            /// <param name="nativeId">The native id.</param>
            /// <returns></returns>
            public IdentityId GetMappedAccount(string authType, string nativeId)
            {
                IdentityId id;
                return _store.TryGetValue(nativeId, out id) ? id : null;
            }

            /// <summary>
            /// Gets the list of roles for the account.
            /// </summary>
            /// <param name="mappedId">The mapped id.</param>
            /// <returns></returns>
            public IEnumerable<Account> GetRoles(IdentityId mappedId)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the flat list of groups in which the account has membership.
            /// </summary>
            /// <param name="mappedId">The mapped id.</param>
            /// <returns></returns>
            public IEnumerable<Account> GetGroups(IdentityId mappedId)
            {
                throw new NotImplementedException();
            }
        }
    }
}
