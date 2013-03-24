using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler.Test.Fakes
{
	public class FakeHandler
	{

		public dynamic OneStringParam(string param1)
		{
			return new { param1 };
		}

		public dynamic TwoStringParam(string param1, string param2)
		{
			return new { param1, param2 };
		}

		public dynamic MultiTypeParam(string param1, int param2, double param3, bool param4, DateTime param5, string[] param6, FakeObject param7)
		{
			return new { param1, param2, param3, param4, param5, param6, param7 };
		}

		public dynamic SingleObjectParam(FakeObject param1)
		{
			return param1;
		}


	}
}
