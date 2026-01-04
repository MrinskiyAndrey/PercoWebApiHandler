using ConfigurationService;
using NewEmployeesService.Models;
using NewEmployeesService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace NewEmployeesService.Controllers
{
    public class DivisionController
    {
        public static async Task<List<DivisionData>?> GetDivisionListFromPercoWeb(HttpClient client, string token)
        {
            string urlGetDivisions = $"divisions/list?token={token}";
            List<DivisionData>? divisions = new List<DivisionData>();

            try
            {
                var response = await client.GetAsync(urlGetDivisions);
                var responseBody = await response.Content.ReadAsStringAsync();
                divisions = JsonSerializer.Deserialize<List<DivisionData>>(responseBody);
                return divisions;
            }
            catch (Exception ex)
            {
                
                Logger.Log($"Ошибка получения списка подразделений в DivisionController/GetDivisionList: {ex.Message}");
                return null;
            }
        }

        public static List<string>? CheckingDivisionsIsNotExist(List<DivisionData> divisionsFromPerco, List<Employee> newEmployees)
        {
            
            // получение списков названий существующих подразделений и названий подразделений у новых сотрудников
            var namesAlreadyExistDivisions = divisionsFromPerco.Select(div => div.Name?.ToString()).ToList();
            var namesDivisionsInNewEmployees = newEmployees.Select(emp => emp.Division).Distinct().ToList();
            
            // проверка есть ли у новых сотрудников подразделения отсутствующие в Perco
            List<string> divIsNotExistString = namesDivisionsInNewEmployees.Where(div => !namesAlreadyExistDivisions.Contains(div)).ToList();

            return (divIsNotExistString.Count > 0) ? divIsNotExistString : null;
        }

        public static List<DivisionData>? CreateDivisionData(List<string> divIsNotExistString)
        {
            // Создание и заполнение списка новых подразделений

            if (divIsNotExistString.Count > 0)
            {
                var newDivisions = new List<DivisionData>();

                if (divIsNotExistString.Count > 0)
                {
                    foreach (var div in divIsNotExistString)
                    {
                        newDivisions.Add(new DivisionData { Name = div });
                    }
                }

                return newDivisions;
            }
            else return null;
        }

        public static async Task<List<DivisionData>> AddDivision(HttpClient client, string token, List<DivisionData>newDivisions)
        {
            var url = $"divisions?token={token}";

            

            if(newDivisions.Count > 0)
            {

                foreach(var div in newDivisions)
                {
                    try
                    {
                        var divJson = JsonSerializer.Serialize(div);
                        var content = new StringContent(divJson, Encoding.UTF8, "application/json");
                        var response = await client.PutAsync(url, content);
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var id = JsonSerializer.Deserialize<IdData>(responseBody);
                        if (id != null)
                        {
                            div.Id = id.Id;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка добавления нового подразделения! {ex.Message}");
                        throw;
                    }
                    
                    
                }
            }

            return newDivisions;
        }

        public static int GetDivisionIdByName(string divisionName, List<DivisionData> divisionsFromPerco)
        {
            if (divisionsFromPerco == null)
            {
                Logger.Log($"Ошибка в DivisionController.GetDivisionIdByName: Нет списка подразделений!");
                return 0;
            }

            var foundDivision = divisionsFromPerco.FirstOrDefault(div => div.Name == divisionName);

            if (foundDivision != null)
            {
                return foundDivision.Id;
            }
            else
            {
                return 0;
            }
        }

    }
}
