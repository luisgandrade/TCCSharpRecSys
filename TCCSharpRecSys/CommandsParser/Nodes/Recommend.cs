using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class Recommend : IStmt
  {
    public IAlgorithm algorithm { get; private set; }
    
    public Recommend(IAlgorithm algorithm)
    {
      if (algorithm == null)
        throw new ArgumentException("algorithm");

      this.algorithm = algorithm;
    }

    public bool checkSemantics(IList<IStmt> previousStatements)
    {
      throw new NotImplementedException();
    }
  }
}
