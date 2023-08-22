using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MqttClientStaj.Models;
using System.Text;
using System.Text.Json;
using System.IO;
using MqttClientStaj.MQTT;
using System.IO.Ports;

namespace MqttClientStaj.Forms
{
    public partial class FormMain : Form
    {
        internal SystemState systemState = new SystemState();
        Dictionary<string, DataGridViewRow> topicDgvrDictionary;
        Mqtt mqttObject;
        public FormMain()
        {
            InitializeComponent();
            topicDgvrDictionary = new Dictionary<string, DataGridViewRow>();
            mqttObject = new Mqtt(this);
        }

        private async Task initialization()
        {
            await mqttObject.initiliaze();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            initialization();

            addDevices();
            displayDevices();
        }

        public void addDevices()
        {
            systemState.analogDeviceList.Add(new Device<float>("3", "analog1"));
            systemState.analogDeviceList.Add(new Device<float>("9", "analog4"));
            systemState.analogDeviceList.Add(new Device<float>("14", "analog2"));
            systemState.digitalDeviceList.Add(new Device<bool>("13", "digital3"));
            systemState.digitalDeviceList.Add(new Device<bool>("15", "digital1"));
            systemState.digitalDeviceList.Add(new Device<bool>("16", "digital2"));
        }

        void displayDevices()
        {
            dgvAnalog.Rows.Clear();
            dgvDigital.Rows.Clear();
            topicDgvrDictionary.Clear();
            object[] objArr = new object[2];
            int nRowIndex;
            foreach (var ad in systemState.analogDeviceList)
            {
                objArr[0] = ad.Name;
                objArr[1] = ad.Value;
                dgvAnalog.Rows.Add(objArr);

                nRowIndex = dgvAnalog.Rows.Count - 1;
                dgvAnalog.Rows[nRowIndex].Tag = ad;
                topicDgvrDictionary.Add(ad.Topic.ToLower(), dgvAnalog.Rows[nRowIndex]);
            }
            foreach (var dd in systemState.digitalDeviceList)
            {
                objArr[0] = dd.Name;
                objArr[1] = dd.Value ? "ON" : "OFF";
                dgvDigital.Rows.Add(objArr);
                nRowIndex = dgvDigital.Rows.Count - 1;
                dgvDigital.Rows[nRowIndex].Tag = dd;
                topicDgvrDictionary.Add(dd.Topic.ToLower(), dgvDigital.Rows[nRowIndex]);
            }
        }

        public void updateAnalogDevices(string topic, string value)
        {
            this.Invoke(new Action(() =>
            {
                DataGridViewRow dgvR;
                if (topicDgvrDictionary.TryGetValue(topic, out dgvR))
                {
                    dgvR.Cells[1].Value = float.Parse(value);
                }
            }));
        }
        public void updateDigitalDevices(string topic, string value)
        {
            this.Invoke(new Action(() =>
            {
                DataGridViewRow dgvR;
                if (topicDgvrDictionary.TryGetValue(topic, out dgvR))
                {
                    var d = (Device<bool>)dgvR.Tag;
                    d.Value = value == "1" ? true : false;
                    dgvR.Cells[1].Value = value == "1" ? "ON" : "OFF";
                }
            }));
        }

        int selectedIndex;
        private void dgvDigital_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == dgvDigital.Columns["clmBtn"].Index)
            {
                string currentValuStr = dgvDigital.CurrentRow.Cells[1].Value.ToString();
                var device = ((Device<bool>)dgvDigital.CurrentRow.Tag);
                mqttObject.PublishDigital(!device.Value ? "1" : "0", device.Topic);
            }

        }

    }
}
