using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using RhosynsEnder3V3SELibrary.Models;

namespace RhosynsEnder3V3SELibrary.Systems
{
    /// <summary>
    /// COM system for the Ender 3 V3 SE
    /// </summary>
    public class COMSystem : ISerialPortManager
    {
        public static COMSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new COMSystem();
                }
                return _instance;
            }
        }
        private static COMSystem _instance;

        public SerialPort SerialPort
        {
            get
            {
                if (_serialPort == null)
                {
                    throw new InvalidOperationException("SerialPort is not initialized. Please set the SerialPort property before accessing it.");
                }
                return _serialPort;
            }
        }
        private SerialPort _serialPort;

        /// <summary>
        /// Initializes a new COMSystem with a referenced SerialPort.
        /// </summary>
        /// <param name="port"></param>
        public COMSystem(ref SerialPort port)
        {
            _instance = this;
            SetupSerialPort(ref port);
        }

        /// <summary>
        /// Initializes a new COMSystem class without a referenced SerialPort. The SerialPort must be set up later using SetupSerialPort method.
        /// </summary>
        [Obsolete("Use COMSystem(ref SerialPort port) instead.")]
        public COMSystem()
        {
            _instance = this;
        }

        [Obsolete("Use COMSystem(ref SerialPort port) instead.")]
        public static SerialPort SetupSerialPort(ref SerialPort port)
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("COMSystem instance is not initialized. Please create an instance of COMSystem before setting up the SerialPort.");
            }
            _instance._serialPort = port;
            return _instance._serialPort;
        }

        public void Open() => _serialPort.Open();
        public void Close() => _serialPort.Close();
        public bool IsOpen => _serialPort.IsOpen;
        public void WriteLine(string command) => _serialPort.WriteLine(command);
        public string ReadLine() => _serialPort.ReadLine();
        public int BytesToRead => _serialPort.BytesToRead;
        public event SerialErrorReceivedEventHandler ErrorReceived
        {
            add { _serialPort.ErrorReceived += value; }
            remove { _serialPort.ErrorReceived -= value; }
        }

        public void Dispose() => _serialPort?.Dispose();
    }
}
