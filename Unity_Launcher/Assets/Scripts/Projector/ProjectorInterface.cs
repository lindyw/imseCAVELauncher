using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public interface IProjectorScript {
    string portName { get; }
    bool isProjectorWorking { get; }

    InputField GetInputPortName();
    InputField GetInputDelay();
    Button GetStatusConnection();
    Transform GetTransform();
    GameObject GetGameObject();
    MonoBehaviour GetMonoBehaviour();

    bool Init();
    void End();
    void PowerOnHandler();
    void PowerOffHandler();
    void ThreeDOnHandler();
    void ThreeDOffHandler();
	void ThreeDOnDualHeadHandler();
	void HighResCAVEInputHandler ();
	void LowResCAVEInputHandler ();
    void UpdateHandler();
    IEnumerator IE_PowerAnd3DOnHandler();
}