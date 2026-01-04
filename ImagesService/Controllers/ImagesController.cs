using NewEmployeesService.Models;
using NewEmployeesService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImagesService.Controllers
{
    public static class ImagesController
    {
        public static async Task GetImagesAsync(HttpClient client, string token, string pathToPhoto, List<EmployeeFullListData> employees)
        {
            foreach (var employee in employees)
            {

                var url = $"users/{employee.Id}/image?field_id=1&token={token}";

                var response = await client.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();


                Console.WriteLine(responseBody);

            }

            
        }
    }
}
