using System;
using UnityEngine;

public class Simulator : MonoBehaviour {
    #region Variables
    public ComputeShader computeShader;

    private RenderTexture renderTexture;
    private ComputeBuffer inputBuffer;
    private ComputeBuffer outputBuffer;

    private int[] inputData;
    private int[] outputData;

    private int kernelIndex;
    private int w;
    private int h;

    [Range(1, 25)]
    public int brushSize;
    #endregion

    void Awake() {
        #region Assign variables
        w = Screen.width / 8;
        h = Screen.height / 8;

        inputData = new int[w * h];
        outputData = new int[w * h];

        inputBuffer = new ComputeBuffer(w * h, sizeof(int));
        outputBuffer = new ComputeBuffer(w * h, sizeof(int));
        #endregion

        #region Assign texture
        renderTexture = new RenderTexture(w, h, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.wrapMode = TextureWrapMode.Repeat;
        renderTexture.useMipMap = false;
        renderTexture.Create();
        #endregion

        #region Set shader variables
        kernelIndex = computeShader.FindKernel("CellularAutomata");

        computeShader.SetBuffer(kernelIndex, "inputBuffer", inputBuffer);
        computeShader.SetBuffer(kernelIndex, "outputBuffer", outputBuffer);
        computeShader.SetTexture(kernelIndex, "textureBuffer", renderTexture);
        computeShader.SetInt("w", w);
        computeShader.SetInt("h", h);
        #endregion
    }

    private void Update() {
        #region Mouse draw
        if (Input.GetMouseButton(0)) {
            int x = Mathf.FloorToInt(Input.mousePosition.x * ((float) w / Screen.width));
            int y = Mathf.FloorToInt(Input.mousePosition.y * ((float) h / Screen.height));
            int brushRadius = brushSize / 2;

            for (int i = -brushRadius; i <= brushRadius; i++) {
                for (int j = -brushRadius; j <= brushRadius; j++) {
                    float distance = Mathf.Sqrt(i * i + j * j);
                    int index = x + i + ((y + j) * w);


                    if (distance <= brushRadius && index >= 0 && index < w * h) {
                        inputData[index] = 1;
                    }
                }
            }

            computeShader.SetBool("drawing", true);

            inputBuffer.SetData(inputData);
            computeShader.Dispatch(kernelIndex, w / 8, h / 8, 1);
            return;
        }
        #endregion

        #region Compute
        inputBuffer.SetData(inputData);

        computeShader.SetBool("drawing", false);
        computeShader.Dispatch(kernelIndex, w / 8, h / 8, 1);

        outputBuffer.GetData(outputData);

        int[] temp = inputData;
        inputData = outputData;
        outputData = temp;
        #endregion
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(renderTexture, destination);
    }

    private void OnDestroy() {
        inputBuffer.Release();
        outputBuffer.Release();
    }
}