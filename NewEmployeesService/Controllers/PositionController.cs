using ConfigurationService;
using NewEmployeesService.Models;
using NewEmployeesService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace NewEmployeesService.Controllers
{
    public static class PositionController
    {
        public static async Task<List<PositionData>?> GetPositionListFromPercoWeb(HttpClient client, string token)
        {
            string urlGetPosition = $"positions/list?token={token}";
            List<PositionData>? positions = new List<PositionData>();

            try
            {
                var response = await client.GetAsync(urlGetPosition);
                var responseBody = await response.Content.ReadAsStringAsync();           
                positions = JsonSerializer.Deserialize<List<PositionData>>(responseBody);
                return positions;
            }
            catch (Exception ex)
            {

                Logger.Log($"Ошибка получения списка должностей в PositionController/GetPositionListFromPercoWeb: {ex.Message}");
                return null;
            }
        }

        public static List<string>? CheckingPositionsIsNotExist(List<PositionData> positionsFromPerco, List<Employee> newEmployees)
        {

            // получение списков названий существующих должностей и названий должностей у новых сотрудников
            var namesAlreadyExistPositions = positionsFromPerco.Select(pos => pos.Name?.ToString()).ToList();
            var namesPositionsInNewEmployees = newEmployees.Select(emp => emp.Position).Distinct().ToList();

            // проверка есть ли у новых сотрудников должности отсутствующие в Perco
            List<string> posIsNotExistString = namesPositionsInNewEmployees.Where(pos => !namesAlreadyExistPositions.Contains(pos)).ToList();

            
            return (posIsNotExistString.Count > 0) ? posIsNotExistString : null;
        }

        public static List<PositionData>? CreatePositionData(List<string> posIsNotExistString)
        {
            // Создание и заполнение списка новых должностей

            if (posIsNotExistString.Count > 0)
            {
                var newPositions = new List<PositionData>();

                if (posIsNotExistString.Count > 0)
                {
                    foreach (var pos in posIsNotExistString)
                    {
                        newPositions.Add(new PositionData { Name = pos });
                    }
                }

                return newPositions;
            }
            else return null;
        }

        public static async Task<List<PositionData>> AddPosition(HttpClient client, string token, List<PositionData> newPositions)
        {
            var url = $"positions?token={token}";

            if (newPositions.Count > 0)
            {
                foreach (var pos in newPositions)
                {
                    var posJson = JsonSerializer.Serialize(pos);
                    var content = new StringContent(posJson, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(url, content);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var id = JsonSerializer.Deserialize<IdData>(responseBody);
                    if (id != null)
                    {
                        pos.Id = id.Id;
                    }

                }
            }

            return newPositions;
        }

        public static int GetPositionIdByName(string positionName, List<PositionData> positionsFromPerco)
        {
            if (positionsFromPerco == null)
            {
                Logger.Log($"Ошибка в PositionController.GetPositionIdByName: Нет списка должностей!");
                return 0;
            }

            var foundPosition = positionsFromPerco.FirstOrDefault(pos => pos.Name == positionName);

            if (foundPosition != null)
            {
                return foundPosition.Id;
            }
            else
            {
                return 0;
            }
        }
    }
}
