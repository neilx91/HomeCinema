using HomeCinema.Data.Extensions;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Services
{
    public class MembershipService : IMembershipService
    {
        #region Variables
        private readonly IEntityBaseRepository<User> _userRepository;
        private readonly IEntityBaseRepository<Role> _roleRepository;
        private readonly IEntityBaseRepository<UserRole> _userRoleRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork; 
        #endregion

        public MembershipService(IEntityBaseRepository<User> userRepository, IEntityBaseRepository<Role> roleRepository, 
            IEntityBaseRepository<UserRole> userRoleRepository, IEncryptionService encryptionService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
        }

        public User CreateUser(string username, string email, string password, int[] roles)
        {
            //first check if the user already exists
            var userExisted = _userRepository.GetSingleByUsername(username);
            if (userExisted != null)
            {
                throw new Exception("User already exists!");
            }
            //then encrypt the password and create the user
            var salt = _encryptionService.CreateSalt();
            var user = new User
            {
                Username = username,
                Salt = salt,
                Email = email,
                IsLocked = false,
                HashedPassword = _encryptionService.EncryptPassword(password, salt),
                DateCreated = DateTime.Now
            };
            _userRepository.Add(user);
            _unitOfWork.Commit();
            //bind the user with the roles, add to UserRole table
            if(roles != null || roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    addUserToRole(user, role);
                }
            }
            return user;
        }

        public User GetUser(int userId)
        {
            return _userRepository.GetSingle(userId);
        }

        public List<Role> GetUserRoles(string username)
        {
            var user = _userRepository.GetSingleByUsername(username);
            List<Role> userRoles = new List<Role>();
            if(user != null)
            {
                foreach (var role in user.UserRoles)
                {
                    userRoles.Add(role.Role);
                }
            }
            return userRoles.Distinct().ToList();
        }

        public MembershipContext ValidateUser(string username, string password)
        {
            var membership = new MembershipContext();
            var user = _userRepository.GetSingleByUsername(username);
            if(user != null && IsUserValid(user, password))
            {
                var userRoles = GetUserRoles(user.Username);
                membership.User = user;
                var identity = new GenericIdentity(user.Username);
                membership.Principal = new GenericPrincipal(identity, userRoles.Select(ur => ur.Name).ToArray());
            }
            return membership;
        }

        #region helper methods
        private void addUserToRole(User user, int roleId)
        {
            var role = _roleRepository.GetSingle(roleId);
            if (role == null)
                throw new ApplicationException("Role doesn't exist.");
            var userRole = new UserRole()
            {
                RoleId = role.ID,
                UserId = user.ID
            };
            _userRoleRepository.Add(userRole);
        }
        private bool IsPasswordValid(User user, string password)
        {
            return string.Equals(_encryptionService.EncryptPassword(password, user.Salt), user.HashedPassword);
        }
        private bool IsUserValid(User user, string password)
        {
            if (IsPasswordValid(user, password))
            {
                return !user.IsLocked;
            }
            return false;
        }
        #endregion
    }

    public interface IMembershipService
    {
        MembershipContext ValidateUser(string username, string password);
        User CreateUser(string username, string email, string password, int[] roles);
        User GetUser(int userId);
        List<Role> GetUserRoles(string username); 
    }
}
