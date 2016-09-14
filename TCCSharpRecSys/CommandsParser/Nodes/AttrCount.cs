using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class AttrCount : IStmt
  {
    public int attr_count { get; set; }

    public AttrCount(int attr_count)
    {
      if (attr_count <= 0)
        throw new InvalidOperationException("A quantidade de atributos deve ser maior que zero.");

      this.attr_count = attr_count;
    }

    public bool checkSemantics(IList<IStmt> previousStatements)
    {
      return true;
    }
  }
}
