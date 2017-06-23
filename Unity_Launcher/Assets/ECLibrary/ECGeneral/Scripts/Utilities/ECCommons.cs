using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECCommons : MonoBehaviour
{    
    /* ------------------------------ FixLength ------------------------------ */
    /// <summary>
    /// Output a string value of a number with floating points.
    /// <para> Example: FixLength(3.1415926f, 2) => 3.14 </para>
    /// </summary>
    /// <param name="value"> The number </param>
    /// <param name="floating"> Length of Decimal Digits </param>    
    public static string FixLength(double value, int floating)
    {
        if (floating < 0 || floating > 99) return value.ToString();
        return value.ToString("F" + floating);
    }
    /// <summary>
    /// Output a string value of a number with floating points and specific digit.
    /// <para> Example: FixLength(3.1415926, 2, 3) => 003.14 </para>
    /// </summary>
    /// <param name="value"> The number </param>
    /// <param name="floating"> Length of Decimal Digits </param>
    /// <param name="digit"> Length of Integer Digits </param>    
    public static string FixLength(double value, int floating, int digit)
    {
        string result = "";
        if (digit < 1) digit = 1;
        double tmp = System.Math.Pow(10, digit-1);
        for (int i = 1; i < digit && value < tmp; i++)
        {
            result += "0";
            tmp /= 10;
        }
        result += FixLength(value, floating);
        return result;
    }
    /// <summary>
    /// Output a string value of a number with specific digit. It supports non-decimal numbers.
    /// <para> Exaple: FixLength("10011", 8) => 00010011 </para>
    /// </summary>
    /// <param name="value"> The string value of the number </param>
    /// <param name="digit"> Length of Digits </param>    
    public static string FixLength(string value, int digit)
    {
        while (value.Length < digit) value = "0" + value;
        return value;
    }

    /* ------------------------------ BaseConvert ------------------------------ */
    /// <summary>
    /// Convert number from any integer base (Range: 2 ~ Code Base Length) to Decimal Number.
    /// 0 will be returned if there is any wrong value.
    /// <para> Example: Base10("FF", 16, ECConstant.DIGIT)      => 255 </para>
    /// <para> Example: Base10("11111111", 2, ECConstant.DIGIT) => 255 </para>
    /// </summary>
    /// <param name="value"> The string value of the number </param>
    /// <param name="origin"> The original base </param>
    /// <param name="digitBase"> The reference character array for converting 
    /// <para> Default: ECConstant.DIGIT </para>
    /// </param>    
    public static long Base10(string value, int origin, string digitBase)
    {
        if (origin < 2 || origin > digitBase.Length) return -1;
        int vLength = value.Length;
        int[] number = new int[vLength];
        long decValue = 0;
        //value = value.ToUpper();
        for (int i = 0; i < vLength; i++)
        {
            int index = digitBase.IndexOf(value[i]);
            if (index != -1)
            {
                decValue += index * (long)(System.Math.Pow(origin, vLength - 1 - i));
            }
            else return -1;
        }
        //Debug.Log(value + " String, Value " + decValue);
        return decValue;
    }
    /// <summary>
    /// Convert number from any integer base (Range: 2 ~ Code Base Length) to Decimal Number with the
    /// default code base. 0 will be returned if there is any wrong value.
    /// <para> Example: BaseConvert("FF", 16)      => 255 </para>
    /// <para> Example: BaseConvert("11111111", 2) => 255 </para>
    /// </summary>
    /// <param name="value"> The string value of the number </param>
    /// <param name="origin"> The original base </param>    
    public static long Base10(string value, int origin)
    {
        return Base10(value, origin, ECConstant.DIGIT);
    }
    /// <summary>
    /// Convert number from any integer base to other integer base (Range: 2 ~ Code Base Length).
    /// 0 will be returned if there is any wrong value.
    /// <para> Example: BaseConvert("FF", 16, 10, ECConstant.DIGIT)      => 255 </para>
    /// <para> Example: BaseConvert("11111111", 2, 16, ECConstant.DIGIT) => FF </para>
    /// </summary>
    /// <param name="value"> The string value of the number </param>
    /// <param name="origin"> The original base </param>
    /// <param name="target"> The target base </param>
    /// <param name="digitBase"> The reference character array for converting 
    /// <para> Default: ECConstant.DIGIT </para></param>    
    public static string BaseConvert(string value, int origin, int target, string digitBase)
    {
        long decValue = Base10(value, origin, digitBase);
        if (target < 2 || target > digitBase.Length || decValue < 0) return "-1";
        else if (decValue == 0) return "0";
        string result = "";
        if (target == 10) return decValue.ToString();
        while (decValue > 0)
        {
            result = digitBase[(int)(decValue % target)] + result;
            //Debug.Log(decValue + "..." + (decValue % target));
            decValue /= target;
        }
        return result;
    }
    /// <summary>
    /// Convert number from any integer base to other integer base (Range: 2 ~ Code Base Length)
    /// with the default code base. 0 will be returned if there is any wrong value.
    /// <para> Example: BaseConvert("FF", 16, 10)      => 255 </para>
    /// <para> Example: BaseConvert("11111111", 2, 16) => FF </para>
    /// </summary>
    /// <param name="value"> The string value of the number </param>
    /// <param name="origin"> The original base </param>
    /// <param name="target"> The target base </param>    
    public static string BaseConvert(string value, int origin, int target)
    {
        return BaseConvert(value, origin, target, ECConstant.DIGIT);
    }
    /// <summary>
    /// Convert number from Decimal Number to other integer base (Range: 2 ~ Code Base Length).
    /// 0 will be returned if there is any wrong value.
    /// <para> Example: BaseConvert(255, 16, 10, ECConstant.DIGIT) => FF </para>
    /// <para> Example: BaseConvert(255, 2, 16, ECConstant.DIGIT) => 11111111 </para>
    /// </summary>
    /// <param name="value"> The string value of the number </param>
    /// <param name="target"> The target base </param>
    /// <param name="digitBase"> The reference character array for converting 
    /// <para> Default: ECConstant.DIGIT </para></param>    
    public static string BaseConvert(long value, int target, string digitBase)
    {
        return BaseConvert(value.ToString(), 10, target, digitBase);
    }
    /// <summary>
    /// Convert number from Decimal Number to other integer base (Range: 2 ~ Code Base Length)
    /// with the default code base. 0 will be returned if there is any wrong value.
    /// <para> Example: BaseConvert(255, 16, 10) => FF </para>
    /// <para> Example: BaseConvert(255, 2, 16) => 11111111 </para>
    /// </summary>
    /// <param name="value"> The string value of the number </param>
    /// <param name="target"> The target base </param>    
    public static string BaseConvert(long value, int target)
    {
        return BaseConvert(value.ToString(), 10, target, ECConstant.DIGIT);
    }

    /* ------------------------------ Trans Code ------------------------------ */
    /// <summary>
    /// Get the string value from stored data.
    /// <para> Example: Encode("YQA=") => "a" </para>
    /// </summary>
    /// <param name="data"> The stored data </param>    
    public static string Encode(string data)
    {
        string result = "";
        result = System.Text.Encoding.Unicode.GetString(System.Convert.FromBase64String(data));
        return result;
    }
    /// <summary>
    /// Get the stored data from the string.
    /// <para> Example: Decode("a") => "YQA=" </para>
    /// </summary>
    /// <param name="data"> The string value </param>    
    public static string Decode(string value)
    {
        string result = "";
        result = System.Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(value));
        return result;
    }
    /// <summary>
    /// Return an unreadable string value to protect data.
    /// <para> Encrypt("123", ECConstant.PASSWORD) => "??????" </para>
    /// </summary>
    /// <param name="data"> The data needed to protect </param>
    /// <param name="digitBase"> The reference character array for converting 
    /// <para> Default: ECConstant.DIGIT </para></param>
    
    public static string Encrypt(string data, string digitBase)
    {
        string result = "";
        for (int i = 0; i < data.Length; i++) result += BaseConvert(data[i] * (i + 1) * (data.Length - i) % int.MaxValue, digitBase.Length, digitBase);
        return result;
    }
    /// <summary>
    /// Return an unreadable string value to protect data.
    /// <para> Encrypt("123") => "??????" </para>
    /// </summary>
    /// <param name="data"> The data needed to protect </param>    
    public static string Encrypt(string data)
    {
        return Encrypt(ECConstant.PASSWORD);
    }
    /// <summary>
    /// Return a number liked string to protect data.
    /// <para> Digital("123", 2, ECConstant.DIGIT) => "??????" </para>
    /// </summary>
    /// <param name="data"> The data needed to protect </param>
    /// <param name="digit"> The target base </param>
    /// <param name="digitBase"> The reference character array for converting 
    /// <para> Default: ECConstant.DIGIT </para>
    /// </param>    
    public static string Digital(string data, int digit, string digitBase)
    {
        string result = "";
        for (int i = 0; i < data.Length; i++) result += BaseConvert(data[i], digit, digitBase);
        return result;
    }
    /// <summary>
    /// Return a number liked string to protect data to Binary Number.
    /// <para> Digital("123") => "??????" </para>
    /// </summary>
    /// <param name="data"> The data needed to protect </param>    
    public static string Digital(string data)
    {
        return Digital(data, 2, ECConstant.DIGIT);
    }

    /* ------------------------------ Rounding ------------------------------ */
    /// <summary>
    /// Round up the number with specific digit.
    /// <para> Example: Ceil(1234.5678, -2) => 1234.57 </para>
    /// <para> Example: Ceil(1234.5678, 0) => 1235 </para>
    /// <para> Example: Ceil(1234.5678, 2) => 1300 </para>
    /// </summary>
    /// <param name="value"> The floating number </param>
    /// <param name="digit"> The digit needed to round </param>    
    public static double Ceil(double value, int digit)
    {
        double tmp = System.Math.Pow(10, digit);
        return System.Math.Ceiling(value / tmp) * tmp;
    }
    /// <summary>
    /// Round down the number with specific digit.
    /// <para> Example: Ceil(1234.5678, -2) => 1234.56 </para>
    /// <para> Example: Ceil(1234.5678, 0) => 1234 </para>
    /// <para> Example: Ceil(1234.5678, 2) => 1200 </para>
    /// </summary>
    /// <param name="value"> The floating number </param>
    /// <param name="digit"> The digit needed to round </param>    
    public static double Floor(double value, int digit)
    {
        double tmp = System.Math.Pow(10, digit);
        return System.Math.Floor(value / tmp) * tmp;
    }
    /// <summary>
    /// Round the number with specific digit.
    /// <para> Example: Ceil(1234.5678, -2) => 1234.57 </para>
    /// <para> Example: Ceil(1234.5678, 0) => 1235 </para>
    /// <para> Example: Ceil(1234.5678, 2) => 1200 </para>
    /// </summary>
    /// <param name="value"> The floating number </param>
    /// <param name="digit"> The digit needed to round </param>    
    public static double Round(double value, int digit)
    {
        double tmp = System.Math.Pow(10, digit);
        return System.Math.Round(value / tmp) * tmp;
    }

    /* ------------------------------ Approach ------------------------------ */
    /// <summary>
    /// To approach the original number to target number by division.
    /// <para> Example: Approach(0, 10, 4, 0.01f) => 2.5f </para>
    /// <para> Example: Approach(2.5f, 10, 4, 0.01f) => 4.375f </para>
    /// <para> Example: Approach(4.375f, 10, 4, 0.01f) => 5.78125f </para>
    /// </summary>
    /// <param name="origin"> The origin number </param>
    /// <param name="target"> The target number </param>
    /// <param name="division"> The steps need to move from origin to target </param>
    /// <param name="error"> The minimum value of moving distance </param>    
    public static float Approach(float origin, float target, float division, float error)
    {
        float distance = target - origin;
        if (division <= 1 || error <= 0 || 
            Mathf.Abs(distance * Mathf.Pow(division, -1)) <= error) return target;
        return origin + distance * Mathf.Pow(division, -1);
    }
    /// <summary>
    /// To approach the original integer to target integer by division.
    /// <para> Example: Approach(10, 0, 4, 0.01f) => 7 </para>
    /// <para> Example: Approach(7, 0, 4, 0.01f) => 5 </para>
    /// <para> Example: Approach(5, 0, 4, 0.01f) => 3 </para>
    /// </summary>
    /// <param name="origin"> The origin integer </param>
    /// <param name="target"> The target integer </param>
    /// <param name="division"> The steps need to move from origin to target </param>
    /// <param name="error"> The minimum value of moving distance </param>    
    public static int Approach(int origin, int target, float division, float error)
    {
        int distance = target - origin;
        if (division <= 1 || error <= 0 ||
            Mathf.Abs(distance * Mathf.Pow(division, -1)) <= error) return target;
        if (origin < target) return origin + Mathf.CeilToInt(distance * Mathf.Pow(division, -1));
        return origin + Mathf.FloorToInt(distance * Mathf.Pow(division, -1));
    }

    /* ------------------------------ Sorting ------------------------------ */
    /// <summary>
    /// Swap the values of two indices of an array.
    /// <para> Example: Swap(naturalNum, 1, 3) => 0, 3, 2, 1, 4, 5... </para>
    /// </summary>
    /// <param name="array"> The related array </param>
    /// <param name="i"> The first index </param>
    /// <param name="j"> The second index </param>
    public static void Swap<T>(T[] array, int i, int j)
    {
        if (i < 0 || i >= array.Length || j < 0 || j >= array.Length) return;
        T tmp = array[i];
        array[i] = array[j];
        array[j] = tmp;
    }
    /// <summary>
    /// Randomly shuffle the array by N times.
    /// <para> Example: Shuffle(naturalNum, 100) => Shuffle 100 times </para>
    /// </summary>
    /// <param name="array"> The related array </param>
    /// <param name="times"> The times of shuffle action </param>
    public static void Shuffle<T>(T[] array, int times)
    {
        int a = 0, b = 0;
        for (int i = 0; i < times; i++)
        {
            a = Random.Range(0, array.Length);
            b = Random.Range(0, array.Length);
            Swap(array, a, b);
        }
    }
    /// <summary>
    /// Rearrange a range of index of an array by Quick Sort.
    /// <para> Example: Sort(naturalNum, 0, 9, true) => 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 </para>
    /// <para> Example: Sort(naturalNum, 0, 9, false) => 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 </para>
    /// </summary>
    /// <param name="array"> The related array</param>
    /// <param name="left"> The first index needed to sort </param>
    /// <param name="right"> The last index needed to sort </param>
    /// <param name="ascending"> Sorting Order
    /// <para> True: Ascending Order </para>
    /// <para> False: Decending Order </para>
    /// </param>
    public static void Sort<T>(T[] array, int left, int right, bool ascending)
    {
        bool isNum = true;
        if (array.GetType() != typeof(byte[]) && array.GetType() != typeof(short[]) &&
            array.GetType() != typeof(int[]) && array.GetType() != typeof(long[]) &&
            array.GetType() != typeof(float[]) && array.GetType() != typeof(double[]) &&
            array.GetType() != typeof(char[]) && array.GetType() != typeof(string[]) && array.GetType() != typeof(System.String[])) return;
        else if (array.GetType() == typeof(System.String[]) || array.GetType() == typeof(char[]) || array.GetType() == typeof(string[]))
        {
            isNum = false;
        }
        if (left < right)
        {
            int i = left, j = right + 1;
            while (true)
            {
                if (isNum)
                {
                    while (i + 1 < array.Length && (ascending && float.Parse(array[++i].ToString()) < float.Parse(array[left].ToString()) || !ascending && float.Parse(array[++i].ToString()) > float.Parse(array[left].ToString()))) ;
                    while (j - 1 > -1 && (ascending && float.Parse(array[--j].ToString()) > float.Parse(array[left].ToString()) || !ascending && float.Parse(array[--j].ToString()) < float.Parse(array[left].ToString()))) ;
                }
                else
                {
                    while (i + 1 < array.Length && (ascending && array[++i].ToString().CompareTo(array[left].ToString()) < 0 || !ascending && array[++i].ToString().CompareTo(array[left].ToString()) > 0)) ;
                    while (j - 1 > -1 && (ascending && array[--j].ToString().CompareTo(array[left].ToString()) > 0 || !ascending && array[--j].ToString().CompareTo(array[left].ToString()) < 0)) ;
                }
                if (i >= j) break;
                Swap(array, i, j);
            }
            Swap(array, left, j);
            Sort(array, left, j - 1, ascending);
            Sort(array, j + 1, right, ascending);
        }
    }
    /// <summary>
    /// Rearrange the array by Quick Sort.
    /// <para> Example: Sort(naturalNum, true) => 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 </para>
    /// <para> Example: Sort(naturalNum, false) => 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 </para> 
    /// </summary>
    /// <param name="array"> The related array </param>
    /// <param name="ascending"> Sorting Order
    /// <para> True: Ascending Order </para>
    /// <para> False: Decending Order </para>
    /// </param>
    public static void Sort<T>(T[] array, bool ascending)
    {
        Sort(array, 0, array.Length - 1, ascending);
    }
    /// <summary>
    /// Rearrange the array by Quick Sort with ascending order.
    /// <para> Example: Sort(naturalNum) => 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 </para>
    /// </summary>
    /// <param name="array"> The related array </param>
    public static void Sort<T>(T[] array)
    {
        Sort(array, 0, array.Length - 1, true);
    }


    /*
     * MousePos - Return the mouse position on the world point 
     * */
    public static Vector3 MousePos()
    {
        float mousex = (Input.mousePosition.x);
        float mousey = (Input.mousePosition.y);
        return Camera.main.ScreenToWorldPoint(new Vector3(mousex, mousey, 0));
    }

    /* ------------------------------ Initializing ------------------------------ */
    /// <summary>
    /// Smartly set a game component to be scriptable, you may not need to create a component at start if not necessary. 
    /// <para> Example: light = (Light)SetComponent(typeof(Light), gameObject) => Automatically map a component to "light" </para>
    /// </summary>
    /// <param name="component"> The component variable </param>
    /// <param name="gameObject"> The related game object </param>
    public static Component SetComponent(System.Type type, GameObject gameObject)
    {
        Component component;
        if (!gameObject.GetComponent(type))
        {
            component = gameObject.AddComponent(type);
        }
        else
        {
            component = gameObject.GetComponent(type);
        }
        return component;
    }

    /// <summary>
    /// Smartly set a game component to be scriptable, you may not need to create a component at start if not necessary. 
    /// <para> Example: light = SetComponent&lt;Light&gt;(gameObject) => Automatically map a component to "light" </para>
    /// </summary>
    /// <param name="gameObject"> The related game object </param>
    public static T SetComponent<T>(GameObject gameObject) where T : Component
    {
        T component;
        if (!gameObject.GetComponent(typeof(T))) component = gameObject.AddComponent(typeof(T)) as T;
        else component = gameObject.GetComponent(typeof(T)) as T;
        return component;
    }

    /* ------------------------------ String ------------------------------ */
    /// <summary>
    /// Combine number of data to a string.
    /// <para> Example: Combine("|", 123, "abc", 0.1f) => "123|abc|0.1" </para>
    /// </summary>
    /// <param name="separator"> The identifier to separate data </param>
    /// <param name="values"> All data </param>
    public static string Combine(string separator, params object[] values)
    {
        string result = "";
        for (int i = 0; i < values.Length; i++)
        {
            result += values[i];
            if (i < values.Length - 1) result += separator;
        }
        return result;
    }
    /// <summary>
    /// Combine an array to string.
    /// <para> Example: ArrayToString("|", an array) => "123|abc|0.1" </para>
    /// </summary>
    /// <param name="separator"> The identifier to combine data </param>
    /// <param name="value"> The target array </param>
    public static string ArrayToString<T>(string separator, T[] values)
    {
        string result = "";
        for (int i = 0; i < values.Length; i++)
        {
            result += values[i];
            if (i < values.Length - 1) result += separator;
        }
        return result;
    }
    /// <summary>
    /// Combine a list to string.
    /// <para> Example: ArrayToString("|", a list) => "123|abc"|0.1" </para>
    /// </summary>
    /// <param name="separator"> The identifier to combine data </param>
    /// <param name="value"> The target list </param>
    public static string ListToString<T>(string separator, List<T> values)
    {
        string result = "";
        for (int i = 0; i < values.Count; i++)
        {
            result += values[i];
            if (i < values.Count - 1) result += separator;
        }
        return result;
    }
    /// <summary>
    /// Separate number of data to strings.
    /// <para> Example: Separate("|", 123|abc|0.1) => "123", "abc", "0.1" </para>
    /// </summary>
    /// <param name="separator"> The identifier to separate data </param>
    /// <param name="value"> The target string </param>
    public static string[] Separate(string separator, string value)
    {
        List<string> result = new List<string>();
        int sIndex = 0; 
        if (!value.Contains(separator))
        {
            result.Add(value);
            return result.ToArray();
        }
        for (int i = 0; i < value.Length - separator.Length + 1; i++)
        {
            if(value.Substring(i, separator.Length).Contains(separator)){
                result.Add(value.Substring(sIndex, i - sIndex));
                i += separator.Length;
                sIndex = i;
                i--;
            }
        }
        if (sIndex < value.Length) result.Add(value.Substring(sIndex));
        return result.ToArray();
    }

    /* ------------------------------ Algebra ------------------------------ */
    /// <summary>
    /// Find the roots of a cubic equation.
    /// <para> Example: CubicRoots(1, 3, 3, 1) => x^3 + 3x^2 + 3x + 1 = 0, x = -1, -1, -1 </para>
    /// </summary>
    /// <param name="a"> Coefficient of x^3 </param>
    /// <param name="b"> Coefficient of x^2 </param>
    /// <param name="c"> Coefficient of x </param>
    /// <param name="d"> Constant </param>
    public static Complex[] CubicRoots(double a, double b, double c, double d)
    {
        Complex[] roots = new Complex[3];
        double b2 = b * b;
        double f = (c - b2 / (a * 3.0f)) / a;
        double g = (2 * b2 * b / (a * a) - 9 * b * c / a + 27 * d) / (27.0f * a);
        double h = g * g * 0.25f + f * f * f / 27.0f;
        if (h <= 0)
        {
            if (f == 0 && g == 0 && h == 0)
            {
                roots[0] = new Complex(-System.Math.Pow(d / a, 1 / 3.0f));
                roots[1] = roots[0];
                roots[2] = roots[0];
            }
            else
            {
                double i = System.Math.Sqrt(g * g * 0.25f - h);
                double j = System.Math.Pow(i, 1 / 3.0f);
                double k = System.Math.Acos(-g * 0.5f / i);
                double l = -j;
                double angle = k / 3.0f;
                double m = System.Math.Cos(angle);
                double n = System.Math.Sqrt(3) * System.Math.Sin(angle);
                double p = b / (3.0f * a);
                roots[0] = new Complex(2 * j * m - p);
                roots[1] = new Complex(l * (m + n) - p);
                roots[2] = new Complex(l * (m - n) - p);
            }
        }
        else
        {
            double r = -0.5f * g + System.Math.Sqrt(h);
            double s = System.Math.Pow(r, 1 / 3.0f);
            double t = -0.5f * g - System.Math.Sqrt(h);
            double u = System.Math.Pow(System.Math.Abs(t), 1 / 3.0f);
            if (t < 0) u = -u;
            double p = b / (3.0f * a);
            double q1 = s + u;
            double q2 = s - u;
            double vr = -0.5f * q1 - p;
            double vi = 0.5f * System.Math.Sqrt(3) * q2;
            roots[0] = new Complex(q1 - p);
            roots[1] = new Complex(vr, vi);
            roots[2] = new Complex(vr, -vi);
        }
        return roots;
    }
    /// <summary>
    /// Find the roots of a quartic equation.
    /// <para> Example: QuarticRoots(1, 4, 6, 4, 1) => x^4 + 4x^3 + 6x^2 + 4x + 1 = 0, x = -1, -1, -1, -1 </para>
    /// </summary>
    /// <param name="a"> Coefficient of x^4 </param>
    /// <param name="b"> Coefficient of x^3 </param>
    /// <param name="c"> Coefficient of x^2 </param>
    /// <param name="d"> Coefficient of x </param>
    /// <param name="e"> Constant </param>
    public static Complex[] QuarticRoots(double a, double b, double c, double d, double e)
    {
        Complex[] roots = new Complex[4];
        if (a != 1)
        {
            b /= a;
            c /= a;
            d /= a;
            e /= a;
            a = 1;
        }
        double b2 = b * b;
        double f = c - b2 * 0.375;
        double g = d + b2 * b * 0.125 - b * c * 0.5;
        double h = e - b2 * b2 * 0.09375 * 0.125 + b2 * c * 0.0625 - b * d * 0.25;
        double B = 0.5f * f;
        double C = f * f * 0.0625 - h * 0.25;
        double D = -g * g * 0.015625;
        Complex[] ys = CubicRoots(1, B, C, D);
        Complex[] y = new Complex[2];
        int i = 0;
        for (int j = ys.Length - 1; j >= 0; j--)
        {
            if (ys[j] != Complex.zero)
            {
                y[i] = ys[j];
                i++;
            }
            if (i >= 2) break;
        }
        Complex p = Complex.Sqrt(y[0])[0];
        Complex q = Complex.Sqrt(y[1])[1];
        Complex r = -g * 0.125 / (p * q);
        Complex s = new Complex(0.25f * b / a);
        roots[0] = p + q + r - s;
        roots[1] = p - q - r - s;
        roots[2] = q - p - r - s;
        roots[3] = r - p - q - s;
        return roots;
    }

    /* ------------------------------ Geometry ------------------------------ */
    /// <summary>
    /// Return the minimum distance between a target point and other points.
    /// </summary>
    /// <param name="target"> The target point </param>
    /// <param name="points"> Other points </param>
    public static float MinDistance(Vector2 target, params Vector2[] points)
    {
        float result = -1;
        for (int i = 0; i < points.Length; i++)
        {
            float tmp = Vector2.Distance(target, points[i]);
            if (!float.IsNaN(tmp) && !float.IsInfinity(tmp) && tmp != float.Parse(null) && (result > tmp || result == -1))
            {
                result = tmp;
            }
        }
        return result;
    }
    public static Vector2 NearestPointStrict(Vector2 target, Vector2 start, Vector2 end)
    {
        Vector2 fullDirection = end - start;
        Vector2 lineDirection = fullDirection.normalized;
        float closestPoint = Vector2.Dot(target - start, lineDirection) / Vector2.Dot(lineDirection, lineDirection);
        return start + (Mathf.Clamp((int)closestPoint, 0, fullDirection.magnitude) * lineDirection);
    }
    /// <summary>
    /// Return the angle from 3D to 2D.
    /// <para> Example: Angle3D(new Vector3(60, 60, 60)) => 103.92 </para>
    /// </summary>
    /// <param name="angle"> The euler angle value of the object </param>
    public static float Angle3D(Vector3 angle)
    {
        for (int i = 0; i < 3; i++)
        {
            angle[i] = (360 + angle[i]) % 360;
            angle[i] = angle[i] > 180 ? 360 - angle[i] : angle[i];
        }
        return Vector3.Distance(angle, Vector3.zero);
    }
    /// <summary>
    /// Return the angle from 3D to 2D.
    /// <para> Example: Angle3D(new Vector3(60, 60, 60)) => 103.92 </para>
    /// </summary>
    /// <param name="angle"> The euler angle value of the object </param>
    public static float Angle3D(Vector3 angle, Vector3 target)
    {
        angle -= target;
        return Angle3D(angle);
    }
    /// <summary>
    /// Return the acute angle value of an angle.
    /// </summary>
    /// <param name="angle"> Original value </param>
    public static float AcuteAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        else if (angle < -180) angle += 360;
        return angle;
    }
    /// <summary>
    /// Return the acute angle value of an object.
    /// </summary>
    /// <param name="angle"> Original euler angle </param>
    public static Vector3 AcuteAngle(Vector3 angle)
    {
        for (int i = 0; i < 3; i++) angle[i] = AcuteAngle(angle[i]);
        return angle;
    }

    /* ------------------------------ Format Convert ------------------------------ */
    /// <summary>
    /// Convert the Render Texture to Texture2D.
    /// </summary>
    /// <param name="renderTexture"> The target render texture </param>
    public static Texture2D RenderTexTo2D(RenderTexture renderTexture)
    {
        if (renderTexture == null)
            return null;

        int width = renderTexture.width;
        int height = renderTexture.height;
        Texture2D tex2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderTexture;
        tex2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex2d.Apply();
        return tex2d;
    }

    /* ------------------------------ Raycast ------------------------------ */
    /// <summary>
    /// Return a raycast hit when hitting objects with a specific tag.
    /// <para> Example: Raycast(Vector3.zero, Vector3.down, 100, "Raycast") </para>
    /// </summary>
    /// <param name="origin"> Start point of raycast </param>
    /// <param name="direction"> Direction of raycast </param>
    /// <param name="length"> Length of ray </param>
    /// <param name="tag"> Object tag </param>
    public static RaycastHit Raycast(Vector3 origin, Vector3 direction, float length, string tag)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, length) && (tag == "" || tag == hit.collider.tag))
        {
            Debug.DrawLine(origin, hit.point, Color.red);
            return hit;
        }
        return new RaycastHit();
    }
    /// <summary>
    /// Return a raycast hit when hitting objects.
    /// <para> Example: Raycast(Vector3.zero, Vector3.down, 100) </para>
    /// </summary>
    /// <param name="origin"> Start point of raycast </param>
    /// <param name="direction"> Direction of raycast </param>
    /// <param name="length"> Length of ray </param>
    public static RaycastHit Raycast(Vector3 origin, Vector3 direction, float length)
    {
        return Raycast(origin, direction, length, "");
    }
    /// <summary>
    /// Return a raycast hit when hitting objects with a specific layer.
    /// <para> Example: Raycast(Vector3.zero, Vector3.down, 100, 8) </para>
    /// </summary>
    /// <param name="origin"> Start point of raycast </param>
    /// <param name="direction"> Direction of raycast </param>
    /// <param name="length"> Length of ray </param>
    /// <param name="layer"> Layer index </param>
    public static RaycastHit RaycastWithLayer(Vector3 origin, Vector3 direction, float length, int layer)
    {
        RaycastHit hit;
        int layerMask = 1 << layer;
        if (Physics.Raycast(origin, direction, out hit, length, layerMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red);
            return hit;
        }
        return new RaycastHit();
    }
    /// <summary>
    /// Return a raycast hit when hitting objects without a specific layer.
    /// <para> Example: Raycast(Vector3.zero, Vector3.down, 100, 8) </para>
    /// </summary>
    /// <param name="origin"> Start point of raycast </param>
    /// <param name="direction"> Direction of raycast </param>
    /// <param name="length"> Length of ray </param>
    /// <param name="layer"> Layer index </param>
    public static RaycastHit RaycastWithoutLayer(Vector3 origin, Vector3 direction, float length, int layer)
    {
        RaycastHit hit;
        int layerMask = 1 << layer;
        layerMask = ~layerMask;
        if (Physics.Raycast(origin, direction, out hit, length, layerMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red);
            return hit;
        }
        return new RaycastHit();
    }

    /* ------------------------------ Display ------------------------------ */
    /// <summary>
    /// Set the Unity Program to display on a selected monitor.
    /// <para> Example: SetMonitorIndex(0) </para>
    /// </summary>
    /// <param name="monitorIndex"> Index of Target Monitor </param>
    /// <param name="width"> Width of Program </param>
    /// <param name="height"> Height of Program </param>
    /// <param name="fullScreen"> Fullscreen or Window Mode </param>
    public static void SetMonitorIndex(int monitorIndex, bool fullScreen = true, int width = 0, int height = 0)
    {
        PlayerPrefs.SetInt("UnitySelectMonitor", monitorIndex);
        if (width <= 0 || height <= 0)
        {
            var display = Display.displays[Mathf.Min(monitorIndex, Display.displays.Length - 1)];
            width = display.systemWidth;
            height = display.systemHeight;
        }
        Screen.SetResolution(width, height, fullScreen);
    }
}