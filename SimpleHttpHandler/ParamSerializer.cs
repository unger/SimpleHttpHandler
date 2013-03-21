namespace SimpleHttpHandler
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	/// <summary>
	/// Port of jquery-deparam
	/// https://github.com/chrissrogers/jquery-deparam/blob/master/jquery-deparam.js
	/// </summary>
	public class ParamSerializer
	{

		/// <summary>
		/// Serialize an array of form elements or a set of key/values into a query string
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public string Serialize(object obj)
		{
			var jsonstring = JsonConvert.SerializeObject(obj);
			var dict = this.DeserializeToDictionary(jsonstring);

			var paramString = this.Parametrize(dict, string.Empty);

			return paramString.TrimEnd(new[] { '&' });
		}

		private string Parametrize(object obj, string value)
		{
			var returnVal = string.Empty;

			if (obj is Dictionary<string, object>)
			{
				var dict = obj as Dictionary<string, object>;
				foreach (var key in dict.Keys)
				{
					returnVal += this.Parametrize(dict[key], value == string.Empty ? key : string.Format("{0}[{1}]", value, key));
				}
			}
			else if (obj is JArray)
			{
				foreach (var item in obj as JArray)
				{
					returnVal += this.Parametrize(item, string.Format("{0}[]", value));
				}
			}
			else
			{
				return string.Format("{0}={1}&", value, obj);
			}

			return returnVal;
		}

		public Dictionary<string, object> Deserialize(NameValueCollection input)
		{
			var output = new StringBuilder();
			foreach (var key in input.AllKeys)
			{
				output.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(input[key]));
			}

			return this.Deserialize(output.ToString().TrimEnd(new[] { '&' }));
		}

		public Dictionary<string, object> Deserialize(string input)
		{
			var obj = new Dictionary<string, object>();

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

								if (cur is IList<object>)
								{
									if (i == keysLast)
									{
										if (index >= 0 && index < (cur as IList<object>).Count)
										{
											(cur as IList<object>)[index] = val;
										}
										else
										{
											(cur as IList<object>).Add(val);
										}
									}
									else
									{
										if (index < 0 || index >= (cur as IList<object>).Count)
										{
											if (keys[i + 1] == string.Empty || int.TryParse(keys[i + 1], out nextindex))
											{
												(cur as IList<object>).Add(new List<object>());
											}
											else
											{
												(cur as IList<object>).Add(new Dictionary<string, object>());
											}

											index = 0;
										}

										cur = (cur as IList<object>).ElementAt(index);
									}
								}
								else if (cur is Dictionary<string, object>)
								{
									if (i == keysLast)
									{
										(cur as Dictionary<string, object>)[key] = val;
									}
									else
									{
										if (!(cur as Dictionary<string, object>).ContainsKey(key))
										{
											if (keys[i + 1] == string.Empty || int.TryParse(keys[i + 1], out nextindex))
											{
												(cur as Dictionary<string, object>).Add(key, new List<object>());
											}
											else
											{
												(cur as Dictionary<string, object>).Add(key, new Dictionary<string, object>());
											}
										}

										cur = (cur as Dictionary<string, object>)[key];
									}
								}
							}
						}
						else
						{
							// Simple key, even simpler rules, since only scalars and shallow
							// arrays are allowed.
							if (obj.ContainsKey(key) && obj[key] is IList<object>)
							{
								// val is already an array, so push on the next value.
								(obj[key] as IList<object>).Add(val);
							}
							else if (obj.ContainsKey(key) && val != null)
							{
								// val isn't an array, but since a second value has been specified,
								// convert val into an array.
								obj[key] = new List<object> { obj[key], val };
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

		private Dictionary<string, object> DeserializeToDictionary(string jsonstring)
		{
			var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonstring);
			var values2 = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> d in values)
			{
				if (d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject"))
				{
					values2.Add(d.Key, this.DeserializeToDictionary(d.Value.ToString()));
				}
				else
				{
					values2.Add(d.Key, d.Value);
				}
			}

			return values2;
		}
	}
}
