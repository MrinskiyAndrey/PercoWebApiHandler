using ConfigurationService;
using MassAccessService.Models;
using MassAccessService.Models.Data;
using NewEmployeesService.Models.DTO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MassAccessService.Controllers
{
    public static class AccessTemplatesController
    {

        public static List<UserOfAccess> FormatUsersData(List<UserOfAccess> usersOfAccess)
        {

            foreach (var user in usersOfAccess)
            {
                if (user.FIO != null && user.FIO.Contains("- "))
                {

                    user.FIO = user.FIO.Replace("- ", "");
                    
                }

            }
            return usersOfAccess;
        }

        public static async Task<List<UserOfAccess>> GetUsersFromFile(string path, string fileName)
        {
            List<UserOfAccess> usersOfAccess = new List<UserOfAccess>();
            var usersStringData =  File.ReadAllLines($"{path}{fileName}");

            foreach ( string line in usersStringData)
            {
                var user = line?.Split(',');
                if (user != null && user.Length >= 3)
                {
                    usersOfAccess.Add(new UserOfAccess
                    {
                        TabNumber = user[0],
                        FIO = user[1],
                        Department = user[2]
                    });
                }
                else
                {
                    Logger.Log($"Нераспознанная строка: {line}");
                }
            }
            return usersOfAccess;
        }

        public static async Task<List<AccessTemplateData>> GetAccessTemplates(HttpClient client, string token)
        {
            var url = $"accessTemplates/list?token={token}";

            try
            {

                var response = await client.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();
                var accessTemplates = System.Text.Json.JsonSerializer.Deserialize<List<AccessTemplateData>>(responseBody);              
                return accessTemplates ?? new List<AccessTemplateData>();
            }
            catch (Exception ex)
            {

                Logger.Log($"Ошибка получения шаблонов доступа в MassAccessService.Controllers.AccessTemplatesController.GetAccessTemplates: {ex.Message}");
            }
            return new List<AccessTemplateData>();
        }


        public static int GetAccessTemplateId(List<AccessTemplateData> accessTemplates, string accessName)
        {
            foreach(var accessTemplate in accessTemplates)
            {
                if(accessTemplate != null && accessTemplate.Name != null)
                {
                    if (accessTemplate.Name.Contains($"{accessName}"))
                    {
                        return accessTemplate.Id;
                    }
                    
                }
                
            }
            Logger.Log($"Шаблон доступа с именем {accessName} не найден.");
            return 0;
        }


        private static async Task LogNotFoundedUsers(List<UserOfAccess> NotFoundedUsers, string path)
        {
            var sb = new StringBuilder();
            foreach(var user in NotFoundedUsers)
            {
                sb.AppendLine($"{user.FIO}, {user.Department}");
            }
            var fileName = Path.Combine(path, $"NotFoundedUsers{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
            await File.WriteAllTextAsync(fileName, sb.ToString(), Encoding.UTF8);
        }

        public static async Task<List<EmployeeData>> CreateListOfEmployeesForAccessTemplate(
            HttpClient client, 
            string token, 
            List<UserOfAccess> usersOfAccess, 
            List<EmployeeFullListData> allEmployees, 
            int AccessTemplateID, 
            string pathToAccessFile)
        {
            var employeesForAccessTemplate = new List<EmployeeData>();

            //  Lookup с ключом по табельному номеру
            var allEmployeeTabelMap = allEmployees
                .Where(e => !string.IsNullOrEmpty(e.TabelNumber))
                .ToLookup(e => e.TabelNumber);

            //  Lookup с ключом по ФИО
            var allEmployeeFioMap = allEmployees
                .Where(e => !string.IsNullOrEmpty(e.LastName))
                .ToLookup(e => $"{e.LastName} {e.FirstName} {e.MiddleName}".Trim().ToLower());

            var targetEmployees = new List<EmployeeFullListData>();
            var usersTrulyNotFound = new List<UserOfAccess>(); //  Пользователи, которых не удалось найти ни по табельному номеру, ни по ФИО

            foreach (var user in usersOfAccess)
            {
                IEnumerable<EmployeeFullListData>? found = null;

                if(!string.IsNullOrEmpty(user.TabNumber))
                    found = allEmployeeTabelMap[user.TabNumber];
                else if(!string.IsNullOrEmpty(user.FIO))
                    found = allEmployeeFioMap[user.FIO.Trim().ToLower()];
                if(found != null && found.Any())
                    targetEmployees.AddRange(found);
                else
                    usersTrulyNotFound.Add(user);
            }

            var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            await Parallel.ForEachAsync(targetEmployees, options, async (empl, tokenCancel) =>
            {
                try
                {
                    var responseBody = await client.GetStringAsync($"users/staff/{empl.Id}?token={token}", tokenCancel);
                    var employeeExt = JsonSerializer.Deserialize<EmployeeExtensionData>(responseBody);

                    if (employeeExt?.AccessTemplate != null)
                    {
                        var templatesID = employeeExt.AccessTemplate
                        .SelectMany(dict => dict.Keys)
                        .Select(int.Parse)
                        .ToList();

                        if (!templatesID.Contains(AccessTemplateID) && !templatesID.Contains(649144485) && !templatesID.Contains(649144486))
                        {
                            templatesID.Add(AccessTemplateID);
                        }

                        employeesForAccessTemplate.Add(new EmployeeData
                        {
                            Id = empl.Id,
                            LastName = empl.LastName,
                            FirstName = empl.FirstName,
                            MiddleName = empl.MiddleName,
                            Division = empl.DivisionId,
                            TabelNumber = empl.TabelNumber,
                            AccessTemplate = templatesID
                        });
                    }

                }
                catch (Exception ex)
                {

                    Logger.Log($"Ошибка в MassAccessService.Controllers.AccessTemplatesController.CreateListOfEmployeesForAccessTemplate:  \n{empl.LastName} {empl.FirstName} divName: {empl.DivisionName} divID: {empl.DivisionId}\n {ex.Message} ");
                }

            });

            await LogNotFoundedUsers(usersTrulyNotFound, pathToAccessFile);

            return employeesForAccessTemplate.ToList();


            //List<UserOfAccess> usersWithoutTabel = new List<UserOfAccess>();

            //StringBuilder strUsersWithoutTabel = new StringBuilder();

            //foreach (var user in usersOfAccess)
            //{
            //    if (!string.IsNullOrEmpty(user.TabNumber))
            //    {

            //        foreach (var empl in allEmployees)
            //        {

            //            if(empl.TabelNumber == user.TabNumber)
            //            {

            //                try
            //                {
            //                    var url = $"users/staff/{empl.Id}?token={token}";

            //                    var response = await client.GetAsync(url);
            //                    var responseBody = await response.Content.ReadAsStringAsync();

            //                    var employeeExt = JsonSerializer.Deserialize<EmployeeExtensionData>(responseBody);

            //                    if (employeeExt?.AccessTemplate != null)
            //                    {


            //                        var templatesID = employeeExt.AccessTemplate
            //                            .SelectMany(dict => dict.Keys)
            //                            .Select(int.Parse)
            //                            .ToList();

            //                        if (!templatesID.Contains(AccessTemplateID) && !templatesID.Contains(649144485))
            //                        {
            //                            templatesID.Add(AccessTemplateID);
            //                        }


            //                        var employe = new EmployeeData
            //                        {
            //                            Id = empl.Id,
            //                            LastName = empl.LastName,
            //                            FirstName = empl.FirstName,
            //                            MiddleName = empl.MiddleName,
            //                            Division = empl.DivisionId,
            //                            TabelNumber = empl.TabelNumber,
            //                            AccessTemplate = templatesID

            //                        };

            //                        employeesForAccessTemplate.Add(employe);
            //                    }


            //                }
            //                catch (Exception ex )
            //                {

            //                    Logger.Log($"Ошибка в MassAccessService.Controllers.AccessTemplatesController.CreateListOfEmployeesForAccessTemplate: {ex.Message} \n{empl.LastName} {empl.FirstName} divName: {empl.DivisionName} divID: {empl.DivisionId} ");
            //                }


            //            }

            //        }

            //    }
            //    else
            //    {
            //        usersWithoutTabel.Add(user);
            //    }
            //}

            //if(usersWithoutTabel != null && usersWithoutTabel.Count>0)
            //{
            //    foreach (var user in usersWithoutTabel)
            //    {
            //        strUsersWithoutTabel.Append(user.FIO);
            //        strUsersWithoutTabel.Append(", ");
            //        strUsersWithoutTabel.Append(user.Department);
            //        strUsersWithoutTabel.Append(Environment.NewLine);
            //    }

            //    await File.WriteAllTextAsync(pathToAccessFile + $"WithoutTabel{DateTime.Now.ToString("yyyy-MM-dd")}.txt", strUsersWithoutTabel.ToString(), Encoding.UTF8);
            //}


            //return employeesForAccessTemplate;
        }  

    }
}
