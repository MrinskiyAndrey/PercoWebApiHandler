using ConfigurationService;
using NewEmployeesService.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewEmployeesService
{
    public class NewEmployees
    {
        public static async Task NewEmployeesMethod()
        {
            var config = ConfigurationService.Config.Load();
            var client = ConnectionService.Connection.client;
            var token = await ConnectionService.Connection.GetPercoWebToken(client, config.Login, config.Password);

            if (!string.IsNullOrEmpty(token))
            {

                // Получение текста из PERCO.SQL (выгрузка с Паруса с новыми сотрудниками)
                string viewRawContent = await ViewReaderController.ReadViewAsync(config.PathToNewEmployeesView);

                // Извлечение данных в список объектов класса Employee
                var employeesFromParus = viewRawContent != null ? EmployeeController.ExtractingDataFromParus(viewRawContent) : null;

                // Получение списока подразделений от PercoWeb в виде списка объектов класса DivisionData
                var divisionsListFromPerco = await DivisionController.GetDivisionListFromPercoWeb(client, token);

                // Сравнение подразделений новых сотрудников с существующими подразделениями и получение текстового списока новых подразделений
                var newDivisionsNames = (employeesFromParus != null && divisionsListFromPerco != null) ? DivisionController.CheckingDivisionsIsNotExist(divisionsListFromPerco, employeesFromParus) : null;

                // Создание списока объектов DivisionData из текстового списка новых подразделений
                var newDivisionsData = newDivisionsNames != null ? DivisionController.CreateDivisionData(newDivisionsNames) : null;

                // Создание новых подразделений в PercoWeb, и получение их Id к списку который отправлялся
                var newDivisionsWithId = newDivisionsData != null ? await DivisionController.AddDivision(client, token, newDivisionsData) : null;

                // Добавление новых подразделений с Id к существующему списку подразделений от PercoWeb (понадобится Id подразделений, когда будут добавляться сотрудники)
                if (newDivisionsWithId != null)
                {
                    divisionsListFromPerco?.AddRange(newDivisionsWithId);
                }


                // Получение списка должностей от PercoWeb в виде списка объектов PositionData
                var positionsListFromPerco = await PositionController.GetPositionListFromPercoWeb(client, token);

                
                // Сравнение должностей новых сотрудников с существующими должностями и получение текстового списока новых должностей
                var newPositionsNames = (employeesFromParus != null && positionsListFromPerco != null) ? PositionController.CheckingPositionsIsNotExist(positionsListFromPerco, employeesFromParus) : null;

                

                // Создание списока объектов PositionData из текстового списка новых должностей
                var newPositionsData = newPositionsNames != null ? PositionController.CreatePositionData(newPositionsNames) : null;

                
                // Создание новых должностей в PercoWeb, и получение их Id к списку который отправлялся
                var newPositionsWithId = newPositionsData != null ? await PositionController.AddPosition(client, token, newPositionsData) : null;

                // Добавление новых подразделений с Id к существующему списку подразделений от PercoWeb (понадобится Id подразделений, когда будут добавляться сотрудники)
                if (newPositionsWithId != null)
                {
                    positionsListFromPerco?.AddRange(newPositionsWithId);
                }

                // Создание списка объектов EmployeeData на основе имеющихся данных
                var newEmployeesData = (employeesFromParus != null && positionsListFromPerco != null&& divisionsListFromPerco != null) ? EmployeeController.CreateEmployeeData(employeesFromParus, positionsListFromPerco, divisionsListFromPerco): null;

                // Создание новых сотруддников в PercoWeb
                if(newEmployeesData != null)
                {
                    await EmployeeController.AddEmployee(client, token, config.PathToNewEmployeesView, newEmployeesData);
                }

                Logger.Log("NewEmployees module have been executed successfully.");
            }
        }
    }
}
