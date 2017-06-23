using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class FixInputSelectionMasking : MonoBehaviour {
	InputField _inputField;
	Transform _caret;

	public void Awake() {
		_inputField = GetComponent<InputField>();
	}

	public void Update() {
		if (!_caret) {
			_caret = _inputField.transform.Find(_inputField.transform.name + " Input Caret");
			if (_caret) {
				var graphic = _caret.GetComponent<Graphic>();
				if (!graphic)
					_caret.gameObject.AddComponent<Image>();
					_caret.gameObject.GetComponent<Image>().color = new Color (255,255,255,0);
			}
		}
	}
}