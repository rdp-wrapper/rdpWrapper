using sergiye.Common;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;

namespace rdpWrapper {

  internal class LocalUsersManager: IDisposable {

    private readonly Logger logger;
    private readonly PrincipalContext context;

    public const string RemoteDesktopUsersGroupSid = "S-1-5-32-555";

    public LocalUsersManager(Logger logger) {
      this.logger = logger;
      context = new PrincipalContext(ContextType.Machine);
    }

    public UserPrincipal CreateUserIfNotExist(string userName) {
      if (string.IsNullOrEmpty(userName))
        throw new ArgumentException("The username cannot be empty.");

      var user = UserPrincipal.FindByIdentity(context, userName);
      if (user == null) {
        user = new UserPrincipal(context) {
          Name = userName,
          PasswordNeverExpires = true,
          UserCannotChangePassword = false,
          Enabled = true,
        };
        user.Save();
        logger.Log($"User '{userName}' created.", Logger.StateKind.Info);
      }
      else {
        logger.Log($"User '{userName}' already exists.", Logger.StateKind.Info);
      }
      return user;
    }

    public void SetUserPassword(UserPrincipal user, string password) {

      if (user == null)
        throw new ArgumentNullException(nameof(user));
      if (string.IsNullOrEmpty(password))
        throw new ArgumentNullException(nameof(password));

      user.SetPassword(password);
      user.Save();
      logger.Log($"User '{user.Name}' password changed.", Logger.StateKind.Info);
    }

    public void EnsureUserInRemoteDesktopUsers(UserPrincipal user) {

      if (user == null)
        throw new ArgumentNullException(nameof(user));

      var group = GetRemoteDesktopUserGroup();
      if (!group.Members.Contains(user)) {
        group.Members.Add(user);
        group.Save();
        logger.Log($"User '{user.Name}' added to '{group.Name}' group.", Logger.StateKind.Info);
      }
      else {
        logger.Log($"User '{user.Name}' is already a member of '{group.Name}'.", Logger.StateKind.Info);
      }
    }

    public IEnumerable<string> GetRemoteDesktopUsers() {

      var group = GetRemoteDesktopUserGroup();
      return group.Members.Select(u => u.Name);
    }

    public GroupPrincipal GetRemoteDesktopUserGroup() {
      var remoteDesktopGroupSid = new SecurityIdentifier(RemoteDesktopUsersGroupSid);
      return GroupPrincipal.FindByIdentity(context, IdentityType.Sid, remoteDesktopGroupSid.Value)
        ?? throw new Exception($"Group 'Remote Desktop Users' not found on this machine.");
    }

    public void Dispose() {
      context?.Dispose();
    }
  }
}
