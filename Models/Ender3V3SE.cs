using RhosynsEnder3V3SELibrary.Systems;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhosynsEnder3V3SELibrary.Models
{
    /// <summary>
    /// The actual printer itself, which is responsible for sending commands and receiving responses.
    /// </summary>
    public class Ender3V3SE : IDisposable
    {
        private const int DefaultBytesToRead = 0;
        private const int CommandTimeoutMinutes = 2;
        private const int ResponseDelayMilliseconds = 10;

        public static Ender3V3SE Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Ender3V3SE instance is not initialized. Please create an instance of Ender3V3SE before accessing it.");
                }
                return _instance;
            }
        }
        private static Ender3V3SE _instance;

        private readonly ISerialPortManager port;

        public string PortName => port?.IsOpen == true ? port.ToString() : "Port not initialized";
        public bool IsPortOpen => port?.IsOpen == true;
        public int BytesToRead => port?.IsOpen == true ? port.BytesToRead : DefaultBytesToRead;
        public string Status = "";
        public StringBuilder ResponseBuffer = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the Ender3V3SE class with the serial port manager.
        /// </summary>
        /// <param name="portManager"></param>
        public Ender3V3SE(ISerialPortManager portManager)
        {
            port = portManager;
            _instance = this;
        }

        /// <summary>
        /// Initialize the serial port and set up the error received event handler. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Initialize()
        {
            port.Open();
            port.ErrorReceived += Port_ErrorReceived;

            if (!port.IsOpen)
                throw new InvalidOperationException("failed to open the serial port.");
        }

        /// <summary>
        /// when an error is received on the serial port, this event handler will be called. It will log the error to the console.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            // handle the error
            throw new InvalidOperationException($"serial port error received: {e.EventType}");
        }

        /// <summary>
        /// Send a command to your printer and wait for a response. If no response is received within 2 minutes, a TimeoutException will be thrown.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The response from the printer as a string.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TimeoutException"></exception>
        public async Task<string> SendCommand(string command, Action<string> onByte = null)
        {
            if (port == null || !port.IsOpen)
            {
                throw new InvalidOperationException("serial port is not initialized or not open.");
            }
            port.WriteLine(command);

            var response = await Task.Run(() =>
            {
                var buffer = new StringBuilder();
                var timeout = DateTime.Now.AddMinutes(CommandTimeoutMinutes);

                while (DateTime.Now < timeout)
                {
                    if (port.BytesToRead > 0)
                    {
                        string line = port.ReadLine();
                        Status = line;
                        buffer.AppendLine(line);

                        onByte?.Invoke(line);

                        var trimmed = line.Trim().ToLowerInvariant();
                        if (trimmed == "ok" || trimmed == "k")
                        {
                            break;
                        }
                    }
                    else
                    {
                        Task.Delay(ResponseDelayMilliseconds).Wait();
                    }
                }

                if (buffer.Length == 0)
                {
                    throw new TimeoutException("no response received from the printer within the timeout period.");
                }
                ResponseBuffer = buffer;
                return buffer.ToString();
            });
            return response;
        }

        /// <summary>
        /// Send GCode to the printer line by line, waiting for a response after each line. If no response is received within 2 minutes for any line, a TimeoutException will be thrown.
        /// </summary>
        /// <param name="gcode"></param>
        /// <param name="onByte"></param>
        /// <returns></returns>
        public async Task<string> Print(GCode gcode, Action<string>? onByte = null)
        {
            var response = new StringBuilder();
            for (int i = 0; i < gcode.Commands.Count; i++)
            {
                var cmd = gcode.Commands[i];
                var lineResponse = await SendCommand(cmd.CommandText, onByte);
                response.AppendLine(lineResponse);
            }
            return response.ToString();
        }

        /// <summary>
        /// Send a GCode file to the printer line by line, waiting for a response after each line. If no response is received within 2 minutes for any line, a TimeoutException will be thrown.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="onByte"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<string> PrintFromFile(string filePath, Action<string>? onByte = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"file not found: {filePath}");

            var gcode = await GCode.LoadFromFileAsync(filePath);
            return await Print(gcode, onByte);
        }

        public void Dispose()
        {
            port?.Close();
        }
    }
}
