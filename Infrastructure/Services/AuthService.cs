using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;

using System.Threading.Tasks;


using Aplication.Interfaces.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("AUTH_KEY")!); // 32 bytes
        private static readonly byte[] Iv = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("AUTH_VECTOR")!); // 16 bytes
        private readonly IConfiguration _configuration;
        
        private readonly List<string> _bannedUsernames;
        private readonly string _jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bannedUsernames.json");

        public AuthService(IConfiguration configuration)
        {
            // Cargar nombres prohibidos desde un archivo JSON
            if (File.Exists(_jsonPath))
            {
                var jsonContent = File.ReadAllText(_jsonPath);
                _bannedUsernames = JsonSerializer.Deserialize<List<string>>(jsonContent) ?? new List<string>();
            }
            else
            {
                _bannedUsernames = new List<string>();
            }
            _configuration = configuration;
           
        }
        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = Iv;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }
        public string Decrypt(string cipherText)
        {
            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = Iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
        //metodos 
        //comprueva si 2 contraseñas son iguales, una encriptada y otra no 
        public bool AuthenticatePasswordEncript(string hashedPassword, string passwordToCheck)
        {
            return BCrypt.Net.BCrypt.Verify(passwordToCheck,hashedPassword);
        }
        public string HashinPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        //comprueva si 2 emails son iguales
        public bool AuthenticateEmail(string email, string emailToCheck)
        {
            email = email.ToLower();
            emailToCheck = emailToCheck.ToLower();
            if (emailToCheck == email)
            {
                return true;
            }
            return false;
        }
        //comprueva si 2 num eros de telefono son iguales
        public bool AuthenticatePhonNumber(string phone, string phoneToCheck)
        {
            if (phone == phoneToCheck)
            {
                return true;
            }
            return false;
        }

        //comprueva si 2 usuarios son iguales 
        public  bool AuthenticateUser(string user, string userToCheck)
        {
            if (user == userToCheck)
            {
                return true;
            }
            else return false;
        }
        //comprueva si un objeto tiene propiedades nullas
        public bool HasNullPropertiesLinq(object obj)
        {

            return obj.GetType()
                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                      .Any(prop => prop.GetValue(obj) is null);
        }

        //valida una contraceña segun una serie de reglas
        public bool ValidatePassword(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }

            // Regla 2: Máximo 16 caracteres
            if (password.Length >16)
            {
                return false;
            }

            // Regla 3: Debe contener al menos un número
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            // Regla 4: Debe contener al menos una letra mayúscula
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // Regla 5: Debe contener al menos una letra minúscula
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            // Regla 6: Debe contener al menos un carácter especial
            //if (!password.Any(c => !char.IsLetterOrDigit(c)))
            //{
            //    return false;
            //}

            // Si pasa todas las reglas, es válida
            return true;
        }
        //valida un Nickname segun una serie de reglas
        public bool ValidateUserName(string userName)
        {
           

            // Regla 1: Longitud mínima de 4 caracteres
            if (userName.Length < 4) return false;



            // Regla 2: Longitud máxima de 20 caracteres
            if (userName.Length > 20) return false;


            // Regla 3: Solo permitir letras
            if (!Regex.IsMatch(userName, @"^[a-zA-Z]+$")) return false;


            // Regla 4: Comparar con nombres prohibidos
            if (_bannedUsernames.Contains(userName, StringComparer.OrdinalIgnoreCase)) return false;


            // Retorna true si no hay errores
            return true;
        }
        //valida si un a fecha es mayor de 18 años
        public bool IsAdult(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            // Ajuste por años bisiestos o fechas posteriores en el año actual
            if (dateOfBirth.Date > today.AddYears(-age))
                age--;

            return age >= 18;
        }

        public bool AuthenticateDeviceEncript(string haschedDevice, string deviceToCheck)
        {
            if(haschedDevice == null || deviceToCheck == null) return false;
            return BCrypt.Net.BCrypt.Verify(deviceToCheck,haschedDevice);
        }
    }
}
