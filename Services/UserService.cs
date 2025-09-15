using Web.DTOs;
using Web.Interfaces;
using Web.Models;
using Microsoft.AspNetCore.Identity;
using Web.Exceptions;
using Web.Utilities;

namespace Web.Services
{

    public class UserService : IUserService
    {

        private readonly IUserRepository _db;

        public UserService(IUserRepository user, IEmailService _emailService)
        {

            _db = user;
   
        }

        public async Task<Guid> CreateUserAsync(RegisterDto user)
        {
            
            User userdata = new User
            {

                Email = user.Email

            };

            userdata.Password = new PasswordHasher<User>().HashPassword(userdata, user.Password);
            userdata.Surname = user.Surname;
            userdata.IdNumber = HashHelper.HashId(user.IdNumber);
            userdata.FirstName = user.FirstName;
            
            
            var userExist = await _db.GetUserByIdNumber(userdata.IdNumber);

            if (userExist != null)
                throw new UserAlreadyExistException("User already exist");

            await _db.CreateAsync(userdata);

            return userdata.Id;

        }

        public async Task<User> GetUserAsync(LoginDtos login)
        {

            var user = await _db.GetAsync(login.customNumber);

            if (user == null)
                throw new UserNotFoundException("User does not exist");

             if (user.isVerified == false)
                throw new UserNotActivatedException("User did not verify email");


            PasswordVerificationResult results = new PasswordHasher<User>().
                                                 VerifyHashedPassword(new User { Email = user.Email},
                                                 user.Password, login.password);

            if (results == PasswordVerificationResult.Failed)
                throw new InvalidCredentialsException("Invalid Credentials");

            return user;

        }

        public async Task<User> GetUserById(Guid userId)
        {
            
            var userInfo = await _db.GetUserById(userId);

            if (userInfo == null)
                throw new UserNotFoundException("User does not exist");

            return userInfo;

        } 

        public async Task UpdateUserNumberAsync(string customNumber, User user)
        {

            user.customNumber = customNumber;
            user.isVerified = true;

            await _db.UpdateAsync(user);

        }


        public async Task UpdateUserAsync(User user)
        {

            var userExist = await _db.GetAsync(user.customNumber);

            if (userExist == null)
                throw new UserNotFoundException("User does not exist");

            await _db.UpdateAsync(user);

        }


        public async Task DeleteUserAsync(DeleteUserDtos deleteUser)
        {

            if (deleteUser.customNumber == null)
                throw new UserNotActivatedException("User not active");

            var user = await _db.GetAsync(deleteUser.customNumber);

            if (user == null)
                throw new UserNotFoundException("User does not exist");

            PasswordVerificationResult results = new PasswordHasher<User>().VerifyHashedPassword(new User { Email = user.Email},
                                                 user.Password, deleteUser.password);

            if (results == PasswordVerificationResult.Failed)
                throw new InvalidCredentialsException("Invalid Credentials");

            await _db.DeleteAsync(user);

        }

        public async Task<string> GetUserNameByCustomNumber(string customNumber)
        {

           return await _db.GetUserByCustomNumber(customNumber);

        }

    }

}
 