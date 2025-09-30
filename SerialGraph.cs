using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZedGraph;

namespace RealTimeGraph
{
    public partial class SerialGraph : Form
    {

        GraphPane ZedGraphPane;
        RollingPointPairList[] la;
        CheckBox[] cbGraphArray;
        TextBox[] txarray;
        TextBox[] xxarray;

        Queue<byte> Que = new Queue<byte>();
        private bool DATARUNFLAG = true;
        private delegate void SetTextDeleg(byte[] text);
        private delegate void SetSaveDeleg(string text);

        public SerialGraph()
        {
            InitializeComponent();
            SerialPort_Init();

            cbGraphArray = new CheckBox[] {  cbDebug1, cbDebug2, cbDebug3, cbDebug4, cbDebug5, cbDebug6, cbDebug7, cbDebug8, cbDebug9,
                                                        cbDebug10, cbDebug11, cbDebug12, cbDebug13 ,cbDebug14, cbDebug15, cbDebug16, cbDebug17 };

            txarray = new TextBox[] { tbDebug1,  tbDebug2,  tbDebug3,  tbDebug4,  tbDebug5,  tbDebug6,  tbDebug7,  tbDebug8, tbDebug9,
                                      tbDebug10, tbDebug11, tbDebug12, tbDebug13, tbDebug14, tbDebug15, tbDebug16, tbDebug17};
            xxarray = new TextBox[] {textBox1,  textBox2,  textBox3,  textBox4,  textBox5,  textBox6,  textBox7,  textBox8, textBox9,
                                     textBox10, textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17};

            foreach (TextBox txt in xxarray)
            {
                txt.Text = "1";
            }

            foreach (TextBox txt in txarray)
            {
                txt.Text = "0";
            }

            ZedGraph_Init();

            //Thread start
            Thread Tparsing = new Thread(new ThreadStart(ThreadParsing));
            Tparsing.Start();
            Tparsing.IsBackground = true;
            Thread.Sleep(100);


            File_save.file_save_init();

        }

        private void ZedGraph_Init()
        {
            ZedGraphPane = new GraphPane();

            ZedGraphPane = zedGraphControl1.GraphPane;

            ZedGraphPane.Title.Text = "Serial to Graph";
            ZedGraphPane.Title.IsVisible = false;
            ZedGraphPane.XAxis.Type = AxisType.Linear;

            ZedGraphPane.XAxis.Title.FontSpec.Size = 0.5f * (this.Size.Width / 100);
            ZedGraphPane.XAxis.Title.IsVisible = false;
            ZedGraphPane.XAxis.Scale.MinorStep = 1;    //작은 눈금 
            ZedGraphPane.XAxis.Scale.MajorStep = 5;   //큰 눈금
            ZedGraphPane.XAxis.Scale.Min = 0;
            ZedGraphPane.XAxis.Scale.Max = 100;

            ZedGraphPane.YAxis.Title.IsVisible = false;
            ZedGraphPane.YAxis.Title.FontSpec.Size = 0.5f * (this.Size.Width / 100);
            ZedGraphPane.YAxis.Scale.MinorStep = 5;
            ZedGraphPane.YAxis.Scale.MajorStep = 10;
            ZedGraphPane.YAxis.Scale.MagAuto = true;
            ZedGraphPane.YAxis.Scale.MinAuto = true;
            ZedGraphPane.YAxis.Scale.MaxAuto = true;
            ZedGraphPane.YAxis.Scale.MajorStepAuto = true;
            ZedGraphPane.YAxis.Scale.MinorStepAuto = true;
            //ZedGraphPane.YAxis.Scale.MagAuto = true;
            //ZedGraphPane.Y2Axis.Scale.MagAuto = true;

            ZedGraphPane.Margin.Left = 0;
            ZedGraphPane.Margin.Right = 0;
            ZedGraphPane.Margin.Top = 5;
            ZedGraphPane.Margin.Bottom = 0;
            //ZedGraphPane.IsFontsScaled = true;
            ZedGraphPane.Title.FontSpec.Size = 0.1f * (this.Size.Width / 100);
            ZedGraphPane.TitleGap = -0.5f;

            ZedGraphPane.Legend.Gap = 1;
            ZedGraphPane.Legend.FontSpec.Size = 0.4f * (this.Size.Width / 100);

            ZedGraphPane.XAxis.MajorTic.Color = Color.OrangeRed;
            ZedGraphPane.YAxis.MajorTic.Color = Color.OrangeRed;

            la = new RollingPointPairList[16];
            for (int i = 0; i < 16; i++)
            {
                la[i] = new RollingPointPairList(1000);
                la[i].Clear();
            }

            cbDebug1.BackColor = Color.Green;
            cbDebug2.BackColor = Color.Red;
            cbDebug3.BackColor = Color.Blue;
            cbDebug4.BackColor = Color.YellowGreen;
            cbDebug5.BackColor = Color.RosyBrown;
            cbDebug6.BackColor = Color.BlueViolet;
            cbDebug7.BackColor = Color.DarkGreen;
            cbDebug8.BackColor = Color.Brown;
            cbDebug9.BackColor = Color.DarkOrchid;
            cbDebug10.BackColor = Color.DarkBlue;
            cbDebug11.BackColor = Color.BurlyWood;
            cbDebug12.BackColor = Color.DarkBlue;
            cbDebug13.BackColor = Color.DarkCyan;
            cbDebug14.BackColor = Color.DarkGoldenrod;
            cbDebug15.BackColor = Color.DarkGreen;
            cbDebug16.BackColor = Color.DarkMagenta;

            cbDebug1.ForeColor = Color.Green;
            cbDebug2.ForeColor = Color.Red;
            cbDebug3.ForeColor = Color.Blue;
            cbDebug4.ForeColor = Color.YellowGreen;
            cbDebug5.ForeColor = Color.RosyBrown;
            cbDebug6.ForeColor = Color.BlueViolet;
            cbDebug7.ForeColor = Color.DarkGreen;
            cbDebug8.ForeColor = Color.Brown;
            cbDebug9.ForeColor = Color.DarkOrchid;
            cbDebug10.ForeColor = Color.DarkBlue;
            cbDebug11.ForeColor = Color.BurlyWood;
            cbDebug12.ForeColor = Color.DarkBlue;
            cbDebug13.ForeColor = Color.DarkCyan;
            cbDebug14.ForeColor = Color.DarkGoldenrod;
            cbDebug15.ForeColor = Color.DarkGreen;
            cbDebug16.ForeColor = Color.DarkMagenta;

            LineItem curve0 = ZedGraphPane.AddCurve("D1", la[0], Color.Green, SymbolType.None);
            LineItem curve1 = ZedGraphPane.AddCurve("D2", la[1], Color.Red, SymbolType.None);
            LineItem curve2 = ZedGraphPane.AddCurve("D3", la[2], Color.Blue, SymbolType.None);
            LineItem curve3 = ZedGraphPane.AddCurve("D4", la[3], Color.YellowGreen, SymbolType.None);
            LineItem curve4 = ZedGraphPane.AddCurve("D5", la[4], Color.RosyBrown, SymbolType.None);
            LineItem curve5 = ZedGraphPane.AddCurve("D6", la[5], Color.BlueViolet, SymbolType.None);
            LineItem curve6 = ZedGraphPane.AddCurve("D7", la[6], Color.DarkGreen, SymbolType.None);
            LineItem curve7 = ZedGraphPane.AddCurve("D8", la[7], Color.Brown, SymbolType.None);
            LineItem curve8 = ZedGraphPane.AddCurve("D9", la[8], Color.DarkOrchid, SymbolType.None);
            LineItem curve9 = ZedGraphPane.AddCurve("D10", la[9], Color.DarkBlue, SymbolType.None);
            LineItem curve10 = ZedGraphPane.AddCurve("D11", la[10], Color.BurlyWood, SymbolType.None);
            LineItem curve11 = ZedGraphPane.AddCurve("D12", la[11], Color.DarkBlue, SymbolType.None);
            LineItem curve12 = ZedGraphPane.AddCurve("D13", la[12], Color.DarkCyan, SymbolType.None);
            LineItem curve13 = ZedGraphPane.AddCurve("D14", la[13], Color.DarkGoldenrod, SymbolType.None);
            LineItem curve14 = ZedGraphPane.AddCurve("D15", la[14], Color.DarkGreen, SymbolType.None);
            LineItem curve15 = ZedGraphPane.AddCurve("D16", la[15], Color.DarkMagenta, SymbolType.None);


            for (int i = 0; i < 16; i++)
            {
                LineItem curve = ZedGraphPane.CurveList[i] as LineItem;

                curve.Line.Width = 2.0f;
                curve.Line.IsSmooth = true;
                curve.Line.SmoothTension = 0.2f;

                if (cbGraphArray[i].Checked)
                {
                    curve.Line.IsVisible = true;
                }
                else
                    curve.Line.IsVisible = false;
            }

            zedGraphControl1.GraphPane = ZedGraphPane;
            //zedGraphControl1.Refresh();
            zedGraphControl1.AxisChange();
        }
        public static void StructToBytes(object obj, ref byte[] packet)
        {
            int size = Marshal.SizeOf(obj);
            packet = new byte[size];
            IntPtr buffer = Marshal.AllocHGlobal(size + 1);
            Marshal.StructureToPtr(obj, buffer, false);
            Marshal.Copy(buffer, packet, 0, size);
            Marshal.FreeHGlobal(buffer);
        }

        public static void BytesToStructure(byte[] bValue, ref object obj, Type t)
        {
            int size = Marshal.SizeOf(t);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(bValue, 0, buffer, size);
            obj = Marshal.PtrToStructure(buffer, t);
            Marshal.FreeHGlobal(buffer);
        }


        ADS2OFP_eADC_STRUCT recv_data;

        public static string Packet_to_Str(ADS2OFP_eADC_STRUCT packet)
        {
            string str = "";
            try
            {
                Int32 xx;

                str += "=\"" + DateTime.Now.ToString() + '"' + ',';

                str += "=\"" + packet.CMD_Counter.ToString() + '"' + ',';
                str += "=\"" + packet.AirPosVel[0].ToString() + '"' + ',';
                str += "=\"" + packet.AirPosVel[1].ToString() + '"' + ',';
                str += "=\"" + packet.AirPosVel[2].ToString() + '"' + ',';
                str += "=\"" + packet.AirData_Raw[0].ToString() + '"' + ',';
                str += "=\"" + packet.AirData_Raw[1].ToString() + '"' + ',';
                str += "=\"" + packet.PTPStemp[0].ToString() + '"' + ',';
                str += "=\"" + packet.PTPStemp[1].ToString() + '"' + ',';
                str += "=\"" + packet.bit[0].ToString() + '"' + ',';
                str += "=\"" + packet.bit[1].ToString() + '"' + ',';
                str += "=\"" + packet.bit[2].ToString() + '"' + ',';


                str += "PacketEnd";
                str += Environment.NewLine;
            }
            catch (Exception)
            {

            }
            return str;
        }
        public static string LogPacket_to_Str(GCS2ADS_LOGDATA packet)
        {
            string str = "";
            //try
            //{
            //    str += "=\"" + packet.TIME.ToString() + '"' + ',';

            //    for (int i = 0; i < 8; i++)
            //    {
            //        if ((packet.BMP390_FATALERROR_CODE & (0b_00000001 << i)) == 1)
            //        {
            //            str += "=\"" + "1" + '"' + ',';
            //        }
            //        else
            //            str += "=\"" + "0" + '"' + ',';
            //    }

            //    str += "=\"" + packet.BMP390_PRESSURE_Raw_01.ToString() + '"' + ',';
            //    str += "=\"" + packet.BMP390_PRESSURE_Raw_02.ToString() + '"' + ',';
            //    str += "=\"" + packet.BMP390_PRESSURE_Raw_03.ToString() + '"' + ',';
            //    str += "=\"" + packet.BMP390_PRESSURE_Raw_04.ToString() + '"' + ',';
            //    str += "=\"" + packet.BMP390_PRESSURE_Raw_05.ToString() + '"' + ',';
            //    str += "=\"" + packet.BMP390_PRESSURE_Raw_06.ToString() + '"' + ',';
            //    str += "=\"" + packet.BMP390_PRESSURE_Raw_07.ToString() + '"' + ',';
            //    str += "=\"" + packet.BMP390_PRESSURE_Raw_08.ToString() + '"' + ',';

            //    str += "=\"" + packet.Pressure_Static_Raw.ToString() + '"' + ',';
            //    str += "=\"" + packet.Pressure_Differential_Raw.ToString() + '"' + ',';

            //    str += "=\"" + packet.Altitude.ToString() + '"' + ',';
            //    str += "=\"" + packet.Velocity.ToString() + '"' + ',';

            //    str += "=\"" + packet.AOA_Raw.ToString() + '"' + ',';
            //    str += "=\"" + packet.AOS_Raw.ToString() + '"' + ',';

            //    str += Environment.NewLine;
            //}
            //catch (Exception)
            //{

            //}
            return str;
        }



        public void ThreadParsing()
        {

            byte[] receiveData = new byte[1024];
            int Qsize = Que.Count;
            int parsingstep = 0;
            byte data_type = 0;
            int data_size = 0;
            int data_recv_cnt = 0;
            byte checksum = 0;

            data_size = 0;

            while (DATARUNFLAG)
            {
                Qsize = Que.Count;
                //data read sequence
                if (Qsize > 0)
                {
                    if (parsingstep == 0)
                    {
                        lock (this)
                        {
                            byte tmp = Que.Dequeue();
                            if (tmp == 'A')
                            {
                                receiveData[parsingstep] = (byte)'A';
                                parsingstep = 1;
                            }
                            else if (tmp == 'L')
                            {
                                receiveData[parsingstep] = (byte)'L';
                                parsingstep = 1;
                            }
                        }
                    }
                    else if (parsingstep == 1)
                    {
                        lock (this)
                        {
                            byte tmp = Que.Dequeue();
                            if (tmp == 'O')
                            {
                                receiveData[parsingstep] = (byte)'O';
                                parsingstep = 2;
                                data_size = Marshal.SizeOf(typeof(ADS2OFP_eADC_STRUCT));
                                checksum = 0;
                            }
                            else if (tmp == 'G')
                            {
                                receiveData[parsingstep] = (byte)'G';
                                parsingstep = 2;
                                data_size = Marshal.SizeOf(typeof(GCS2ADS_LOGDATA));
                            }
                            else
                            {
                                checksum = 0;
                                parsingstep = 0;
                            }
                        }
                    }
                    else if (parsingstep == 2)
                    {
                        lock (this)
                        {
                            data_type = Que.Dequeue();
                        }

                        receiveData[parsingstep] = data_type;
                        checksum ^= data_type;

                        parsingstep = 3;
                    }
                    else if (parsingstep < (data_size) - 1)
                    {
                        lock (this)
                        {
                            receiveData[parsingstep] = Que.Dequeue();
                        }

                        checksum ^= receiveData[parsingstep];
                        parsingstep++;

                    }
                    else
                    {
                        lock (this)
                        {
                            checksum ^= Que.Dequeue();
                        }

                        // packet validate is ok?
                        //if (checksum == receiveData[data_recv_cnt])
                        if ((checksum == 0) && (receiveData[0] == 'A') && (receiveData[1] == 'O'))
                        //if  ((receiveData[0] == 'A') && (receiveData[1] == 'O'))
                        {
                            // 원본
                            lock (this)
                            {
                                object data = new object();

                                BytesToStructure(receiveData, ref data, typeof(ADS2OFP_eADC_STRUCT));
                                //PACKET_DEBUG_INFO recv_data = (PACKET_DEBUG_INFO)data;

                                ADS2OFP_eADC_STRUCT recv_data = (ADS2OFP_eADC_STRUCT)data;

                                siDataClass.debug[0] = (float)recv_data.AirPosVel[0];
                                siDataClass.debug[1] = (float)recv_data.AirPosVel[1];
                                siDataClass.debug[2] = (float)recv_data.AirPosVel[2];

                                siDataClass.debug[3] = (float)recv_data.AirData_Raw[0] / 100.0f;
                                siDataClass.debug[4] = (float)recv_data.AirData_Raw[1] / 100.0f;

                                siDataClass.debug[5] = (float)recv_data.PTPStemp[0];
                                siDataClass.debug[6] = (float)recv_data.PTPStemp[1];

                                siDataClass.debug[7] = (float)recv_data.bit[0];
                                siDataClass.debug[8] = (float)recv_data.bit[1];
                                siDataClass.debug[9] = (float)recv_data.bit[2];

                                File_save.file_save_recv(Packet_to_Str(recv_data));
                            }
                            System.Array.Clear(receiveData, 0, receiveData.Length);
                            checksum = 0;
                        }
                        else if ((checksum == 0) && (receiveData[0] == 'L') && (receiveData[1] == 'G'))
                        {
                            object data = new object();

                            BytesToStructure(receiveData, ref data, typeof(GCS2ADS_LOGDATA));
                            LOG4CALIreceived = (GCS2ADS_LOGDATA)data;                            

                            //File_save.Log_File_save(LogPacket_to_Str(recv_data));
                        }
                        else
                        {
                            checksum = 0;
                        }
                        parsingstep = 0;
                        data_recv_cnt = 0;
                    }
                }
                else Thread.Sleep(10);
            }
        }

        public void SerialPort_Init()
        {
            serialPort1.BaudRate = 115200;
            serialPort1.StopBits = System.IO.Ports.StopBits.Two;
        }

        private void cbComportSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
        }

        private void cbComportSel_MouseDown(object sender, MouseEventArgs e)
        {
            string[] strPort = SerialPort.GetPortNames();
            cbComportSel.Items.Clear();

            foreach (string number in strPort)
            {
                cbComportSel.Items.Add(number);
            }
            cbComportSel.Text = serialPort1.PortName;
        }

        private void btPortOpen_Click(object sender, EventArgs e)
        {
            try
            {
                //label_save();
                if (!serialPort1.IsOpen)
                {
                    this.serialPort1.PortName = cbComportSel.SelectedItem.ToString();

                    tickStart = Environment.TickCount - tickPause + tickStart;

                    serialPort1.Open();
                    if (serialPort1.IsOpen)
                    {
                        tbProcessLog.Text = this.serialPort1.PortName + " Port Opened \r\n";
                        btPortOpen.Text = "Disconnect";

                        timer1.Start();

                        File_save.FDR_File_name();
                    }
                    else
                    {
                        tbProcessLog.Text = this.serialPort1.PortName + " Port Open Fail! \r\n";
                    }
                }
                else
                {
                    serialPort1.Close();
                    btPortOpen.Text = "Connect";
                    tickPause = Environment.TickCount;
                    if (serialPort1.IsOpen)
                    {
                        tbProcessLog.Text = this.serialPort1.PortName + " Port Closed Fail ! \r\n";
                    }
                    else
                    {
                        tbProcessLog.Text = this.serialPort1.PortName + " Port Closed \r\n";
                        //trycount = 0;

                        //if (tbFileName.Enabled == false)
                        //tbFileName.Enabled = true;
                    }

                    timer1.Stop();
                }
                tbProcessLog.SelectionStart = tbProcessLog.Text.Length;
                tbProcessLog.ScrollToCaret();
                //PortStatusCheck();
            }
            catch (Exception)
            {
                tbProcessLog.Text = "Port Connect Error \r\n";
                tbProcessLog.Text += "Check Your Connection \r\n";
                tbProcessLog.SelectionStart = tbProcessLog.Text.Length;
                tbProcessLog.ScrollToCaret();
                return;
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    int BytesToRead = serialPort1.BytesToRead;
                    if (BytesToRead > 0)
                    {
                        byte[] array = new byte[BytesToRead];
                        serialPort1.Read(array, 0, BytesToRead);
                        lock (this)
                        {
                            this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { array });
                            //this.Invoke(new SetTextDeleg(si_DataReceived), new object[] { array });
                        }
                    }
                }
            }
            catch (Exception) { return; }
        }


        private void si_DataReceived(byte[] array)
        {
            //data receive
            try
            {
                if (serialPort1.IsOpen)
                {
                    //int BytesToRead = serialPort1.BytesToRead;
                    foreach (byte a in array)
                    {
                        lock (this)
                        {
                            Que.Enqueue(a);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void SerialGraph_FormClosing(object sender, FormClosingEventArgs e)
        {
            DATARUNFLAG = false;

        }

        int tickStart = 0, tickPause = 0;


        private void graph_enable(object sender)
        {
            int num = 0;
            foreach (CheckBox cb in cbGraphArray)
            {
                if (cb.Name.Equals(((CheckBox)sender).Name))
                {
                    break;
                }
                else
                {
                    num++;
                }
            }
            //cbGraphArray

            LineItem curve = zedGraphControl1.GraphPane.CurveList[num] as LineItem;

            if (cbGraphArray[num].Checked)
            {
                curve.Line.IsVisible = true;
            }
            else
                curve.Line.IsVisible = false;

        }

        private void cbDebug1_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug2_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug3_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug4_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug5_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug6_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug7_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug8_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug9_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug10_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug11_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug12_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug13_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug14_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug15_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }
        private void cbDebug16_CheckedChanged(object sender, EventArgs e) { graph_enable(sender); }



        public void Send_uart_Cali_CMD_data(GCS2ADS_LOGDATA str)
        {
            byte[] packet_send_wrtite_data = new byte[Marshal.SizeOf(typeof(GCS2ADS_LOGDATA))];
            byte[] packet_send_wrtite_data_s = new byte[packet_send_wrtite_data.Length];
            // 데이타 변환
            StructToBytes(str, ref packet_send_wrtite_data);

            if (serialPort1.IsOpen)
            {
                // packet_send_wrtite_data_s[0] = 0xff;  //dummy
                packet_send_wrtite_data_s[0] = 0x4C;  //'L'
                packet_send_wrtite_data_s[1] = 0x47;  //'G'

                for (int i = 2; i < packet_send_wrtite_data.Length - 1; i++)
                {
                    packet_send_wrtite_data_s[i] = packet_send_wrtite_data[i];
                    str.Checksum ^= packet_send_wrtite_data[i];
                }


                packet_send_wrtite_data[packet_send_wrtite_data.Length - 1] = str.Checksum;
                packet_send_wrtite_data_s[packet_send_wrtite_data_s.Length - 1] = str.Checksum;

                serialPort1.Write(packet_send_wrtite_data_s, 0, packet_send_wrtite_data.Length);
            }
        }

        public void Send_uart_SerialNo_CMD_data(GCS2ADS_LOGDATA str)
        {
            byte[] packet_send_wrtite_data = new byte[Marshal.SizeOf(typeof(GCS2ADS_LOGDATA))];
            byte[] packet_send_wrtite_data_s = new byte[packet_send_wrtite_data.Length];
            // 데이타 변환
            StructToBytes(str, ref packet_send_wrtite_data);

            if (serialPort1.IsOpen)
            {
                // packet_send_wrtite_data_s[0] = 0xff;  //dummy
                packet_send_wrtite_data_s[0] = 0x4C;  //'L'
                packet_send_wrtite_data_s[1] = 0x48;  //'H'

                for (int i = 2; i < packet_send_wrtite_data.Length - 1; i++)
                {
                    packet_send_wrtite_data_s[i] = packet_send_wrtite_data[i];
                    str.Checksum ^= packet_send_wrtite_data[i];
                }

                packet_send_wrtite_data[packet_send_wrtite_data.Length - 1] = str.Checksum;
                packet_send_wrtite_data_s[packet_send_wrtite_data_s.Length - 1] = str.Checksum;

                serialPort1.Write(packet_send_wrtite_data_s, 0, packet_send_wrtite_data.Length);
            }
        }

        GCS2ADS_LOGDATA LOG4CALI = new GCS2ADS_LOGDATA() { BasePressure = new float[2], SerialNo = new byte[32] };
        GCS2ADS_LOGDATA LOG4CALIreceived = new GCS2ADS_LOGDATA() { BasePressure = new float[2], SerialNo = new byte[32] };

        private void btn_cali_Click(object sender, EventArgs e)
        {
            float temp = 0.0f;
            float.TryParse(tb_basepressure.Text.ToString(), out temp);
            LOG4CALI.BasePressure[0] = temp;

            Send_uart_Cali_CMD_data(LOG4CALI);
        }

        private void bt_serialNo_Click(object sender, EventArgs e)
        {
            byte[] temp = Encoding.UTF8.GetBytes(tb_serialNo.Text);

            // SerialNo는 이미 32바이트 배열이라고 가정
            Array.Clear(LOG4CALI.SerialNo, 0, LOG4CALI.SerialNo.Length); // 먼저 0으로 초기화
            Array.Copy(temp, LOG4CALI.SerialNo, Math.Min(temp.Length, LOG4CALI.SerialNo.Length));


            Send_uart_SerialNo_CMD_data(LOG4CALI);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    double temp = 0;

                    for (int loop = 0; loop < 7; loop++)
                    {
                        if (xxarray[loop].Text != "")
                        {
                            temp = Convert.ToDouble(xxarray[loop].Text) * siDataClass.debug[loop];
                            txarray[loop].Text = temp.ToString("N2");
                        }
                        else
                        {
                            txarray[loop].Text = "배율입력>>";
                        }
                    }

                    var a = Convert.ToString((byte)siDataClass.debug[7], 2).PadLeft(8, '0');
                    txarray[14].Text = a;
                    var b = Convert.ToString((byte)siDataClass.debug[8], 2).PadLeft(8, '0');
                    txarray[15].Text = b;
                    var c = Convert.ToString((byte)siDataClass.debug[9], 2).PadLeft(8, '0');
                    txarray[16].Text = c;

                    tb_serialNoInADC.Text = LOG4CALIreceived.SerialNo.ToString();
                    tb_serialNoInADC.Text = Encoding.UTF8.GetString(LOG4CALIreceived.SerialNo);

                    tb_basepressureInADC.Text = LOG4CALIreceived.BasePressure[0].ToString("N2");


                    LineItem[] curveArray = new LineItem[16];

                    for (int loop = 0; loop < 16; loop++)
                    {
                        curveArray[loop] = zedGraphControl1.GraphPane.CurveList[loop] as LineItem;

                        if (curveArray[loop] == null) return;
                    }

                    // Get the PointPairList
                    IPointListEdit[] listArray = new IPointListEdit[17];
                    for (int loop = 0; loop < 16; loop++)
                    {
                        listArray[loop] = curveArray[loop].Points as IPointListEdit;
                    }

                    double time = (double)((Environment.TickCount - tickStart) / 1000.0);

                    for (int loop = 0; loop < 13; loop++)
                    {
                        listArray[loop].Add(time, siDataClass.debug[loop] * Convert.ToDouble(xxarray[loop].Text));
                    }

                    //tbTemp.Text+= "list1.Tostring = "+  list1.ToString(siDataClass.Culture);

                    //lbDEBUG.Text = Math.Sin((double)time * Math.PI / 10.0).ToString(siDataClass.Culture);
                    // Keep the X scale at a rolling 30 second interval, with one
                    // major step between the max X value and the end of the axis
                    Scale xScale1 = zedGraphControl1.GraphPane.XAxis.Scale;

                    if (time > xScale1.Max - xScale1.MajorStep)
                    {
                        xScale1.Max = time + xScale1.MajorStep;
                        xScale1.Min = xScale1.Max - 15.0;
                    }

                    // Make sure the Y axis is rescaled to accommodate actual data
                    zedGraphControl1.AxisChange();
                    // Force a redraw
                    zedGraphControl1.Invalidate();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
