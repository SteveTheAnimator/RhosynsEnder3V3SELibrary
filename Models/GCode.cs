namespace RhosynsEnder3V3SELibrary.Models
{
    public class GCode
    {
        public IReadOnlyList<GCodeCommand> Commands { get; }

        /// <summary>
        /// Initializes with a list of commands.
        /// </summary>
        public GCode(IEnumerable<GCodeCommand> commands)
        {
            Commands = commands.ToList();
        }

        /// <summary>
        /// Parses G-code text into a GCode object.
        /// </summary>
        public static GCode Parse(string txt)
        {
            var ls = txt.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var Commands = new List<GCodeCommand>();
            int ln = 1;

            for (int i = 0; i < ls.Length; i++)
            {
                var l = ls[i];
                var t = l.Trim();
                if (string.IsNullOrWhiteSpace(t) || t.StartsWith(";"))
                {
                    ln++;
                    continue;
                }

                var p = t.Split(';', 2);
                var c = p[0].Trim();
                var cm = p.Length > 1 ? p[1].Trim() : null;

                if (!string.IsNullOrWhiteSpace(c))
                    Commands.Add(new GCodeCommand(c, ln, cm));

                ln++;
            }

            return new GCode(Commands);
        }

        /// <summary>
        /// Loads and parses G-code from a file
        /// </summary>
        public static async Task<GCode> LoadFromFileAsync(string path)
        {
            var txt = await File.ReadAllTextAsync(path);
            return Parse(txt);
        }
    }
}