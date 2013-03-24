using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler.Test
{
	using NUnit.Framework;

	using Newtonsoft.Json.Linq;

	using SimpleHttpHandler.Test.Fakes;

	[TestFixture]
	public class MethodInfoWrapperTest
	{
		[Test]
		public void OneStringParam()
		{
			// Arrange
			var handler = new FakeHandler();
			var method = handler.GetType().GetMethod("OneStringParam");
			var miw = new MethodInfoWrapper<FakeHandler>(method);
			var input = new { param1 = "value1" };

			// Act
			miw.BindValues(JObject.FromObject(input));
			dynamic output = miw.Invoke(handler);

			// Assert
			Assert.AreEqual(1, miw.MatchedParameters());
			Assert.AreEqual(input.param1, output.param1);
		}

		[Test]
		public void MapObjectToSingleStringParam_NoParameterShouldMatch()
		{
			// Arrange
			var handler = new FakeHandler();
			var method = handler.GetType().GetMethod("OneStringParam");
			var miw = new MethodInfoWrapper<FakeHandler>(method);
			var input = new { test1 = "value1", test2 = "value2" };

			// Act
			miw.BindValues(JObject.FromObject(input));
			dynamic output = miw.Invoke(handler);

			// Assert
			Assert.AreEqual(0, miw.MatchedParameters());
		}

		[Test]
		public void SingleObjectParam()
		{
			// Arrange
			var handler = new FakeHandler();
			var method = handler.GetType().GetMethod("SingleObjectParam");
			var miw = new MethodInfoWrapper<FakeHandler>(method);
			var input = new FakeObject
				           {
					           Prop1 = "val1",
					           Prop2 = 20,
					           Prop3 = 20.5,
					           Prop4 = true,
					           Prop5 = DateTime.Now,
					           Prop6 = new string[] { "va1", "val2", "val3" }
				           };


			// Act
			miw.BindValues(JObject.FromObject(input));
			dynamic output = miw.Invoke(handler);

			// Assert
			Assert.AreEqual(1, miw.MatchedParameters());
			Assert.AreEqual(input, output);
		}

		

		[Test]
		public void MultiTypeParam()
		{
			// Arrange
			var handler = new FakeHandler();
			var method = handler.GetType().GetMethod("MultiTypeParam");
			var miw = new MethodInfoWrapper<FakeHandler>(method);
			var input = new {
							param1 = "value1",
					        param2 = 10,
					        param3 = 10.50 ,
					        param4 = true ,
					        param5 = DateTime.Now ,
					        param6 = new List<string> { "value1", "value2", "value3" },
							param7 = new FakeObject
								         {
									         Prop1 = "val1",
											 Prop2 = 20,
											 Prop3 = 20.5,
											 Prop4 = true,
											 Prop5 = DateTime.Now,
											 Prop6 = new string[] { "va1", "val2", "val3" }
								         }
				           };

			// Act
			miw.BindValues(JObject.FromObject(input));
			dynamic output = miw.Invoke(handler);

			// Assert
			Assert.AreEqual(7, miw.MatchedParameters());
			Assert.AreEqual(input.param1, output.param1);
			Assert.AreEqual(input.param2, output.param2);
			Assert.AreEqual(input.param3, output.param3);
			Assert.AreEqual(input.param4, output.param4);
			Assert.AreEqual(input.param5, output.param5);
			Assert.AreEqual(input.param6, output.param6);
			Assert.AreEqual(input.param7, output.param7);
		}

	}
}
