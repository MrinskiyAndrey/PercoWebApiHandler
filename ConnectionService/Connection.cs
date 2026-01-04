using ConfigurationService;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using ConnectionService.Models;

namespace ConnectionService
{
    public static class Connection
    {
        public static HttpClient client = new HttpClient {BaseAddress = new Uri($"http://127.0.0.1/api/") };
        

        public static async Task<string> GetPercoWebToken(HttpClient client, string login, string password)
        {
            
            string urlAuth = "system/auth";

            //Перевод логина и пароля в JSON формат
            var loginData = new { login, password };
            var payload = JsonSerializer.Serialize(loginData);

            // Создание содержимого запроса
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            try
            {
                // отправка запроса
                using var response = await client.PostAsync(urlAuth, content);
                // Получение тела ответа
                var responseBody = await response.Content.ReadAsStringAsync();

                // Десериализация токена и возврат его в виде строки
                var tokenObject = JsonSerializer.Deserialize<Token>(responseBody);
                string? token = tokenObject?.token;
                return token ?? string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Log($"Токен не получен {ex.Message}");
                return string.Empty;
            }

        }
    }
}
