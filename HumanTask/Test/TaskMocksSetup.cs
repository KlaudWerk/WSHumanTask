using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using KlaudWerk.Security;
using KlaudWerk.Security.Claims;
using Microsoft.IdentityModel.Claims;
using Moq;

namespace HumanTask.Test
{

    public class TaskMocksSetup
    {
        [ThreadStatic]
        private static IPrincipal _principal;


        /// <summary>
        /// Setups the mock principal.
        /// </summary>
        /// <param name="identityName">Name of the identity.</param>
        /// <param name="mockId">The id of the identity</param>
        /// <param name="setRole">The set role.</param>
        /// <param name="roleCheck">The role check.</param>
        /// <returns></returns>
        protected IClaimsPrincipal SetupMockPrincipal(
            string identityName,
            IdentityId mockId,
            Action<string> setRole,
            Func<bool> roleCheck)
        {
            return SetupMockPrincipal(identityName, new[] {new Claim(ClaimType.MappedId.ToString(), mockId.ToString())},
                                      setRole, roleCheck);
        }

        /// <summary>
        /// Setups the mock principal.
        /// </summary>
        /// <param name="identityName">Name of the identity.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="setRole">The set role.</param>
        /// <param name="roleCheck">The role check.</param>
        /// <returns></returns>
        protected IClaimsPrincipal SetupMockPrincipal(
            string identityName,
            IEnumerable<Claim> claims,
            Action<string> setRole,
            Func<bool> roleCheck)
        {
            Mock<IClaimsIdentity> mockIdentity = new Mock<IClaimsIdentity>();
            mockIdentity.SetupGet(i => i.IsAuthenticated).Returns(true);
            mockIdentity.SetupGet(i => i.Name).Returns(identityName);
            mockIdentity.SetupGet(i => i.AuthenticationType).Returns("MOCK");
            ClaimCollection claimCollection = new ClaimCollection(mockIdentity.Object);
            foreach (Claim claim in claims)
            {
                claimCollection.Add(claim);
            }
            mockIdentity.SetupGet(i => i.Claims).Returns(() => claimCollection);
            Mock<IClaimsPrincipal> mockPrincipal = new Mock<IClaimsPrincipal>(MockBehavior.Default);
            mockPrincipal.SetupGet(p => p.Identity).Returns(mockIdentity.Object);
            mockPrincipal.Setup(p => p.IsInRole(It.IsAny<string>())).Callback<string>(setRole.Invoke).Returns(roleCheck.Invoke);
            return mockPrincipal.Object;
        }

        /// <summary>
        /// Setups the mock principal.
        /// </summary>
        /// <param name="identityName">Name of the identity.</param>
        /// <param name="mockId">The mock id.</param>
        /// <param name="setRole">The set role.</param>
        /// <param name="roleCheck">The role check.</param>
        /// <returns></returns>
        protected IPrincipal SetupMockGroupPrincipal(
            string identityName,
            IdentityId mockId,
            Action<string> setRole,
            Func<bool> roleCheck)
        {
            return SetupMockPrincipal(identityName, new[]
                                                        {
                                                            new Claim(ClaimType.MappedId.ToString(), mockId.ToString()),
                                                            new Claim(ClaimType.AccountType.ToString(),ClaimValue.GroupAccountType), 
                                                        },
                          setRole, roleCheck);

        }

        /// <summary>
        /// Creates the task mock.
        /// </summary>
        /// <returns></returns>
        protected virtual Task SetupTaskMock(IIdentity initiator,Action<Mock<Task>> onSetup)
        {
            return SetupTaskMock(initiator, TaskStatus.Created, null, onSetup);
        }

        /// <summary>
        /// Setups the task mock.
        /// </summary>
        /// <param name="initiator">The initiator.</param>
        /// <param name="status">The status.</param>
        /// <param name="actualOwner">The actual owner.</param>
        /// <param name="onSetup">The on setup.</param>
        /// <returns></returns>
        protected virtual Task SetupTaskMock(IIdentity initiator, TaskStatus status, 
                                              IIdentity actualOwner, 
                                              Action<Mock<Task>> onSetup)
        {
            Mock<Task> mt = new Mock<Task>(new TaskId(), status, "Test Task", "Test Task",
                                           Priority.Normal, false, DateTime.UtcNow, initiator, null, null, actualOwner)
                                {
                                    CallBase = true
                                };
            if (onSetup != null)
                onSetup.Invoke(mt);
            return mt.Object;
        }

        /// <summary>
        /// Gets the principal.
        /// </summary>
        /// <value>The principal.</value>
        private static IPrincipal Principal
        {
            get { return _principal; }
        }

        /// <summary>
        /// Setups the logger.
        /// </summary>
        /// <param name="history">The history.</param>
        /// <returns></returns>
        protected virtual ILoggingService SetupLoggerMock(List<TaskHistoryEvent> history)
        {
            Mock<ILoggingService> mockLogService = new Mock<ILoggingService>();
            mockLogService.Setup(s => s.LogHistoryEntry(It.IsAny<TaskHistoryEvent>())).
                Callback<TaskHistoryEvent>(history.Add
                );
            mockLogService.SetupGet(s => s.History).Returns(()=>history);
            return mockLogService.Object;
        }

        protected virtual void SetLoggedUser(IPrincipal principal)
        {
            _principal = principal;
        }

        protected virtual IPrincipal GetLoggedPrincipal()
        {
            return _principal;
        }
    }
}