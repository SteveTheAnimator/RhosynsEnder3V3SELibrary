namespace RhosynsEnder3V3SELibrary.Models
{
    public class GCodeCommand
    {
        public string CommandText { get; }
        public int LineNumber { get; }
        public string? Comment { get; }

        /// <summary>
        /// Creates a new GCodeCommand instance.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="lineNumber"></param>
        /// <param name="comment"></param>
        public GCodeCommand(string commandText, int lineNumber, string? comment = null)
        {
            CommandText = commandText;
            LineNumber = lineNumber;
            Comment = comment;
        }

        public override string ToString() => CommandText;
    }
}