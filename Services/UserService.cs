using AuthApi.DTOs;
using AuthApi.Interfaces;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using AuthApi.Exceptions;
using AuthApi.Utilities;
using AuthApi.Exceptions.ExceptionsTypes;

namespace AuthApi.Services
{

    public class UserService : IUserService
    {

        private readonly IUserRepository _db;

        public UserService(IUserRepository user)
        {

            _db = user;
   
        }

        public async Task<Guid> CreateUserAsync(RegisterDto user, CancellationToken cancellationToken)
        {
            
            var IdNumber = HashHelper.HashId(user.IdNumber);
 
            var userExist = await _db.GetUserByIdNumber(IdNumber, cancellationToken);

            if (userExist != null)
                throw new UserAlreadyExistException("User already exist");

            User userdata = new User
            {

                Email = user.Email
                
            };

            userdata.Surname = user.Surname;
            userdata.IdNumber = IdNumber;
            userdata.FirstName = user.FirstName;
            userdata.Password = HashHelper.HashPassword(user.Password);

            await _db.CreateAsync(userdata, cancellationToken);

            return userdata.Id;

        }

        //test passed but user.Verified needs to be removed as its of no use
        public async Task<User> GetUserAsync(LoginDtos login, CancellationToken cancellationToken)
        {

            var user = await _db.GetAsync(login.customNumber, cancellationToken);

            if (user == null)
                throw new UserNotFoundException("User does not exist");

             if (user.isVerified == false)
                throw new UserNotActivatedException("User did not verify email");


            PasswordVerificationResult results = HashHelper.VerifyHashPassword(user.Password, login.password);

            if (results == PasswordVerificationResult.Failed)
                throw new InvalidCredentialsException("Invalid Credentials");

            return user;

        }

        //yet to be refractor and test
        public async Task<User> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            
            var userInfo = await _db.GetUserById(userId, cancellationToken);

            if (userInfo == null)
                throw new UserNotFoundException("User does not exist");

            return userInfo;

        }

        //yet to be refractor and test
        public async Task UpdateUserNumberAsync(string customNumber, User user)
        {

            user.customNumber = customNumber;
            user.isVerified = true;

            await _db.UpdateAsync(user);

        }

        //yet to be refractor and test
        public async Task IsLoginEmailSent(User user)
        {

            user.sentLoginNumber = true;

            await _db.UpdateAsync(user);

        }
        
        //yet to be refractor and test
        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
        {

            var userExist = await _db.GetAsync(user.customNumber, cancellationToken);

            if (userExist == null)
                throw new UserNotFoundException("User does not exist");

            await _db.UpdateAsync(user);

        }

        //yet to be refractor and test
        public async Task DeleteUserAsync(DeleteUserDtos deleteUser, CancellationToken cancellationToken)
        {

            if (deleteUser.customNumber == null)
                throw new UserNotActivatedException("User not active");

            var user = await _db.GetAsync(deleteUser.customNumber, cancellationToken);

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
 