using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public class UserProfile
  {

    public int user_id { get; private set; }

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
