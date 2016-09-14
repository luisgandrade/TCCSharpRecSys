using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public interface IStmt
  {
    bool checkSemantics(IList<IStmt> previousStatements);
  }
}
