namespace SimpleHttpHandler.Test
{
	using System.Collections.Generic;

	using NUnit.Framework;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using SimpleHttpHandler.ParameterSerializer;

	[TestFixture]
	public class ParamSerializerTest
	{
		[Test]
		public void DeserializeSingleDimension()
		{
			var obj = new ParamSerializer().Deserialize("a=1&b=2");

			Assert.AreEqual(obj["a"], new JValue("1"));
			Assert.AreEqual(obj["b"], new JValue("2"));
		}

		[Test]
		public void DeserializeArray()
		{
			var obj = new ParamSerializer().Deserialize("a[]=1&a[]=2");

			Assert.AreEqual(obj["a"], new JArray { "1", "2" });
		}

		[Test]
		public void DeserializeArrayWithObject()
		{
			var obj = new ParamSerializer().Deserialize("a[0][b]=1&a[1][b]=2");

			Assert.AreEqual(obj["a"], new JArray { new JObject { { "b", "1" } }, new JObject { { "b", "2" } } });
		}

		[Test]
		public void DeserializeArrayWithIndexes()
		{
			var obj = new ParamSerializer().Deserialize("a[0]=1&a[1]=2");

			Assert.AreEqual(obj["a"], new JArray { "1", "2" });
		}

		[Test]
		public void DeserializeArrayWithBadIndexes()
		{
			var obj = new ParamSerializer().Deserialize("a[5]=1&a[4]=2");

			Assert.AreEqual(obj["a"], new JArray { "1", "2" });
		}

		[Test]
		public void DeserializeObject()
		{
			var obj = new ParamSerializer().Deserialize("a[b]=bVal&a[c]=cVal");

			Assert.AreEqual(obj["a"], new JObject { { "b", "bVal" }, { "c", "cVal" } });
		}

		[Test]
		public void DeserializeNestedObject()
		{
			var obj = new ParamSerializer().Deserialize("a[b][c]=cVal&a[b][d]=dVal&a[e]=eVal");

			Assert.AreEqual(obj["a"], new JObject { { "b", new JObject { { "c", "cVal" }, { "d", "dVal" } } }, { "e", "eVal" } });
		}

		[Test]
		public void SerializeSimpleObject()
		{
			var param = new ParamSerializer().Serialize(new { a = "1", b = "2" });

			Assert.AreEqual(param, "a=1&b=2");
		}

		[Test]
		public void SerializeObjectWithArray()
		{
			var param = new ParamSerializer().Serialize(new { a = new[] { "1", "2", "3" }, b = "2" });

			Assert.AreEqual(param, "a[]=1&a[]=2&a[]=3&b=2");
		}

		[Test]
		public void SerializeObjectWithArrayAndObject()
		{
			var param = new ParamSerializer().Serialize(new { a = new object[] { "1", "2", new { d = "8", e = "9" } }, b = "2" });

			Assert.AreEqual(param, "a[]=1&a[]=2&a[2][d]=8&a[2][e]=9&b=2");
		}

		[Test]
		public void SerializeDeserializeSimple()
		{
			var serializer = new ParamSerializer();
			var startObj = new { a = "1", b = "2" };
			var endObj = serializer.Deserialize(serializer.Serialize(startObj));
			var startObjStr = JsonConvert.SerializeObject(startObj);
			var endObjStr = JsonConvert.SerializeObject(endObj);

			Assert.AreEqual(startObjStr, endObjStr);
		}

		[Test]
		public void SerializeDeserializeComplex()
		{
			var serializer = new ParamSerializer();
			var startObj = new { a = new object[] { "1", "2", new { d = "8", e = "9" } }, b = "2", c = new { f = "10", g = "11" } };
			var endObj = serializer.Deserialize(serializer.Serialize(startObj));
			var startObjStr = JsonConvert.SerializeObject(startObj);
			var endObjStr = JsonConvert.SerializeObject(endObj);

			Assert.AreEqual(startObjStr, endObjStr);
		}
	}
}
