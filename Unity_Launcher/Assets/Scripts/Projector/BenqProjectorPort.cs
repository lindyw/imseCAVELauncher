using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

public class BenqProjectorPort : ProjectorPort {

	public const int PROJECTOR_READ_DELAY_MS = 250;

    private const char PROJECTOR_CR_CHAR = '\r';
	private const char PROJECTOR_NL_CHAR = '\n';
	private static char[] PROJECTOR_TRIM_CHARS = new char[] {PROJECTOR_CR_CHAR, PROJECTOR_NL_CHAR, ' '};
    private static string[] PROJECTOR_SPLIT_DELIMITERS = new string[] { "?#", "=#", "=?", "?\r", "?\n" };
    private const string PROJECTOR_ATTR_PATTERN = @"=((?:(?!\?).)*)#";

    public bool isBusy = false;

    private string modelName = "";
	private Regex attrRegex = new Regex (PROJECTOR_ATTR_PATTERN);

    public BenqProjectorPort (string portName) : base(portName) {

	}

    protected override string FormatCommand(string cmd) {
        return "" + PROJECTOR_CR_CHAR + '*' + cmd + '#' + PROJECTOR_CR_CHAR;
    }

    private void BeforeGetAttr (string attr) {
        Debug.Log("get: " + attr);
        WriteCommand(attr + "=?");
    }

    private string AfterGetAttr () {
        string result = ReadCommand();
        Debug.Log("read: " + result);

        result = SubstringByAny(result, PROJECTOR_SPLIT_DELIMITERS);

        Match match = attrRegex.Match(result);
        if (match.Success) {
            result = match.Groups[match.Groups.Count - 1].Value;
        } else {
            result = result.Trim(PROJECTOR_TRIM_CHARS);
        }

        string lowerResult = result.ToLower();
        if (lowerResult.Contains("block item")) {
            result = "#Unkwn#";
        } else if (lowerResult.Contains("illegal format")) {
            result = "#CmdFail#";
        }
        return result;
    }

    private string SubstringByAny(string str, string[] delimiters) {
        foreach (string d in delimiters) {
            if (str.Contains(d)) {
                return str.Substring(str.IndexOf(d) + d.Length);
            }
        }
        return str;
    }

    private IEnumerator GetAttrAsync(string attr, Action<string> cb, int delayMs = PROJECTOR_READ_DELAY_MS) {
        BeforeGetAttr(attr);
        yield return new WaitForSeconds(delayMs / 1000f);
        cb(AfterGetAttr());
    }

    private string GetAttr(string attr, int delayMs = PROJECTOR_READ_DELAY_MS) {
        BeforeGetAttr(attr);
		Thread.Sleep (delayMs);
        return AfterGetAttr();
	}

    protected override bool TestIfPortSupported () {
        string read = "";
        try {
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
            _port.Write("" + ((char)13));

            while (true) {
                read += _port.ReadChar();
            }

        } catch (Exception e) {
            Debug.Log(e.Message);
        }

        return read != "";
    }

	public string GetPower (int delayMs = PROJECTOR_READ_DELAY_MS) {
		return GetAttr ("pow", delayMs);
	}

	public string GetSource () {
		return GetAttr ("sour");
	}

    public IEnumerator GetModelNameAsync (Action<string> cb) {
        if (modelName == "" || (modelName.StartsWith("#") && modelName.EndsWith("#")))
            yield return GetAttrAsync("modelname", (model) => {
                modelName = model;
                cb(modelName);
            }, 8000);
        else
            cb(modelName);
    }

	public string GetModelName () {
		if (modelName == "" || (modelName.StartsWith("#") && modelName.EndsWith("#")))
			modelName = GetAttr ("modelname", 8000);
		return modelName;
	}

	public string Get3DStatus () {
		return GetAttr ("3d");
	}

	public void PowerOn () {
		SendCommand ("pow=on");
	}

	public void PowerOff () {
		SendCommand ("pow=off");
	}

	public void _3DEnable () {
		SendCommand ("3d=fs");
	}

	public void _3DDisable () {
		SendCommand ("3d=off");
	}
}
