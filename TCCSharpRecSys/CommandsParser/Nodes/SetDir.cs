using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class SetDir : IStmt
  {
    public string path { get; private set; }

    public SetDir(string path)
    {
      if (path == null)
        throw new ArgumentException("path");

      this.path = path;
    }

    public bool checkSemantics(IList<IStmt> previousStatements)
    {
      return true;
    }
  }
}
