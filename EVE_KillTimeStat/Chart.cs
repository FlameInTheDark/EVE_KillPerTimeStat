using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_KillTimeStat
{
    partial class ProgramForm
    {
        // Функция получения информации от сервера
        public async Task<string> RequestJsonAsyncTask(string type, string request)
        {
            string uri = "";

            // Определяем тип запроса
            switch (type)
            {
                case "autocomplete":
                    uri = @"https://zkillboard.com/autocomplete/" + request + @"/";
                    break;
                case "character":
                    uri = @"https://zkillboard.com/api/kills/characterID/" + request + "/startTime/" + startKillDatePicker.Value.ToString("yyyyMMdd") + "0000/no-attackers/no-items/";
                    break;
                case "system":
                    uri = @"https://zkillboard.com/api/kills/solarSystemID/" + request + "/startTime/" + startKillDatePicker.Value.ToString("yyyyMMdd") + "0000/no-attackers/no-items/";
                    break;
                case "corporation":
                    uri = @"https://zkillboard.com/api/kills/corporationID/" + request + "/startTime/" + startKillDatePicker.Value.ToString("yyyyMMdd") + "0000/no-attackers/no-items/";
                    break;
            }
            string response;
            return await Task.Run(() =>
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        webClient.Headers["User-Agent"] = "EVE_KillTimeStat";
                        // Выполняем запрос по адресу и получаем ответ в виде строки
                        response = webClient.DownloadString(uri);
                    }
                }
                catch (Exception ex)
                {
                    LogTextBox.Text += ex.ToString() + "\n\n";
                    return "[]";
                }

                return response;
            });
        }
        // Функция построения графика
        // TODO: Добавить выборку за несколько дней
        public async Task ChartAsync()
        {
            chartStatus.Text = "Получение данных";
            chartProgressBar.Value = 10;

            var objectRequestText = await RequestJsonAsyncTask("autocomplete", searchBox.Text);
            objectRequestText = objectRequestText.Insert(0, "{\"FoundObjects\":");
            objectRequestText = objectRequestText + "}";

            chartStatus.Text = "Сериализация";
            chartProgressBar.Value = 40;
            LogTextBox.Text += objectRequestText + "\n\n";

            try
            {
                var objectsSearch = JsonConvert.DeserializeObject<SearchObjects>(objectRequestText);

                findIDLabelText.Text = objectsSearch.FoundObjects[0].id.ToString();
                findTypeLabelText.Text = objectsSearch.FoundObjects[0].type.ToString();

                var requestText = await RequestJsonAsyncTask(objectsSearch.FoundObjects[0].type, objectsSearch.FoundObjects[0].id.ToString());

                if (requestText == "[]")
                {
                    killChart.Series["Kills"].Points.Clear();
                    for (int i = 1; i <= 24; i++)
                    {
                        this.killChart.Series["Kills"].Points.AddXY(i.ToString(), 0);
                    }
                    chartProgressBar.Value = 100;
                    chartStatus.Text = "Готово";
                    chartProgressBar.Value = 0;
                    return;
                }

                requestText = requestText.Insert(0, "{\"Kills\":");
                requestText = requestText + "}";

                var allKills = JsonConvert.DeserializeObject<PlayerKills>(requestText);

                chartProgressBar.Value = 85;
                chartStatus.Text = "Построение графика";
                killChart.Series["Kills"].Points.Clear();
                for (int i = 1; i <= 24; i++)
                {
                    int countKills = 0;
                    int counter = 1;
                    foreach (var onekill in allKills.Kills)
                    {
                        var dateTime = DateTime.ParseExact(onekill.killTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        counter++;
                        if (i == dateTime.Hour)
                        {
                            countKills++;
                        }
                    }
                    this.killChart.Series["Kills"].Points.AddXY(i.ToString(), countKills);
                }
                chartProgressBar.Value = 90;

                chartProgressBar.Value = 100;
                chartStatus.Text = "Готово";
                chartProgressBar.Value = 0;
            }
            catch (Exception ex)
            {
                LogTextBox.Text += ex.Message + "\n\n"; ;
                chartProgressBar.Value = 100;
                chartStatus.Text = "График не был построен";
            }
        }
    }
}
