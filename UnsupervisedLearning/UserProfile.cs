using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  /// <summary>
  /// Representa um perfil de usuário.
  /// </summary>
  public class UserProfile
  {
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    public int user_id { get; private set; }
    /// <summary>
    /// As relevâncias as tags de acordo com a preferência deste usuário.
    /// </summary>
    public IList<double> profile { get; private set; }

    public UserProfile(int user_id, IList<double> profile)
    {
      if (profile == null)
        throw new ArgumentException("profile");

      this.user_id = user_id;
      this.profile = profile;
    }

  }
}
