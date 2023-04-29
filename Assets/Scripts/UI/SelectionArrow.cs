using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour {
    [SerializeField] private RectTransform[] Options;
    [SerializeField] private AudioClip ArrowMoveSound;
    [SerializeField] private AudioClip SelectSound;
    private RectTransform ArrowTransform;
    private int CurrentPosition;

    private void Awake() {
        ArrowTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) ChangePostion(-1);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) ChangePostion(1);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) Interact();
    }

    private void ChangePostion(int position) {
        CurrentPosition += position;

        //if (position != 0) SoundManager.Instance.PlaySound(ArrowMoveSound);

        if (CurrentPosition < 0) CurrentPosition = Options.Length - 1;
        else if (CurrentPosition > Options.Length - 1) CurrentPosition = 0;

        ArrowTransform.position = new Vector3(ArrowTransform.position.x, Options[CurrentPosition].position.y, 0);
    }

    private void Interact() { 
        //SoundManager.Instance.PlaySound(SelectSound);
        Options[CurrentPosition].GetComponent<Button>().onClick.Invoke();
    }
}