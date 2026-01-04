using System;
using System.Collections.Generic;
using System.Text;

namespace ImagesService
{
    public static class ImagesService
    {
        public static async Task MainImagesService()
        {
            var config = ConfigurationService.Config.Load();
            var client = ConnectionService.Connection.client;
            var token = await ConnectionService.Connection.GetPercoWebToken(client, config.Login, config.Password);
            
            

        }
    }
}
