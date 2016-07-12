using System;
using System.Linq;

namespace Search
{
	public static class EditDistance
	{
		private struct JaroMetrics
		{
			public int Matches;

			public int Transpositions;
		}

		private static EditDistance.JaroMetrics Matches(string s1, string s2)
		{
			string text;
			string text2;
			if (s1.Length > s2.Length)
			{
				text = s1;
				text2 = s2;
			}
			else
			{
				text = s2;
				text2 = s1;
			}
			int num = Math.Max(text.Length / 2 - 1, 0);
			int[] array = new int[text2.Length];
			int i;
			for (i = 0; i < array.Length; i++)
			{
				array[i] = -1;
			}
			bool[] array2 = new bool[text.Length];
			int num2 = 0;
			for (int j = 0; j < text2.Length; j++)
			{
				char c = text2[j];
				int k = Math.Max(j - num, 0);
				int num3 = Math.Min(j + num + 1, text.Length);
				while (k < num3)
				{
					if (!array2[k] && c == text[k])
					{
						array[j] = k;
						array2[k] = true;
						num2++;
						break;
					}
					k++;
				}
			}
			char[] array3 = new char[num2];
			char[] ms2 = new char[num2];
			i = 0;
			int num4 = 0;
			while (i < text2.Length)
			{
				if (array[i] != -1)
				{
					array3[num4] = text2[i];
					num4++;
				}
				i++;
			}
			i = 0;
			num4 = 0;
			while (i < text.Length)
			{
				if (array2[i])
				{
					ms2[num4] = text[i];
					num4++;
				}
				i++;
			}
			int num5 = array3.Where((char t, int mi) => t != ms2[mi]).Count<char>();
			EditDistance.JaroMetrics result;
			result.Matches = num2;
			result.Transpositions = num5 / 2;
			return result;
		}

		public static int DameruLevenshteinDistance(this string s, string t)
		{
			int result;
			if (string.IsNullOrEmpty(s))
			{
				result = (t ?? "").Length;
			}
			else if (string.IsNullOrEmpty(t))
			{
				result = s.Length;
			}
			else
			{
				if (s.Length > t.Length)
				{
					string text = s;
					s = t;
					t = text;
				}
				int num = s.Length;
				int num2 = t.Length;
				while (num > 0 && s[num - 1] == t[num2 - 1])
				{
					num--;
					num2--;
				}
				int num3 = 0;
				if (s[0] == t[0] || num == 0)
				{
					while (num3 < num && s[num3] == t[num3])
					{
						num3++;
					}
					num -= num3;
					num2 -= num3;
					if (num == 0)
					{
						result = num2;
						return result;
					}
					t = t.Substring(num3, num2);
				}
				int[] array = new int[num2];
				int[] array2 = new int[num2];
				for (int i = 0; i < num2; i++)
				{
					array[i] = i + 1;
				}
				char c = s[0];
				int num4 = 0;
				for (int j = 0; j < num; j++)
				{
					char c2 = c;
					c = s[num3 + j];
					char c3 = t[0];
					int num5 = j;
					num4 = j + 1;
					int num6 = 0;
					for (int i = 0; i < num2; i++)
					{
						int num7 = num4;
						int num8 = num6;
						num6 = array2[i];
						num4 = (array2[i] = num5);
						num5 = array[i];
						char c4 = c3;
						c3 = t[i];
						if (c != c3)
						{
							if (num5 < num4)
							{
								num4 = num5;
							}
							if (num7 < num4)
							{
								num4 = num7;
							}
							num4++;
							if (j != 0 && i != 0 && c == c4 && c2 == c3)
							{
								num8++;
								if (num8 < num4)
								{
									num4 = num8;
								}
							}
						}
						array[i] = num4;
					}
				}
				result = num4;
			}
			return result;
		}

		public static float JaroWinkler(this string s1, string s2, float prefixScale, float boostThreshold)
		{
			prefixScale = ((prefixScale > 0.25f) ? 0.25f : prefixScale);
			prefixScale = ((prefixScale < 0f) ? 0f : prefixScale);
			float num = s1.Jaro(s2);
			int num2 = 0;
			for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
			{
				if (s1[i] != s2[i])
				{
					break;
				}
				num2++;
			}
			return (num < boostThreshold) ? num : (num + prefixScale * (float)num2 * (1f - num));
		}

		public static float JaroWinkler(this string s1, string s2, float prefixScale)
		{
			return s1.JaroWinkler(s2, prefixScale, 0.7f);
		}

		public static float JaroWinkler(this string s1, string s2)
		{
			return s1.JaroWinkler(s2, 0.1f, 0.7f);
		}

		public static float Jaro(this string s1, string s2)
		{
			EditDistance.JaroMetrics jaroMetrics = EditDistance.Matches(s1, s2);
			float num = (float)jaroMetrics.Matches;
			int transpositions = jaroMetrics.Transpositions;
			float result;
			if (num == 0f)
			{
				result = 0f;
			}
			else
			{
				float num2 = (num / (float)s1.Length + num / (float)s2.Length + (num - (float)transpositions) / num) / 3f;
				result = num2;
			}
			return result;
		}

		public static int LevenshteinDistance(this string source, string target)
		{
			int result;
			if (string.IsNullOrEmpty(source))
			{
				if (string.IsNullOrEmpty(target))
				{
					result = 0;
				}
				else
				{
					result = target.Length;
				}
			}
			else if (string.IsNullOrEmpty(target))
			{
				result = source.Length;
			}
			else
			{
				if (source.Length > target.Length)
				{
					string text = target;
					target = source;
					source = text;
				}
				int length = target.Length;
				int length2 = source.Length;
				int[,] array = new int[2, length + 1];
				for (int i = 1; i <= length; i++)
				{
					array[0, i] = i;
				}
				int num = 0;
				for (int j = 1; j <= length2; j++)
				{
					num = (j & 1);
					array[num, 0] = j;
					int num2 = num ^ 1;
					for (int i = 1; i <= length; i++)
					{
						int num3 = (target[i - 1] == source[j - 1]) ? 0 : 1;
						array[num, i] = Math.Min(Math.Min(array[num2, i] + 1, array[num, i - 1] + 1), array[num2, i - 1] + num3);
					}
				}
				result = array[num, length];
			}
			return result;
		}
	}
}
