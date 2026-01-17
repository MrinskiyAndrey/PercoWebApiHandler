using MassAccessService.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassAccessService
{
    public static class MassAccess
    {
        public static async Task MainMethod(string fileName, string accessName)
        {
            var config = new ConfigurationService.Config();
            var client = ConnectionService.Connection.client;
            var token = await ConnectionService.Connection.GetPercoWebToken(client, config.Login, config.Password);

            // Get users from file and format data
            var rawUserData = await AccessTemplatesController.GetUsersFromFile(config.PathToUsersForAccessTemplate, fileName);
            var usersOfAccess = AccessTemplatesController.FormatUsersData(rawUserData);

            // Get access templates from Perco Web
            var accessTemplates = await AccessTemplatesController.GetAccessTemplates(client, token);

            // Find the specified access template
            var accessTemplateId = AccessTemplatesController.GetAccessTemplateId(accessTemplates, accessName);

            var allEmployees = await NewEmployeesService.Controllers.EmployeeController.GetAllEmployeesFromPerco(client, token);

            if(allEmployees != null)
            {
                // Create list of employees for the access template
                var employeesForAccessTemplate = await AccessTemplatesController.CreateListOfEmployeesForAccessTemplate(client, token, usersOfAccess, allEmployees, accessTemplateId, config.PathToUsersForAccessTemplate);

                foreach (var employee in employeesForAccessTemplate)
                {
                    await NewEmployeesService.Controllers.EmployeeController.EditEmployee(client, token, employee);
                }
            }
            
            

            

            


        }
    }
}
