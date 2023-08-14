using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class MainMenuUI : MonoBehaviour
{
    Button btnStart;
    Button btnExit;
    Button btnEasy;
    Button btnNormal;
    Button btnHard;
    Button btnExtreme;
    VisualElement difficultyGroup;
    Camera cam;
    public static Difficulty selectedLevel;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        var UIDoc = GetComponent<UIDocument>();
        btnStart = UIDoc.rootVisualElement.Q<Button>("Play");
        btnExit = UIDoc.rootVisualElement.Q<Button>("Exit");
        btnEasy= UIDoc.rootVisualElement.Q<Button>("Easy");
        btnNormal= UIDoc.rootVisualElement.Q<Button>("Normal");
        btnHard= UIDoc.rootVisualElement.Q<Button>("Hard");
        btnExtreme = UIDoc.rootVisualElement.Q<Button>("Extreme");
        difficultyGroup = UIDoc.rootVisualElement.Q<VisualElement>("Difficulty");

        btnStart.clicked += BtnStart_clicked;
        btnExit.clicked += BtnExit_clicked;
        btnEasy.clicked += () => StartLevel(Difficulty.Easy);
        btnNormal.clicked += () => StartLevel(Difficulty.Normal);
        btnHard.clicked += () => StartLevel(Difficulty.Hard);
        btnExtreme.clicked += () => StartLevel(Difficulty.Extreme);
    }

    private void StartLevel(Difficulty difficulty)
    {
        selectedLevel = difficulty;
        SceneManager.LoadSceneAsync(1);
    }

    private void Update()
    {
        cam.transform.Rotate(0,20*Time.deltaTime,0,Space.World);
    }

    private void BtnExit_clicked()
    {
        Application.Quit();
    }

    private void BtnStart_clicked()
    {
        difficultyGroup.ToggleInClassList("hide");
    }
}

public enum Difficulty{
    Easy,
    Normal,
    Hard,
    Extreme
}
