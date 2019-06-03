﻿using NooLiteServiceSoft.DeviceProperties;
using NooLiteServiceSoft.Settings;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NooLiteServiceSoft.IconClass
{
    public class PictureMain:Device
    {
        readonly XmlDevice xmlDevice = new XmlDevice();
        readonly XmlTypeDevice xmlTypeDevice = new XmlTypeDevice();
        UpdateFW.UpdateDeviceFW update = new UpdateFW.UpdateDeviceFW();
        Icons icons = new Icons();

       
        public PictureBox CreatePictureMain(int i, SerialPort port,PictureBox pictureBox,string devicesChannel, PictureDeviceOn _deviceOn, PictureDeviceOff _deviceoff, PictureDeviceNoConnection deviceNoConnection, string idDevices, string devicesName, string deviceType, TabPage tabPage, int positionPictureTop, int positionPictureLeft,Label srf13000T,Label tempT,Label tempMaxT)
        {

            PictureBox pct = new PictureBox
            {
                Height = 100,
                Width = 100,
                Name = "pct" + i.ToString(),
                Left = 25 + positionPictureLeft * 120,
                Top = 60 + positionPictureTop * 120,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle

            };
            if (deviceType.Equals("7"))
            {
                pct.MouseClick += delegate (object sender, MouseEventArgs e) { MenuItemSRF11000R_Setting(sender, e, port, pictureBox, devicesChannel, devicesName, idDevices, _deviceOn, _deviceoff, deviceNoConnection, i); };
                pct.MouseUp += delegate (object sender, MouseEventArgs e) { Btn_MouseUp(sender, e, port, pictureBox, _deviceOn, _deviceoff, deviceNoConnection, devicesChannel, idDevices, pct, devicesName, deviceType, tabPage, srf13000T); };
            }

            if (deviceType.Equals("6"))
            {
                pct.MouseClick += delegate (object sender, MouseEventArgs e) { MenuItemSRF13000T_Setting(sender, e, port, pictureBox, devicesChannel, devicesName, idDevices, _deviceOn, _deviceoff, deviceNoConnection, srf13000T,i,tempT,tempMaxT); };
                pct.MouseUp += delegate (object sender, MouseEventArgs e) { Btn_MouseUp(sender, e, port, pictureBox, _deviceOn, _deviceoff, deviceNoConnection, devicesChannel, idDevices, pct, devicesName, deviceType, tabPage, srf13000T); };
            }
            else
            {
                pct.MouseClick += delegate (object sender, MouseEventArgs e) { DbClick_Connection(sender, e, port, devicesChannel, _deviceOn, _deviceoff, deviceNoConnection, idDevices); };
                pct.MouseUp += delegate (object sender, MouseEventArgs e) { Btn_MouseUp(sender, e, port, pictureBox, _deviceOn, _deviceoff, deviceNoConnection, devicesChannel, idDevices, pct, devicesName, deviceType, tabPage, srf13000T); };
            }
            tabPage.Controls.Add(pct);
            return pct;
        }

        private void MenuItemSRF13000T_Setting(object _sender, MouseEventArgs e, SerialPort port,PictureBox pct, string devicesChannel, string devicesName, string idDevices, PictureDeviceOn _deviceOn, PictureDeviceOff deviceOff, PictureDeviceNoConnection deviceNoConnection, Label srf13000T,int i, Label tempT, Label tempMaxT)
        {
            if (e.Button == MouseButtons.Left)
            {
                string[] idArray = idDevices.Split('&');
                byte[] idArrayByte = new byte[idArray.Length];
                for (int j = 0; j< idArray.Length; j++)
                {
                    idArrayByte[j] = byte.Parse(idArray[j]);
                }
                Device device = new Device
                {
                    NameDevice = devicesName,
                    Channel = byte.Parse(devicesChannel),
                    Id = idArrayByte
                };

                byte[] buffer = new byte[17] { 171, 2, 8, 0, byte.Parse(devicesChannel), 128, 0, 0, 0, 0, 0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
                byte[] tx_buffer = CRC(buffer);
                byte[] rx_buffer = new byte[17];
                if (port.IsOpen == false) port.Open();
                port.Write(tx_buffer, 0, tx_buffer.Length);
                WaitData(port, rx_buffer);//to do          

                if (port.IsOpen) port.Close();
                char[] a = Binary(rx_buffer[9]);
                if (rx_buffer[2] == 1)
                {

                    deviceNoConnection.VisibleTrue();
                    _deviceOn.VisibleFalse();
                    deviceOff.VisibleFalse();
                }
                else
                {
                    if (rx_buffer[3] == 0)
                    {
                        if (int.Parse(a[0].ToString()) == 1)
                        {
                            _deviceOn.VisibleTrue();
                            deviceOff.VisibleFalse();
                            deviceNoConnection.VisibleFalse();
                        }
                        else
                        {
                            _deviceOn.VisibleFalse();
                            deviceOff.VisibleTrue();
                            deviceNoConnection.VisibleFalse();
                        }
                        using (SettingSRF13000T fm = new SettingSRF13000T(device, pct, _deviceOn, deviceOff, deviceNoConnection, srf13000T, i, tempT, tempMaxT))
                        {
                            fm.ShowDialog();
                        }
                    }
                    else
                    {
                        if (rx_buffer[3] == 2)
                        {
                            _deviceOn.VisibleFalse();
                            deviceNoConnection.VisibleTrue();
                        }
                    }
                }
            }
        }

        private void MenuItemSRF11000R_Setting(object _sender, MouseEventArgs e, SerialPort port, PictureBox pct, string devicesChannel, string devicesName, string idDevices, PictureDeviceOn _deviceOn, PictureDeviceOff deviceOff, PictureDeviceNoConnection deviceNoConnection,int i)
        {
            if (e.Button == MouseButtons.Left)
            {
                string[] idArray = idDevices.Split('&');
                byte[] idArrayByte = new byte[idArray.Length];
                for (int j = 0; j < idArray.Length; j++)
                {
                    idArrayByte[j] = byte.Parse(idArray[j]);
                }
                Device device = new Device
                {
                    NameDevice = devicesName,
                    Channel = byte.Parse(devicesChannel),
                    Id = idArrayByte
                };

                byte[] buffer = new byte[17] { 171, 2, 8, 0, byte.Parse(devicesChannel), 128, 0, 0, 0, 0, 0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
                byte[] tx_buffer = CRC(buffer);
                byte[] rx_buffer = new byte[17];
                if (port.IsOpen == false) port.Open();
                port.Write(tx_buffer, 0, tx_buffer.Length);
                WaitData(port, rx_buffer);//to do          

                if (port.IsOpen) port.Close();
                char[] a = Binary(rx_buffer[9]);
                if (rx_buffer[2] == 1)
                {

                    deviceNoConnection.VisibleTrue();
                    _deviceOn.VisibleFalse();
                    deviceOff.VisibleFalse();
                }
                else
                {
                    if (rx_buffer[3] == 0)
                    {
                        if (int.Parse(a[0].ToString()) == 1)
                        {
                            _deviceOn.VisibleTrue();
                            deviceOff.VisibleFalse();
                            deviceNoConnection.VisibleFalse();
                        }
                        else
                        {
                            _deviceOn.VisibleFalse();
                            deviceOff.VisibleTrue();
                            deviceNoConnection.VisibleFalse();
                        }
                        using (SettingSRF11000R fm = new SettingSRF11000R(device))
                        {
                            fm.ShowDialog();
                        }
                    }
                    else
                    {
                        if (rx_buffer[3] == 2)
                        {
                            _deviceOn.VisibleFalse();
                            deviceNoConnection.VisibleTrue();
                        }
                    }
                }
            }
        }

        private void DbClick_Connection(object sender, MouseEventArgs e, SerialPort port, string devicesChannel, PictureDeviceOn _deviceOn,PictureDeviceOff deviceOff, PictureDeviceNoConnection deviceNoConnection,string idDevices)
        {
            //SWITCH COMMAND
            if (e.Button == MouseButtons.Left)
            {
                if (deviceNoConnection.StatusIcon()== false)
                {
                    string[] idArray = idDevices.Split('&');
                    byte[] buffer = new byte[17] { 171, 2, 8, 0, byte.Parse(devicesChannel), 4, 0, 0, 0, 0, 0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
                    byte[] tx_buffer = CRC(buffer);
                    byte[] rx_buffer = new byte[17];
                    if (port.IsOpen == false) port.Open();
                    port.Write(tx_buffer, 0, tx_buffer.Length);
                    WaitData(port, rx_buffer);                  
                    if (rx_buffer[2] == 1) {
                        
                            deviceNoConnection.VisibleTrue();
                            _deviceOn.VisibleFalse();
                            deviceOff.VisibleFalse();
                        
                    }
                    else
                    {
                        if (rx_buffer[3] == 0)
                        {
                            if (rx_buffer[9] == 1)
                            {
                                _deviceOn.VisibleTrue();
                                deviceOff.VisibleFalse();
                                deviceNoConnection.VisibleFalse();
                                MessageBox.Show("Передача");
                            }
                            if (rx_buffer[9] == 0)
                            {
                                _deviceOn.VisibleFalse();
                                deviceOff.VisibleTrue();
                                deviceNoConnection.VisibleFalse();
                                MessageBox.Show("Передача");
                            }
                        }
                        else
                        {
                            if (rx_buffer[3] == 2)
                            {
                                _deviceOn.VisibleFalse();
                                deviceNoConnection.VisibleTrue();
                            }
                        }
                    }
                    if (port.IsOpen) { port.Close(); }
                }
            }
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e, SerialPort port, PictureBox pictureBox,PictureDeviceOn _deviceOn, PictureDeviceOff _deviceoff, PictureDeviceNoConnection deviceNoConnection, string devicesChannel, string idDevices, PictureBox pct, string devicesName,string deviceType, TabPage tabPage, Label srf13000T)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (deviceNoConnection.StatusIcon() == false)
                {
                    ContextMenuStrip context = new ContextMenuStrip();
                    ToolStripMenuItem menuItem1 = new ToolStripMenuItem();
                    ToolStripMenuItem menuItem2 = new ToolStripMenuItem();
                    ToolStripMenuItem menuItem3 = new ToolStripMenuItem();
                    ToolStripMenuItem menuItem4 = new ToolStripMenuItem();
                    context.Items.AddRange(new ToolStripMenuItem[] { menuItem1, menuItem2, menuItem3, menuItem4 });
                    menuItem1.Text = "Отвязать";
                    menuItem2.Text = "Свойства";
                    menuItem3.Text = "Настройка";
                    menuItem4.Text = "Обновление Прошивки";
                    menuItem1.Click += delegate (object _sender, EventArgs _e) { MenuItem1_ClickRemove(_sender, _e, port, devicesChannel, idDevices, pct, devicesName, tabPage); };
                    menuItem2.Click += delegate (object _sender, EventArgs _e) { MenuItem2_ClickProperty(_sender, _e, port, devicesChannel, idDevices, pct, devicesName, deviceType); };
                    menuItem3.Click += delegate (object _sender, EventArgs _e) { MenuItem3_Setting(_sender, _e, port, devicesChannel, idDevices, pct, devicesName); };
                    menuItem4.Click += delegate (object _sender, EventArgs _e) { MenuItem4_UpdateFirmware(_sender, _e, port, devicesChannel, idDevices, deviceType); };
                    context.Show(Cursor.Position);
                }
                else
                {
                    ContextMenuStrip context = new ContextMenuStrip();
                    ToolStripMenuItem menuItem1 = new ToolStripMenuItem();
                    ToolStripMenuItem menuItem2 = new ToolStripMenuItem();
                    ToolStripMenuItem menuItem3 = new ToolStripMenuItem();
                    context.Items.AddRange(new ToolStripMenuItem[] { menuItem1, menuItem2,menuItem3 });
                    menuItem1.Text = "Отвязать";
                    menuItem2.Text = "Обновить состояние";
                    menuItem3.Text = "Обновление Прошивки";
                    menuItem1.Click += delegate (object _sender, EventArgs _e) { MenuItem1_ClickRemove(_sender, _e, port, devicesChannel, idDevices, pct, devicesName, tabPage); };
                    menuItem2.Click += delegate (object _sender, EventArgs _e) { icons.StatusAllIcons(pictureBox, _deviceoff, _deviceOn, deviceNoConnection, idDevices, srf13000T); };
                    menuItem3.Click += delegate (object _sender, EventArgs _e) { MenuItem4_UpdateFirmware(_sender, _e, port, devicesChannel, idDevices, deviceType); };
                    context.Show(Cursor.Position);

                }
            }
        }

        private void MenuItem1_ClickRemove(object _sender, EventArgs _e, SerialPort port, string devicesChannel, string idDevices, PictureBox pct, string devicesName, TabPage tabPage)
        {
            //SERVICE AND REMOVE COMMAND
            string[] idArray = idDevices.Split('&');
            byte[] bufferService = new byte[17] { 171, 2, 8, 0, byte.Parse(devicesChannel), 131, 0, 1, 0, 0, 0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
            byte[] bufferRemove = new byte[17] {171,2,8,0, byte.Parse(devicesChannel),9,0,1,0,0,0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
            byte[] tx_bufferService = CRC(bufferService);
            byte[] tx_bufferRemove = CRC(bufferRemove);
            byte[] rx_buffer = new byte[17];
            if (port.IsOpen == false) port.Open();
            port.Write(tx_bufferService, 0, tx_bufferService.Length);
            Thread.Sleep(500);
            port.Write(tx_bufferRemove,0,tx_bufferRemove.Length);
            WaitData(port, rx_buffer);
            if (rx_buffer[3] == 0)
            {
                xmlDevice.DeviceRemoveXml(idArray,devicesChannel);
                pct.Dispose();
            }
            if (port.IsOpen) port.Close();
            icons.IconAddallDevices(tabPage);
        }

        private void MenuItem2_ClickProperty(object _sender, EventArgs _e, SerialPort port, string devicesChannel, string idDevices, PictureBox pct, string devicesName, string deviceType)
        {
            //PROPERTIES
            string[] idArray = idDevices.Split('&');
            string status;
            byte[] bufferMainPropertiesFirstWrite = new byte[17] { 171, 2, 8, 0, byte.Parse(devicesChannel), 128, 0, 170, 0, 0, 0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
            byte[] bufferMainPropertiesSecondWrite = new byte[17] { 171, 2, 8, 0, byte.Parse(devicesChannel), 128, 2, 170, 0, 0, 0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
            byte[] tx_bufferMainPropertiesFirstWrite = CRC(bufferMainPropertiesFirstWrite);
            byte[] tx_bufferMainPropertiresSecondWrite = CRC(bufferMainPropertiesSecondWrite);

            byte[] rx_bufferMainPropertiesFirstRequest = new byte[17];
            byte[] rx_bufferMainPropertiesSecondRequest = new byte[17];
            byte[] rx_bufferActiveChannel = new byte[17];//для активного канала в SRF-10-1000


            if (port.IsOpen == false) port.Open();
            port.Write(tx_bufferMainPropertiesFirstWrite, 0, tx_bufferMainPropertiesFirstWrite.Length);
            WaitData(port, rx_bufferMainPropertiesFirstRequest);           
            port.DiscardInBuffer();
            port.Write(tx_bufferMainPropertiresSecondWrite, 0, tx_bufferMainPropertiresSecondWrite.Length);
            WaitData(port, rx_bufferMainPropertiesSecondRequest);
            if (deviceType.Equals("2"))
            {
                byte[] bufferActiveChannel = new byte[17] { 171, 2, 8, 0, byte.Parse(devicesChannel), 128, 1, 170, 0, 0, 0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
                byte[] tx_bufferActiveChannel = CRC(bufferActiveChannel);
                port.Write(tx_bufferActiveChannel, 0, tx_bufferActiveChannel.Length);
                WaitData(port, rx_bufferActiveChannel);
            }

            if (port.IsOpen) port.Close();
            if (rx_bufferMainPropertiesFirstRequest[9] == 1)
            {
                status = "ВКЛ";
            }
            else
            {
                status = "ВЫКЛ";
            }
            using (DeviceProperty deviceProperty = new DeviceProperty(rx_bufferMainPropertiesFirstRequest, rx_bufferMainPropertiesSecondRequest, rx_bufferActiveChannel, status))
            {
                deviceProperty.ShowDialog();
            }          
        }


        private void MenuItem3_Setting(object _sender, EventArgs _e, SerialPort port, string devicesChannel, string idDevices, PictureBox pct, string devicesName)
        {
            string[] idArray = idDevices.Split('&');
            int count = 0;

            Device device = new Device
            {
                NameDevice = devicesName,
                Channel = byte.Parse(devicesChannel),         
            };

            foreach(var d in idArray)
            {
                device.Id[count] = byte.Parse(d);
                count++;
            }

            byte[] bufferMainPropertiesFirstWrite = new byte[17] { 171, 2, 8, 0, byte.Parse(devicesChannel), 128, 0, 170, 0, 0, 0, byte.Parse(idArray[0]), byte.Parse(idArray[1]), byte.Parse(idArray[2]), byte.Parse(idArray[3]), 0, 172 };
            byte[] tx_bufferMainPropertiesFirstWrite = device.CRC(bufferMainPropertiesFirstWrite);
            byte[] rx_bufferMainPropertiesFirstRequest = new byte[17];
            if (port.IsOpen == false) port.Open();
            port.Write(tx_bufferMainPropertiesFirstWrite, 0, tx_bufferMainPropertiesFirstWrite.Length);
            WaitData(port, rx_bufferMainPropertiesFirstRequest);
            if (port.IsOpen == true) port.Close();
            device.TypeCode = rx_bufferMainPropertiesFirstRequest[7];
            using (SettingFTX fm = new SettingFTX(device))
            {
                fm.ShowDialog();
            }         
        }

        private void MenuItem4_UpdateFirmware(object _sender, EventArgs _e, SerialPort port, string devicesChannel, string idDevices, string deviceType)
        {

            update.PreUpdateFW(port,devicesChannel,idDevices,deviceType);
        }
    }
}
 