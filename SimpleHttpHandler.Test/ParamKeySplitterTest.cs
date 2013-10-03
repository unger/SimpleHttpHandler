using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler.Test
{
	using NUnit.Framework;

	using SimpleHttpHandler.RequestHelpers;

	[TestFixture]
	public class ParamKeySplitterTest
	{
		[Test]
		public void SingleParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("test");

			Assert.AreEqual(new [] { "test" }, result);
		}

		[Test]
		public void ArrayParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("test[0]");

			Assert.AreEqual(new[] { "test", "0" }, result);
		}

		[Test]
		public void ObjectArrayParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("test.array[0]");

			Assert.AreEqual(new[] { "test", "array", "0" }, result);
		}

		[Test]
		public void ObjectArrayObjectParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("test.array[0][subkey]");

			Assert.AreEqual(new[] { "test", "array", "0", "subkey" }, result);
		}
		
		[Test]
		public void ArrayWithObjectParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("[0].FirstName");

			Assert.AreEqual(new[] { "0", "FirstName" }, result);
		}

		[Test]
		public void NamedArrayWithObjectParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("employees[100].FirstName");

			Assert.AreEqual(new[] { "employees", "100", "FirstName" }, result);
		}

		[Test]
		public void UnindexedArrayWithObjectParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("employees[].FirstName");

			Assert.AreEqual(new[] { "employees", "", "FirstName" }, result);
		}

		[Test]
		public void MultiArrayParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("test[0][1][0]");

			Assert.AreEqual(new[] { "test", "0", "1", "0" }, result);
		}

		[Test]
		public void MultiObjectParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("test[a][b][c]");

			Assert.AreEqual(new[] { "test", "a", "b", "c" }, result);
		}

		[Test]
		public void MultiObjectWithDotNotationParamTest()
		{
			var splitter = new ParamKeySplitter();

			var result = splitter.SplitKey("test.a.b.c");

			Assert.AreEqual(new[] { "test", "a", "b", "c" }, result);
		}
	}
}
