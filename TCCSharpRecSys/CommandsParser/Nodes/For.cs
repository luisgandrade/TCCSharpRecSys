using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class For : IStmt
  {

    public int init_range { get; private set; }

    public int end_range { get; private set; }

    public IList<IStmt> stmts { get; private set; }


    public For(int init_range, int end_range, IList<IStmt> stmts)
    {
      if (init_range <= 0 || end_range <= 0)
        throw new InvalidOperationException("Ambos o início e fim do range precisam ser maiores que zero.");
      if (end_range < init_range)
        throw new InvalidOperationException("O fim do range não pode ser menor que o início do range.");

      this.init_range = init_range;
      this.end_range = end_range;
      this.stmts = stmts;
    }

    public bool checkSemantics(IList<IStmt> previousStatements)
    {
      throw new NotImplementedException();
    }
  }
}
