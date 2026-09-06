using System.Text;

namespace RhosynsEnder3V3SELibrary.Models
{
    /// <summary>
    /// A print job
    /// </summary>
    public class PrintJob
    {
        private readonly GCode gcode;
        private readonly Ender3V3SE printer;

        /// <summary>
        /// Create a new print job with the given GCode and printer
        /// </summary>
        /// <param name="gcode"></param>
        /// <param name="printer"></param>
        public PrintJob(GCode gcode, Ender3V3SE printer)
        {
            this.gcode = gcode;
            this.printer = printer;
        }

        /// <summary>
        /// Executes the print job
        /// </summary>
        /// <param name="onByte"></param>
        /// <returns></returns>
        public async Task<string> ExecuteAsync(Action<string>? onByte = null)
        {
            var response = new StringBuilder();
            foreach (var cmd in gcode.Commands)
            {
                Console.WriteLine($"Sending line {cmd.LineNumber}: {cmd.CommandText}");
                var lineResponse = await printer.SendCommand(cmd.CommandText, onByte);
                response.AppendLine(lineResponse);
            }
            return response.ToString();
        }
    }
}