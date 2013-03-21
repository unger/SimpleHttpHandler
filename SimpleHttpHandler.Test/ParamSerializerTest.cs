namespace SimpleHttpHandler.Test
{
	using System.Collections.Generic;

	using NUnit.Framework;

	[TestFixture]
	public class ParamSerializerTest
	{
		[Test]
		public void DeserializeSingleDimension()
		{
			var obj = new ParamSerializer().Deserialize("a=1&b=2");

			Assert.True(obj.ContainsKey("a"));
			Assert.True(obj.ContainsKey("b"));
			Assert.AreEqual(obj["a"], "1");
			Assert.AreEqual(obj["b"], "2");
		}

		[Test]
		public void DeserializeArray()
		{
			var obj = new ParamSerializer().Deserialize("a[]=1&a[]=2");

			Assert.True(obj.ContainsKey("a"));
			Assert.AreEqual(obj["a"], new[] { "1", "2" });
		}

		[Test]
		public void DeserializeArrayWithIndexes()
		{
			var obj = new ParamSerializer().Deserialize("a[0]=1&a[1]=2");

			Assert.True(obj.ContainsKey("a"));
			Assert.AreEqual(obj["a"], new[] { "1", "2" });
		}

		[Test]
		public void DeserializeArrayWithBadIndexes()
		{
			var obj = new ParamSerializer().Deserialize("a[5]=1&a[4]=2");

			Assert.True(obj.ContainsKey("a"));
			Assert.AreEqual(obj["a"], new[] { "1", "2" });
		}

		[Test]
		public void DeserializeObject()
		{
			var obj = new ParamSerializer().Deserialize("a[b]=bVal&a[c]=cVal");

			Assert.True(obj.ContainsKey("a"));
			Assert.AreEqual(obj["a"], new Dictionary<string, object> { { "b", "bVal" }, { "c", "cVal" } });
		}

		[Test]
		public void DeserializeNestedObject()
		{
			var obj = new ParamSerializer().Deserialize("a[b][c]=cVal&a[b][d]=dVal&a[e]=eVal");

			Assert.True(obj.ContainsKey("a"));
			Assert.AreEqual(obj["a"], new Dictionary<string, object> { { "b", new Dictionary<string, object> { { "c", "cVal" }, { "d", "dVal" } } }, { "e", "eVal" } });
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
	}
}
