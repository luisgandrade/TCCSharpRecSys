using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class Classify : IStmt
  {
    public IAlgorithm algorithm { get; private set; }

    public Classify(IAlgorithm algorithm)
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
