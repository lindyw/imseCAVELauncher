using System.IO;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ProjectorConfig {

    public const string ConfigCommandPrefix = "cmd_";
    public const string ConfigCommandPowerOn = "cmd_power_on";
    public const string ConfigCommandPowerOff = "cmd_power_off";
    public const string ConfigCommand3DOn = "cmd_3d_on";
    public const string ConfigCommand3DOff = "cmd_3d_off";
	// Lindy Added
	public const string ConfigCommand3DOn_dualhead = "cmd_3d_on_dualhead";

	// Barco Highres CAVE 3D commands
	public const string ConfigCommand_DPort1 = "cmd_dport1";
	// Barco Lowres CAVE 3D commands
	public const string ConfigCommand_HDMI = "cmd_hdmi";

    public const string ConfigModelName = "model_name";
    public const string ConfigAttrBuadrate = "attr_baudrate";
    public const string ConfigAttrDataBits = "attr_data_length";
    public const string ConfigAttrStopBit = "attr_stop_bit";
    public const string ConfigAttrParityCheck = "attr_parity_check";

    public const string ConfigWaitPrefix = "wait_";
    public static char[] ConfigCommandSeparators = { ' ' };

    private readonly Dictionary<string, string> config;
    private readonly string configText;

    public readonly string configFilePath;

    public bool IsReady {
        get { return configText != "" && config != null && configFilePath != ""; }
    }

    public ProjectorConfig(string filePath) {

        if (filePath == "") return;

        try {
            configText = File.ReadAllText(filePath);
            config = ParseConfig(configText);
            configFilePath = filePath;
        } catch (FileNotFoundException e) {
            Debug.LogError(e.FileName + " is not found. Please check the path.");
        }

    }

    private static Dictionary<string, string> ParseConfig(string configText) {
        Dictionary<string, string> configDict = new Dictionary<string, string>();

        Debug.Log("parsing...\n" + configText);
        foreach (string line in configText.Split('\n')) {
            string _line = line.Trim();
            if (_line.StartsWith("#") || _line.Equals("")) continue; // skip for commented config

            if (!_line.Contains("=")) {
                // skip for line not look like an entry
                Debug.Log("Unknown line: " + _line);
                continue;
            }

            string[] entry = _line.Split('=');
            string entryKey = entry[0].Trim();
            string entryValue = entry[1].Trim();

            configDict[entryKey] = entryValue;
            Debug.Log(entryKey + " : " + entryValue);
        }

        return configDict;
    }

    internal string Get(string key, string defaultValue = null) {
        return config.ContainsKey(key) ? config[key] : defaultValue;
    }

    internal int GetInt(string key, int defaultValue) {
        return config.ContainsKey(key) ? int.Parse(config[key]) : defaultValue;
    }

    internal static float WaitStringToFloat(string s) {
        return float.Parse(s.Replace(ConfigWaitPrefix, ""));
    }

    internal static string FloatToWaitString(float f) {
        return ConfigWaitPrefix + f.ToString(CultureInfo.InvariantCulture);
    }

}