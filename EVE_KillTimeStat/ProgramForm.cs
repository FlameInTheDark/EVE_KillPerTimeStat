using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using System.Xml.Serialization;

namespace EVE_KillTimeStat
{
    public partial class ProgramForm : Form
    {
        public ProgramForm()
        {
            InitializeComponent();
        }

        private async void playerFindButton_Click(object sender, EventArgs e) // Нажатие поиска
        {
            chartProgressBar.Value = 0;
            await ChartAsync();
        }

        private async void searchBox_DropDown(object sender, EventArgs e) // Выподающее меню
        {
            var requestText = await RequestJsonAsyncTask("autocomplete", searchBox.Text);
            try
            {
                requestText = requestText.Insert(0, "{\"FoundObjects\":");
                requestText = requestText + "}";
                var objectsSearch = JsonConvert.DeserializeObject<SearchObjects>(requestText);
                searchBox.Items.Clear();
                foreach (var oneObject in objectsSearch.FoundObjects)
                {
                    searchBox.Items.Add(oneObject.name);
                }
            }
            catch
            {
                requestText = "{\"players\":\"error search\"}";
            }
            LogTextBox.Text += requestText;
        }

        private void startKillDatePicker_ValueChanged(object sender, EventArgs e)
        {
            LogTextBox.Text += "Picked time string: " + startKillDatePicker.Value.ToString("yyyy-MM-dd HH:ss") + "\n\n";
        }

        private void searchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchBox.DroppedDown = true;
            }
        }
    }
}
