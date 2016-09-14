using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class BuildUserProfile : IStmt
  {

    public double cutoff { get; private set; }


    public BuildUserProfile(double cutoff)
    {
      if (cutoff <= 0 || cutoff > 1)
        throw new ArgumentOutOfRangeException("O percentual de corte das avaliações deve ser um valor entre zero e um.");

      this.cutoff = cutoff;
    }

    public bool checkSemantics(IList<IStmt> previousStatements)
    {
      return !previousStatements.Any(ps => ps.GetType() == typeof(SetDir)) && !previousStatements.Any(ps => ps.GetType() == typeof(AttrCount));
    }
  }
}
