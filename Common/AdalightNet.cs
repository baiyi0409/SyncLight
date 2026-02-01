using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLight.Common
{
    public sealed class Adalight : IDisposable
    {
        //亮度
        private readonly int _brightness;
        
        //LED矩阵
        private readonly Color[] _ledMatrix;
        
        //COM端口
        private readonly SerialPort _comPort;
        
        //串口数据
        private readonly byte[] _serialData;
        
        //解释数据
        private string _line;
        
        /// <summary>
        /// 是否连接
        /// </summary>
        public bool Connected;
        
        //是否发送
        private bool _sending;
        
        //线程同步机制
        private AutoResetEvent _dataReceived;
        
        //是否释放
        private bool _disposedValue;

        /// <summary>
        /// COM端口
        /// </summary>
        public string Port { get; }

        /// <summary>
        /// 灯珠数量
        /// </summary>
        public int LedCount { get; }

        /// <summary>
        /// 波特率
        /// </summary>
        public int Speed { get; }


        public Adalight(string port, int ledCount, int speed = 115200, int brightness = 255)
        {

            _brightness = brightness;
            LedCount = ledCount;
            Port = port;
            Speed = speed;
            _ledMatrix = new Color[ledCount];

            for (int index = 0; index < ledCount; ++index) 
            {
                _ledMatrix[index] = Color.FromArgb(0, 0, 0);
            }

            _serialData = new byte[6 + ledCount * 3 + 1];

            //_dataReceived = new AutoResetEvent(false);


            try
            {
                _comPort = new SerialPort
                {
                    PortName = port,
                    BaudRate = speed,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One
                };

                //_comPort.DataReceived += ParseMessage;
            }
            catch (Exception ex)
            {
                //Ignored
            }
        }

        #region 打开链接、关闭链接

        public bool Connect()
        {
            try
            {
                _comPort.Open();
                Connected = _comPort.IsOpen;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception connecting to port: " + ex.Message);
                return false;
            }
        }

        public bool Disconnect(bool reset = true)
        {
            if (!Connected)
                return false;
            try
            {
                if (reset)
                {
                    for (int index = 0; index < LedCount; ++index)
                    {
                        UpdatePixel(index, Color.Black, false);
                    }
                    Update();
                }

                _comPort.Close();
                Connected = false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception closing port: " + ex.Message);
                return false;
            }
        }

        #endregion

        //解析返回数据
        private void ParseMessage(object sender, SerialDataReceivedEventArgs e)
        {
            _line = _comPort.ReadLine();
            if (_line.Contains("Ada") || _line.Contains("Adb"))
            {
                _line = _line.Replace("Adb", "");
                _line = _line.Replace("Ada", "");
                _dataReceived.Set();
            }
            else
                _line = "";
        }

        #region 更新颜色，把颜色集合赋值给ledMatix

        public void UpdateColors(List<Color> colors, bool update = true)
        {
            for (int index = 0; index < LedCount; ++index)
            {
                Color color = Color.FromArgb(0, 0, 0);
                if (index < colors.Count) 
                {
                    color = colors[index];
                }
                _ledMatrix[index] = color;
            }

            if (!update)
                return;
            Update();
        }

        public async Task UpdateColorsAsync(List<Color> colors, bool update = true)
        {
            for (int index = 0; index < LedCount; ++index)
            {
                Color color = Color.FromArgb(0, 0, 0);
                if (index < colors.Count) 
                {
                    color = colors[index];
                }
                _ledMatrix[index] = color;
            }

            if (!update)
                return;
            await UpdateAsync();
        }

        #endregion

        #region 更新灯光亮度

        /// <summary>
        /// 更新灯光亮度
        /// </summary>
        /// <param name="brightness"></param>
        public void UpdateBrightness(int brightness)
        {
            while (_sending)
                Task.Delay(1);
            if (brightness < 0 || brightness > byte.MaxValue)
                return;
            _sending = true;
            byte[] buffer = {
                Convert.ToByte("Adb"[0]),
                Convert.ToByte("Adb"[1]),
                Convert.ToByte("Adb"[2]),
                Convert.ToByte("BR"[0]),
                Convert.ToByte("BR"[1]),
                (byte) brightness
            };
            _comPort.Write(buffer, 0, buffer.Length);
            _sending = false;
        }

        #endregion

        #region 更新单个灯光

        /// <summary>
        /// 更新单个灯光
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        /// <param name="update"></param>
        public void UpdatePixel(int index, Color color, bool update = true)
        {
            if (index < LedCount)
            {
                _ledMatrix[index] = color;
            }

            if (!update)
                return;
            Update();
        }

        #endregion

        #region 重点！ 写入数据，控制设备 

        public void Update()
        {
            if (!Connected)
                return;
            try
            {
                if (_sending)
                    return;
                _sending = true;
                WriteHeader();
                WriteMatrixToSerialData();
                _comPort.Write(_serialData, 0, _serialData.Length);
                _sending = false;
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        public async Task UpdateAsync()
        {
            if (!Connected)
                return;
            try
            {
                if (_sending)
                    return;
                _sending = true;
                WriteHeader();
                WriteMatrixToSerialData();
                await _comPort.BaseStream.WriteAsync(_serialData, 0, _serialData.Length);
                await _comPort.BaseStream.FlushAsync();
                _sending = false;
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        #endregion

        //public int[] GetState()
        //{
        //    while (_sending)
        //        Task.Delay(1);
        //    int num1 = 0;
        //    int num2 = 0;
        //    try
        //    {
        //        _sending = true;
        //        byte[] buffer = {
        //            Convert.ToByte("Adb"[0]),
        //            Convert.ToByte("Adb"[1]),
        //            Convert.ToByte("Adb"[2]),
        //            Convert.ToByte("ST"[0]),
        //            Convert.ToByte("ST"[1]),
        //            0
        //        };
        //        _comPort.Write(buffer, 0, buffer.Length);
        //        _sending = false;

        //        _dataReceived.WaitOne(1000);
        //        if (!string.IsNullOrEmpty(_line))
        //        {
        //            if (_line.Contains("N="))
        //            {
        //                foreach (string str in _line.Split(";"))
        //                {
        //                    string[] strArray = str.Split("=");
        //                    if (strArray[0] == "N")
        //                        num1 = int.Parse(strArray[1]);
        //                    if (strArray[0] == "B")
        //                        num2 = int.Parse(strArray[1]);
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Exception: " + ex.Message + " at " + ex.StackTrace);
        //    }

        //    return new[] { num1, num2 };
        //}

        #region 写入数据格式

        private void WriteHeader()
        {
            _serialData[0] = Convert.ToByte("Ada"[0]);
            _serialData[1] = Convert.ToByte("Ada"[1]);
            _serialData[2] = Convert.ToByte("Ada"[2]);
            _serialData[3] = Convert.ToByte(LedCount - 1 >> 8);
            _serialData[4] = Convert.ToByte(LedCount - 1 & byte.MaxValue);
            _serialData[5] = Convert.ToByte(_serialData[3] ^ _serialData[4] ^ 85);
        }

        private void WriteMatrixToSerialData()
        {
            int index1 = 6;
            for (int index2 = 0; index2 <= _ledMatrix.Length - 1; ++index2)
            {
                _serialData[index1] = _ledMatrix[index2].R;
                int index3 = index1 + 1;
                _serialData[index3] = _ledMatrix[index2].G;
                int index4 = index3 + 1;
                _serialData[index4] = _ledMatrix[index2].B;
                index1 = index4 + 1;
            }
        }

        #endregion

        /// <summary>
        /// 获取设备，但设备不一定带有Ada前缀
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, KeyValuePair<int, int>> FindDevices()
        {
            Dictionary<string, KeyValuePair<int, int>> dictionary = new Dictionary<string, KeyValuePair<int, int>>();
            foreach (string portName in SerialPort.GetPortNames())
            {
                try
                {
                    SerialPort serialPort = new SerialPort()
                    {
                        PortName = portName,
                        BaudRate = 115200,
                        Parity = Parity.None,
                        DataBits = 8,
                        StopBits = StopBits.One,
                        ReadTimeout = 1500
                    };
                    serialPort.Open();
                    if (serialPort.ReadLine().Substring(0, 3) == "Ada")
                        dictionary[portName] = new KeyValuePair<int, int>(0, 0);
                    serialPort.Close();
                }
                catch (Exception)
                {
                    //Ignored
                }
            }

            return dictionary;
        }

        #region 释放资源

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        if (Connected)
                            _comPort?.Close();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    _comPort?.Dispose();
                }
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// 设置设备内置灯效模式
        /// </summary>
        /// <param name="mode">模式编号（由设备固件定义，例如：0=关闭, 1=呼吸, 2=流水, 3=音乐等）</param>
        public void GetEmbeddedMode(int mode)
        {
            if (!Connected) return;
            while (_sending)
                Task.Delay(1).Wait();

            _sending = true;
            try
            {
                byte[] command = {
                    (byte)'A',
                    (byte)'d',
                    (byte)(Encoding.Unicode.GetBytes("a").First() + mode), //在这里选着光效
                };

                _comPort.Write(command, 0, 3);

                //int led_status = (byte)_comPort.ReadByte();

                //byte[] Data = new byte[6];
                //uint funSelectBit = 0u;

                //char indexOffset = '\0';
                //uint oneBase = 1u;

                //List<bool> funSelect = new List<bool>() 
                //{

                //};

                //foreach (bool item in funSelect)
                //{
                //    funSelectBit = ((!item) ? (funSelectBit & ~(oneBase << (int)indexOffset)) : (funSelectBit | (oneBase << (int)indexOffset)));
                //    indexOffset = (char)(indexOffset + 1);
                //}

                //byte[] byteArray = BitConverter.GetBytes(funSelectBit);
                //if (byteArray.Length == 4)
                //{
                //    Data[0] = Encoding.Unicode.GetBytes("A").First();
                //    Data[1] = Encoding.Unicode.GetBytes("e").First();
                //    Data[2] = byteArray[0];
                //    Data[3] = byteArray[1];
                //    Data[4] = byteArray[2];
                //    Data[5] = byteArray[3];
                //    _comPort.DiscardInBuffer();
                //    _comPort.Write(Data, 0, 6);
                //    _comPort.BaseStream.Flush();
                //    Task.Delay(1);
                //    if (_comPort.BytesToRead > 0)
                //    {
                //        led_status = (byte)_comPort.ReadByte();
                //    }
                //}
            }
            finally
            {
                _sending = false;
            }
        }
    }
}
