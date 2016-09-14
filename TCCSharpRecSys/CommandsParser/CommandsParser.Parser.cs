using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TCCSharpRecSys.CommandsParser
{
    internal partial class CommandsParserParser
    {
        public CommandsParserParser() : base(null) { }

        public void Parse(string s)
        {
            byte[] inputBuffer = System.Text.Encoding.Default.GetBytes(s);
            MemoryStream stream = new MemoryStream(inputBuffer);
            this.Scanner = new CommandsParserScanner(stream);
            this.Parse();
        }
    }
}
