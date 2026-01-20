using ConfigurationService;
using NewEmployeesService.Models;
using NewEmployeesService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NewEmployeesService.Controllers
{
    public static class EmployeeController
    {
        /// <summary>
        /// Метод принимает необработанный текст из View извлекает данные и возвращает List объектов Employee
        /// 
        /// </summary>
        /// <param name="viewRawContent"></param>
        /// <returns></returns>
        public static List<Employee>? ExtractingDataFromParus(string viewRawContent)
        {
            if (!string.IsNullOrEmpty(viewRawContent))
            {
                List<Employee>? viewContent = new List<Employee>();

                string pattern = @"\'(.*?)\'";

                try
                {
                    // Разделение данных на массив строк
                    var lines = viewRawContent.Replace("\r", string.Empty).Split('\n');

                    // Извлечение данных по сотруддникам 
                    List<string> employeeContent = new List<string>();
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrEmpty(line) && line.Contains("CONVERT_CARD"))
                        {
                            var matches = Regex.Matches(line, pattern);

                            foreach (Match match in matches)
                            {
                                if (match.Success)
                                {
                                    employeeContent.Add(match.Groups[1].Value);
                                }
                            }
                            // Lобавление в список класса Employee
                            viewContent.Add(new Employee
                            {
                                Position = string.IsNullOrEmpty(employeeContent[0]) ? "(не определена)" : employeeContent[0],
                                Division = string.IsNullOrEmpty(employeeContent[1]) ? "(не определено)" : employeeContent[1],
                                TabelNumber = string.IsNullOrEmpty(employeeContent[2]) ? "(не определен)" : employeeContent[2],
                                FirstName = string.IsNullOrEmpty(employeeContent[4]) ? "(не определена)" : employeeContent[4],
                                LastName = string.IsNullOrEmpty(employeeContent[3]) ? "(не определено)" : employeeContent[3],
                                MiddleName = string.IsNullOrEmpty(employeeContent[5]) ? "(не определено)" : employeeContent[5]

                            });
                            employeeContent.Clear();
                        }
                    }

                    // Отсеивание старых записей
                    var employeeList = viewContent
                        .GroupBy(emp =>emp.TabelNumber)
                        .Select(group => group.Last())
                        .ToList();

                    return employeeList;
                }
                catch (Exception ex)
                {

                    Logger.Log($"Ошибка получения данных в EmployeeController.ExtractingDataFromParus:  {ex.Message}");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static List<EmployeeData>? CreateEmployeeData(List<Employee> employees, List<PositionData> positionsFromPerco, List<DivisionData>divisionsFromPerco)
        {
            if (employees.Count > 0 && positionsFromPerco != null)
            {
                var newEmployeesData = new List<EmployeeData>();

                if (employees.Count > 0)
                {
                    foreach (var employee in employees)
                    {
                        int positionId = PositionController.GetPositionIdByName(employee.Position, positionsFromPerco);
                        int divisionId = DivisionController.GetDivisionIdByName(employee.Division, divisionsFromPerco);
                        newEmployeesData.Add(new EmployeeData
                        {
                            Position = positionId,
                            Division = divisionId,
                            TabelNumber = employee.TabelNumber,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            MiddleName = employee.MiddleName,
                            HiringDate = DateTime.Today.ToString("yyyy-MM-dd")

                        });
                    }
                }

                return newEmployeesData;
            }
            else return null;
        }


        public static async Task EditEmployee(HttpClient client, string token, EmployeeData employee, List<string>?tabelListForDelete = null)
        {

            var url = $"users/staff/{employee.Id}?token={token}";

            try
            {
                var employeeJson = JsonSerializer.Serialize(employee);
                var content = new StringContent(employeeJson, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                
                if ( responseBody.Contains("result"))
                {
                    Logger.Log($"Успешно изменен существующий сотрудник  {employee.LastName} {employee.FirstName} {employee.MiddleName}" +
                        $" табельный номер: '{employee.TabelNumber}' ID: '{employee.Id}'");
                    
                    if(employee.TabelNumber != null && tabelListForDelete != null)
                    {
                        tabelListForDelete?.Add(employee.TabelNumber);
                    }
                }
                else
                {
                    Logger.Log($"[Ошибка изменения сотрудника в NewEmployeesService.Controllers.EmployeeController.EditEmployee] \n" +
                        $"{employee.LastName} {employee.FirstName} {employee.MiddleName} " +
                        $"табельный номер: '{employee.TabelNumber}' ID: '{employee.Id}'\n ResponseBody:{responseBody}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка изменения сотрудника в NewEmployeesService.Controllers.EmployeeController.EditEmployee: {ex.Message}");
            }
        }


        public static async Task AddEmployee(HttpClient client, string token, string PathToNewEmployeesView , List<EmployeeData> newEmployees)
        {
            var url = $"users/staff?token={token}";

            var alreadyExistEmployees = new List<EmployeeData>();

            var tabelListForDelete = new List<string>();

            if (newEmployees.Count > 0)
            {
                try
                {
                    foreach (var employee in newEmployees)
                    {
                        var employeeJson = JsonSerializer.Serialize(employee);
                        var content = new StringContent(employeeJson, Encoding.UTF8, "application/json");
                        var response = await client.PutAsync(url, content);
                        var responseBody = await response.Content.ReadAsStringAsync();
                        if(responseBody.Contains("id"))
                        {
                            employee.Id = JsonSerializer.Deserialize<IdData>(responseBody)?.Id;

                            Logger.Log($"Успешно добавлен новый сотрудник:  '{employee.LastName}' '{employee.FirstName}' '{employee.MiddleName}',  " +
                                $"табельный номер: '{employee.TabelNumber}' в PercoWeb, id: '{employee.Id}',");

                            if (employee.TabelNumber != null)
                            {
                                tabelListForDelete.Add(employee.TabelNumber);
                            }
                            
                            
                        }
                        else if (responseBody.Contains("Такой табельный номер уже существует"))
                        {
                            alreadyExistEmployees.Add(employee);
                        }
                        else
                        {
                            Logger.Log($"Неизвестная ошибка при добавлении нового сотрудника" +
                                $"  {employee.LastName} {employee.FirstName} {employee.MiddleName} табельный номер {employee.TabelNumber} Должность: {employee.Position} " +
                                $" в NewEmployeesService.Controllers.EmployeeController.AddEmployee: {responseBody}");
                        }

                    }
                    if(alreadyExistEmployees != null)
                    {
                        await UpdateEmployeeIds(client, token, alreadyExistEmployees);

                        foreach(var employee in alreadyExistEmployees)
                        {
                            await EditEmployee(client, token, employee, tabelListForDelete);
                        }
                        
                    }
                }
                catch (Exception ex)
                {

                    Logger.Log($"Ошибка добавления новых сотрудников в PercoWeb в NewEmployeesService.Controllers.EmployeeController.AddEmployee: {ex.Message}");
                }
                await ViewReaderController.RemoveLines(PathToNewEmployeesView, tabelListForDelete);
            }
        }


        public static async Task<EmployeeData?> GetEmployeeById(HttpClient client, string token, int id)
        {
            var url = $"users/staff/{id}?token={token}";

            try
            {
                var response = await client.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();
                var employee = JsonSerializer.Deserialize<EmployeeData>(responseBody);
                return employee ?? null;
            }
            catch (Exception ex)
            {

                Logger.Log($"Ошибка получения сотрудника по ID в PercoWeb в NewEmployeesService.Controllers.EmployeeController.GetEmployeeById: {ex.Message}");
                return null;
            }
            
           
        }

        public static async Task<EmployeeFullListData?>GetEmployeeByTabelNumber(HttpClient client, string token, string tabelNumber)
        {
            var allEmployees = await GetAllEmployeesFromPerco(client, token);

            
            return allEmployees?.FirstOrDefault(emp =>  emp.TabelNumber == tabelNumber);

        }
 
        public static async Task<List<EmployeeFullListData>?> GetAllEmployeesFromPerco(HttpClient client, string token)
        {
            var url = $"users/staff/fullList?token={token}";
            try
            {
                var response = await client.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();
                var allEmployees = JsonSerializer.Deserialize<List<EmployeeFullListData>>(responseBody);

                if(allEmployees != null)
                {
                    return allEmployees;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                Logger.Log($"Ошибка получения всех списка всех сотрудников от PercoWeb в NewEmployeesService.Controllers.EmployeeController.GetAllEmployeesFromPerco: {ex.Message}");
                return null;
            }

        }


        public static async Task UpdateEmployeeIds(HttpClient client, string token, List<EmployeeData> employeeWithTabelExist)
        {
            
            var allEmployeesFromPerco = await GetAllEmployeesFromPerco(client, token);

            if(allEmployeesFromPerco != null)
            {
                foreach(var newEmp in employeeWithTabelExist)
                {
                    foreach (var oldEmp in allEmployeesFromPerco)
                    {
                        if(newEmp.TabelNumber == oldEmp.TabelNumber)
                        {
                            newEmp.Id = oldEmp.Id;
                        }
                    }
                }

            }

        }

    }
}
