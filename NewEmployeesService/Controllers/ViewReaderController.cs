using ConfigurationService;
using NewEmployeesService.Models;
using NewEmployeesService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NewEmployeesService.Controllers
{

    public static class ViewReaderController
    {
        /// <summary>
        /// Метод принимает путь к текстовому файлу, считывает текст и возвращает его содержимое
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> ReadViewAsync(string path)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return await File.ReadAllTextAsync(path, Encoding.GetEncoding(1251));
            }
            catch (Exception ex)
            {
                Logger.Log($"ViewReaderController.ReadView: {ex.Message}");
                return string.Empty;
            }
        }


        /// <summary>
        /// Метод удаляет строки из файла, содержащие указанные табельные номера
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tabelNumbers"></param>
        public static async Task RemoveLines(string path, List<string>tabelNumbers)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var win1251 = Encoding.GetEncoding("windows-1251");
                string[] allLines = await File.ReadAllLinesAsync(path, win1251);

                var filteredLines = new List<string>();
                var stringForDelete = false;
                foreach(var line in allLines)
                {
                    foreach(var tabel in tabelNumbers)
                    {
                        if(line.Contains(tabel) || string.IsNullOrEmpty(line))
                        {
                            stringForDelete = true;
                        }
                        
                    }
                    if (stringForDelete == false)
                    {
                        filteredLines.Add(line);
                    }
                    stringForDelete = false;
                }



                using (StreamWriter sw = new StreamWriter(path, false, win1251))
                {
                    foreach (var line in filteredLines)
                    {
                        await sw.WriteLineAsync(line);
                    }
                }

                //File.WriteAllLines(path, filteredLines, win1251);
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка в ViewReaderController.RemoveLines: {ex.Message}");
               
            }
        } 




    }
}
