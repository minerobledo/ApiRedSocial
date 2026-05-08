

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Services
{
    public interface IAuthService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        bool AuthenticateDeviceEncript(string haschedDevice, string stringToChec);
        bool AuthenticatePasswordEncript(string hashedPassword, string passwordToCheck);
        bool AuthenticateEmail(string email,string emailToCheck);
        bool AuthenticatePhonNumber(string phone, string phoneToCheck);
        string HashinPassword(string password);
        bool AuthenticateUser(string user, string userToCheck);
        bool HasNullPropertiesLinq(object obj);
        bool ValidatePassword(string password);

        bool ValidateUserName(string userName);
        bool IsAdult(DateTime dateOfBirth);
    }
}
