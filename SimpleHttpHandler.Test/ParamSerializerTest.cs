namespace SimpleHttpHandler.Test
{
	using System.Collections.Generic;
	using System.Linq;

	using NUnit.Framework;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using SimpleHttpHandler.RequestHelpers;

	[TestFixture]
	public class ParamSerializerTest
	{
		[Test]
		public void Deserialize_SingleDimension()
		{
			var obj = new ParamSerializer().Deserialize("a=1&b=2");

			Assert.AreEqual(obj["a"], new JValue("1"));
			Assert.AreEqual(obj["b"], new JValue("2"));
		}

		[Test]
		public void Deserialize_Array()
		{
			var obj = new ParamSerializer().Deserialize("a[]=1&a[]=2");

			Assert.AreEqual(obj["a"], new JArray { "1", "2" });
		}

		[Test]
		public void Deserialize_ArrayWithObject()
		{
			var obj = new ParamSerializer().Deserialize("a[0][b]=1&a[1][b]=2");

			Assert.AreEqual(obj["a"], new JArray { new JObject { { "b", "1" } }, new JObject { { "b", "2" } } });
		}

		[Test]
		public void Deserialize_ArrayWithIndexes()
		{
			var obj = new ParamSerializer().Deserialize("a[0]=1&a[1]=2");

			Assert.AreEqual(obj["a"], new JArray { "1", "2" });
		}

		[Test]
		public void Deserialize_ArrayWithBadIndexes()
		{
			var obj = new ParamSerializer().Deserialize("a[5]=1&a[4]=2");

			Assert.AreEqual(obj["a"], new JArray { "1", "2" });
		}

		[Test]
		public void Deserialize_Object()
		{
			var obj = new ParamSerializer().Deserialize("a[b]=bVal&a[c]=cVal");

			Assert.AreEqual(obj["a"], new JObject { { "b", "bVal" }, { "c", "cVal" } });
		}

		[Test]
		public void Deserialize_NestedObject()
		{
			var obj = new ParamSerializer().Deserialize("a[b][c]=cVal&a[b][d]=dVal&a[e]=eVal");

			Assert.AreEqual(obj["a"], new JObject { { "b", new JObject { { "c", "cVal" }, { "d", "dVal" } } }, { "e", "eVal" } });
		}

		[Test]
		public void Deserialize_EmptyKey()
		{
			var obj = new ParamSerializer().Deserialize("a");
			var proplist = obj.Properties().ToList();

			Assert.AreEqual(1, proplist.Count);
			Assert.AreEqual("a", proplist.First().Name);
			Assert.AreEqual(new JValue((object)null), obj["a"]);
		}

		[Test]
		public void Deserialize_PararmsAndEmptyKey()
		{
			var obj = new ParamSerializer().Deserialize("a&b=1");

			Assert.AreEqual(new JObject { { "a", null }, { "b", "1" } }, obj);
		}

		[Test]
		public void Serialize_SimpleObject()
		{
			var param = new ParamSerializer().Serialize(new { a = "1", b = "2" });

			Assert.AreEqual(param, "a=1&b=2");
		}

		[Test]
		public void Serialize_SimpleJObject()
		{
			var param = new ParamSerializer().Serialize(new JObject { { "a", "1" }, { "b", "2" } });

			Assert.AreEqual(param, "a=1&b=2");
		}

		[Test]
		public void Serialize_ObjectWithArray()
		{
			var param = new ParamSerializer().Serialize(new { a = new[] { "1", "2", "3" }, b = "2" });

			Assert.AreEqual(param, "a[]=1&a[]=2&a[]=3&b=2");
		}

		[Test]
		public void Serialize_ObjectWithArrayAndObject()
		{
			var param = new ParamSerializer().Serialize(new { a = new object[] { "1", "2", new { d = "8", e = "9" } }, b = "2" });

			Assert.AreEqual(param, "a[]=1&a[]=2&a[2][d]=8&a[2][e]=9&b=2");
		}

		[Test]
		public void SerializeDeserialize_Simple()
		{
			var serializer = new ParamSerializer();
			var startObj = new { a = "1", b = "2" };
			var endObj = serializer.Deserialize(serializer.Serialize(startObj));
			var startObjStr = JsonConvert.SerializeObject(startObj);
			var endObjStr = JsonConvert.SerializeObject(endObj);

			Assert.AreEqual(startObjStr, endObjStr);
		}

		[Test]
		public void SerializeDeserialize_Complex()
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
