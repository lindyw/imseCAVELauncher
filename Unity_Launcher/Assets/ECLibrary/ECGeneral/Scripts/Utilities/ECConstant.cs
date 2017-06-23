using UnityEngine;
using System.Collections;

public abstract class Element : Object
{
    public int id;
    public Element()
    {

    }
}

public class ECConstant
{
    public const double NATURAL_E = 2.71828182845904523536;
    public const double GOLDEN_RATIO = 1.61803398874989484820;

    public const string DIGIT = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //36
    public const string LETTER = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";  //62
    public const string ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρστυφχψω"; //120

    public const string PASSWORD = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]_`abcdefghijklmnopqrstuvwxyz{|}~^"; //95 = ASCII + ^
    public const string B64 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`~"; //64
    public const string B95 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`~!@#$%^&*()-_=+[{]}\\|;:'\",<.>/? "; //95
    public const string B128 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρστυφχψω×÷≦≧≠≡±∞"; //128
    public const string B256 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρστυφχψω×÷≦≧≠≡±∞"
        + "～∩∪⊥∠∟⊿㏒㏑∫∮∵∴≒√＜＞＝℅＿＋－＄￥￠￡％€℃℉㏕㎜㎝㎞㏎㎡㎎㎏㏄°＃＆＊※§〃○●△▲◎☆★◇◆□■▽▼㊣♀♂⊕⊙，、。．；：？！（）｛｝〔〕【】《》〈〉「」『』˙ˉˊˇˋㄅㄆㄇㄈㄉㄊㄋㄌㄎㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙㄚㄛㄜㄝㄞㄟㄠㄡㄢㄣㄤㄥㄦㄧㄨㄩ"; //256


    public static Vector2 farPointV2 = new Vector2(-9999, -9999);
    public static Vector3 farPointV3 = new Vector3(-9999, -9999, -9999);
    public static Vector4 farPointV4 = new Vector4(-9999, -9999, -9999, -9999);
}
