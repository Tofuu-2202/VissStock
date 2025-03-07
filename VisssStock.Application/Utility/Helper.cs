using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using VisssStock.Domain.DataObjects;
using VisssStock.Infrastructure.Data;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using VisssStock.Application.Services.StockGroupServices; // Commented out as it is not used and causing an error

namespace VisssStock.Application.Utility
{
    public class Helper
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public Helper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        {
            using (var context = new DataContext())
            {
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    context.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        var entities = new List<T>();

                        while (result.Read())
                        {
                            entities.Add(map(result));
                        }

                        return entities;
                    }
                }
            }
        }

        public static string GenerateCode(string name, int? number)
        {
            string input = $"{name}_{number?.ToString() ?? ""}";
            using (System.Security.Cryptography.SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString().Substring(0, 8).ToUpper();
            }
        }

        public int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }

        public int GetUnixTime()
        {
            return (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
