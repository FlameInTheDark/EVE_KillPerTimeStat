﻿using System;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void playerFindButton_Click(object sender, EventArgs e) // Нажатие поиска
        {
            chartProgressBar.Value = 0;
            StartChart();
        }

        private void searchBox_DropDown(object sender, EventArgs e) // Выподающее меню
        {

            string requestText = RequestJSON("autocomplete", searchBox.Text);
            try
            {
                requestText = requestText.Insert(0, "{\"FoundObjects\":");
                requestText = requestText + "}";
                SearchObjects objectsSearch = JsonConvert.DeserializeObject<SearchObjects>(requestText);
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

        private void dateTimePicker1_Leave(object sender, EventArgs e)
        {
            LogTextBox.Text += dateTimePicker1.Value.ToString("yyyyMMddHHss");
        }
    }
}
