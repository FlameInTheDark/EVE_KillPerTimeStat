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
    partial class Form1
    {
        Thread chartThread;

        // Функция получения информации от сервера
        public string RequestJSON(string type, string request)
        {
            string uri = "";

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
                if (InvokeRequired)
                {
                    Invoke(new Action(() => { LogTextBox.Text += ex.ToString() + "\n\n"; }));
                }
                else
                {
                    LogTextBox.Text += ex.ToString() + "\n\n";
                }
                return "[]";
            }

            return response;
        }

        // Функция построения графика
        // TODO: Добавить выборку за несколько дней
        public void Chart()
        {
            Invoke(new Action(() =>
            {
                chartStatus.Text = "Получение данных";
                chartProgressBar.Value = 10;
            }));

            string objectRequestText = RequestJSON("autocomplete", (string)searchBox.Invoke(new Func<string>(() => searchBox.Text)));
            objectRequestText = objectRequestText.Insert(0, "{\"FoundObjects\":");
            objectRequestText = objectRequestText + "}";

            Invoke(new Action(() =>
            {
                chartStatus.Text = "Сериализация";
                chartProgressBar.Value = 40;
                LogTextBox.Text += objectRequestText + "\n\n";
            }));

            string requestText;

            try
            {
                SearchObjects objectsSearch = JsonConvert.DeserializeObject<SearchObjects>(objectRequestText);

                Invoke(new Action(() =>
                {
                    findIDLabelText.Text = objectsSearch.FoundObjects[0].id.ToString();
                    findTypeLabelText.Text = objectsSearch.FoundObjects[0].type.ToString();
                }));

                requestText = RequestJSON(objectsSearch.FoundObjects[0].type, objectsSearch.FoundObjects[0].id.ToString());

                if (requestText == "[]")
                {
                    Invoke(new Action(() =>
                    {
                        killChart.Series["Kills"].Points.Clear();
                        for (int i = 1; i <= 24; i++)
                        {
                            this.killChart.Series["Kills"].Points.AddXY(i.ToString(), 0);
                        }
                        chartProgressBar.Value = 100;
                        chartStatus.Text = "Готово";
                        chartProgressBar.Value = 0;
                    }));
                    return;
                }

                requestText = requestText.Insert(0, "{\"Kills\":");
                requestText = requestText + "}";

                PlayerKills allKills = JsonConvert.DeserializeObject<PlayerKills>(requestText);

                Invoke(new Action(() =>
                {
                    chartProgressBar.Value = 85;
                    chartStatus.Text = "Построение графика";
                    killChart.Series["Kills"].Points.Clear();
                    for (int i = 1; i <= 24; i++)
                    {
                        int countKills = 0;
                        int counter = 1;
                        foreach (var onekill in allKills.Kills)
                        {
                            DateTime dateTime = DateTime.ParseExact(onekill.killTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            counter++;
                            if (i == dateTime.Hour)
                            {
                                countKills++;
                            }
                        }
                        this.killChart.Series["Kills"].Points.AddXY(i.ToString(), countKills);
                    }
                    chartProgressBar.Value = 90;
                }));

                Invoke(new Action(() =>
                {
                    chartProgressBar.Value = 100;
                    chartStatus.Text = "Готово";
                    chartProgressBar.Value = 0;
                }));
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    LogTextBox.Text += ex.Message + "\n\n"; ;
                    chartProgressBar.Value = 100;
                    chartStatus.Text = "График не был построен";
                }));
            }
        }

        // Функция запуска потока графика
        public void StartChart()
        {
            chartThread = new Thread(Chart);
            chartThread.Start();
        }

    }
}
