using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class Script
  {

    public IList<IStmt> commands { get; set; }


    public Script()
    {
      commands = new List<IStmt>();
    }

  }
}
