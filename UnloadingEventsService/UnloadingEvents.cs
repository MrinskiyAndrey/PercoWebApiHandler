using ConfigurationService;
using ConnectionService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnloadingEventsService.Controllers;
using UnloadingEventsService.Models;

namespace UnloadingEventsService
{
    public static class UnloadingEvents
    {
        public static async Task Unloading()
        {
            var client = Connection.client;
            var config = Config.Load();

            string token = await Connection.GetPercoWebToken(client, config.Login, config.Password);

            var Events = await EventsController.GetEvents(client, token, config.NumberOfDaysEvents);

            File.WriteAllText(config.PathToPercoXML, Events);

            Logger.Log("Events have been unloaded successfully.");
        }
    }
}
