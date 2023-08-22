using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MqttManagement.Models;
using System.Text;
using System.Text.Json;
using System.IO;
using MqttManagement.MQTT;
using System.IO.Ports;

namespace MqttManagement.Forms
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
                topicDgvrDictionary.Add(ad.Topic.ToLower(), dgvAnalog.Rows[nRowIndex]);
            }
            foreach (var dd in systemState.digitalDeviceList)
            {
                objArr[0] = dd.Name;
                objArr[1] = dd.Value;
                dgvDigital.Rows.Add(objArr);
                nRowIndex = dgvDigital.Rows.Count - 1;
                topicDgvrDictionary.Add(dd.Topic.ToLower(), dgvDigital.Rows[nRowIndex]);
            }
        }

        public void updateDevices(string topic, string value)
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
                string deger = currentValuStr == "0" ? "1" : "0";
                mqttObject.Publish_Application_Message(deger, dgvDigital.CurrentRow.Tag.ToString());
            }

        }

    }
}
