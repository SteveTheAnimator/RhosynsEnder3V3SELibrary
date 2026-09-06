using System.IO.Ports;

namespace RhosynsEnder3V3SELibrary.Models
{
    /// <summary>
    /// Interface for managing serial port communication with the Ender 3 V3 SE
    /// </summary>
    public interface ISerialPortManager : IDisposable
    {
        void Open();
        void Close();
        bool IsOpen { get; }
        void WriteLine(string command);
        string ReadLine();
        int BytesToRead { get; }
        event SerialErrorReceivedEventHandler ErrorReceived;
    }
}