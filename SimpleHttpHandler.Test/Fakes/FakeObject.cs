namespace SimpleHttpHandler.Test.Fakes
{
	using System;

	public class FakeObject
	{
		public string Prop1 { get; set; }
	
		public int Prop2 { get; set; }

		public double Prop3 { get; set; }

		public bool Prop4 { get; set; }

		public DateTime Prop5 { get; set; }

		public string[] Prop6 { get; set; }

		public override bool Equals(object obj)
		{
			var fake = obj as FakeObject;
			if (fake == null)
			{
				return false;
			}

			return object.Equals(this.Prop1, fake.Prop1) &&
			       object.Equals(this.Prop2, fake.Prop2) &&
			       object.Equals(this.Prop3, fake.Prop3) &&
			       object.Equals(this.Prop4, fake.Prop4) &&
				   object.Equals(this.Prop5, fake.Prop5) &&
			       this.ArrayEquals(this.Prop6, fake.Prop6);
		}

		private bool ArrayEquals(object[] list1, object[] list2)
		{
			var equals = object.Equals(list1.Length, list2.Length);
			for(int i = 0; i < list1.Length; i++)
			{
				equals = equals && object.Equals(list1[i], list2[i]);
			}
			return equals;
		}
	}
}