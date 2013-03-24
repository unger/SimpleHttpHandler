namespace SimpleHttpHandler.RequestHelpers
{
	using System;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class ParamSerializer : IParamSerializer
	{
		/// <summary>
		/// Serialize an array of form elements or a set of key/values into a query string
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public string Serialize(object obj)
		{
			var paramString = this.Parametrize(JObject.FromObject(obj));
			return paramString.TrimEnd(new[] { '&' });
		}

		public JObject Deserialize(NameValueCollection input)
		{
			var output = new StringBuilder();
			foreach (var key in input.AllKeys)
			{
				output.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(input[key]));
			}

			return this.Deserialize(output.ToString().TrimEnd(new[] { '&' }));
		}

		public JObject Deserialize(string input)
		{
			return this.Deparametrize(input);
		}

		/// <summary>
		/// Translation of jquery-deparam
		/// https://github.com/chrissrogers/jquery-deparam/blob/master/jquery-deparam.js
		/// </summary>
		private JObject Deparametrize(string input) 
		{
			var obj = new JObject();

			var items = input.Replace("+", " ").Split(new[] { '&' });

			// Iterate over all name=value pairs.
			foreach (string item in items)
			{
				var param = item.Split(new[] { '=' });
				var key = HttpUtility.UrlDecode(param[0]);
				if (!string.IsNullOrEmpty(key))
				{
					// If key is more complex than 'foo', like 'a[]' or 'a[b][c]', split it
					// into its component parts.
					var keys = key.Split(new[] { "][" }, StringSplitOptions.RemoveEmptyEntries);
					var keysLast = keys.Length - 1;

					// If the first keys part contains [ and the last ends with ], then []
					// are correctly balanced.
					if (Regex.IsMatch(keys[0], @"\[") && Regex.IsMatch(keys[keysLast], @"\]$"))
					{
						// Remove the trailing ] from the last keys part.
						keys[keysLast] = Regex.Replace(keys[keysLast], @"\]$", string.Empty);

						// Split first keys part into two parts on the [ and add them back onto
						// the beginning of the keys array.
						keys = keys[0].Split(new[] { '[' }).Concat(keys.Skip(1)).ToArray();
						keysLast = keys.Length - 1;
					}
					else
					{
						// Basic 'foo' style key.
						keysLast = 0;
					}

					// Are we dealing with a name=value pair, or just a name?
					if (param.Length == 2)
					{
						var val = HttpUtility.UrlDecode(param[1]);

						// Coerce values.
						// Convert val to int, double, bool, string
						if (keysLast != 0)
						{
							// Complex key, build deep object structure based on a few rules:
							// * The 'cur' pointer starts at the object top-level.
							// * [] = array push (n is set to array length), [n] = array if n is 
							//   numeric, otherwise object.
							// * If at the last keys part, set the value.
							// * For each keys part, if the current level is undefined create an
							//   object or array based on the type of the next keys part.
							// * Move the 'cur' pointer to the next level.
							// * Rinse & repeat.
							object cur = obj;
							for (var i = 0; i <= keysLast; i++)
							{
								int index = -1, nextindex;

								// Array 'a[]' or 'a[1]', 'a[2]'
								key = keys[i];

								if (key == string.Empty || int.TryParse(key, out index))
								{
									key = index == -1 ? "0" : index.ToString(CultureInfo.InvariantCulture);
								}

								if (cur is JArray)
								{
									var jarr = cur as JArray;
									if (i == keysLast)
									{
										if (index >= 0 && index < jarr.Count)
										{
											jarr[index] = val;
										}
										else
										{
											jarr.Add(val);
										}
									}
									else
									{
										if (index < 0 || index >= jarr.Count)
										{
											if (keys[i + 1] == string.Empty || int.TryParse(keys[i + 1], out nextindex))
											{
												jarr.Add(new JArray());
											}
											else
											{
												jarr.Add(new JObject());
											}

											index = jarr.Count - 1;
										}

										cur = jarr.ElementAt(index);
									}
								}
								else if (cur is JObject)
								{
									var jobj = cur as JObject;
									if (i == keysLast)
									{
										jobj[key] = val;
									}
									else
									{
										if (jobj[key] == null)
										{
											if (keys[i + 1] == string.Empty || int.TryParse(keys[i + 1], out nextindex))
											{
												jobj.Add(key, new JArray());
											}
											else
											{
												jobj.Add(key, new JObject());
											}
										}

										cur = jobj[key];
									}
								}
							}
						}
						else
						{
							// Simple key, even simpler rules, since only scalars and shallow
							// arrays are allowed.
							if (obj[key] is JArray)
							{
								// val is already an array, so push on the next value.
								(obj[key] as JArray).Add(val);
							}
							else if (obj[key] != null && val != null)
							{
								// val isn't an array, but since a second value has been specified,
								// convert val into an array.
								obj[key] = new JArray { obj[key], val };
							}
							else
							{
								// val is a scalar.
								obj[key] = val;
							}
						}
					}
					else if (!string.IsNullOrEmpty(key))
					{
						// No value was defined, so set something meaningful.
						obj[key] = null;
					}
				}
			}

			return obj;
		}

		private string Parametrize(object obj, string value = "")
		{
			var returnVal = string.Empty;

			if (obj is JObject)
			{
				var jobj = obj as JObject;
				foreach (var key in jobj.Properties())
				{
					returnVal += this.Parametrize(jobj[key.Name], value == string.Empty ? key.Name : string.Format("{0}[{1}]", value, key.Name));
				}
			}
			else if (obj is JArray)
			{
				var arr = obj as JArray;
				for (int i = 0; i < arr.Count; i++)
				{
					var item = arr[i];
					if (item is JArray || item is JObject)
					{
						returnVal += this.Parametrize(item, string.Format("{0}[{1}]", value, i));
					}
					else
					{
						returnVal += this.Parametrize(item, string.Format("{0}[]", value));
					}
				}
			}
			else
			{
				return string.Format("{0}={1}&", value, obj);
			}

			return returnVal;
		}
	}
}
