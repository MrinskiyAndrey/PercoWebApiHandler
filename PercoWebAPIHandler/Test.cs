
using NewEmployeesService.Models.DTO;

namespace PercoWebAPIHandler
{
    public static class Test
    {
        public static async Task MainTest()
        {
            var config = ConfigurationService.Config.Load();
            var client = ConnectionService.Connection.client;
            var token = await ConnectionService.Connection.GetPercoWebToken(ConnectionService.Connection.client, config.Login, config.Password);


            var employee = await NewEmployeesService.Controllers.EmployeeController.GetEmployeeByTabelNumber(client, token, "T17486");
            List<EmployeeFullListData> employeesNeedPhoto = new List<EmployeeFullListData>();
            if(employee != null)
            {
                employeesNeedPhoto.Add(employee);
                Console.WriteLine("Мой ID: " + employee.Id);
            }
            

            
            await ImagesService.Controllers.ImagesController.GetImagesAsync(client, token, config.PathToPhotos,employeesNeedPhoto);

         
            


        }
    }
}
