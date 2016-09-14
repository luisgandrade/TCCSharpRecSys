using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class Train : IStmt
  {

    public IAlgorithm algorithm { get; private set; }    

    public Train(IAlgorithm algorithm)
    {
      if (algorithm == null)
        throw new ArgumentException("algorithm");
      
      this.algorithm = algorithm;
    }

    public bool checkSemantics(IList<IStmt> previousStatements)
    {
      return !previousStatements.Any(ps => ps.GetType() == typeof(SetDir)) && !previousStatements.Any(ps => ps.GetType() == typeof(AttrCount));
    }
  }
}
