using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class HelpTipsUI : MonoBehaviour
{
    Label lblToolTip;
    public bool placedFirstTower { get; set; }
    public bool destroyedFirstSpawner { get; set; }

    private IEnumerator Start()
    {
        var UIDoc = GetComponent<UIDocument>();
        lblToolTip = UIDoc.rootVisualElement.Q<Label>("Hints");
        changeText("Click on the map to place your Home Turf where you'd like, click the turf again to confirm");
        yield return new WaitForSeconds(2f);
        lblToolTip.ToggleInClassList("hide");
        placedFirstTower = false;
        destroyedFirstSpawner = false;
    }
    public void TextHomeTurfPlaced()
    {
        changeText("Your turf will make money for you, build defenses to support your turf.");
    }
    public void TextPlacedFirstTower()
    {
        if (!placedFirstTower) changeText("Well done don, Now shoot some enemy turfs ");
    }

    public void TextShootSpawners()
    {
        if (!destroyedFirstSpawner) changeText("Well done don, take some more down and claim this town!");
        StartCoroutine(DisableTips());
    }


    private void changeText(string txt)
    {
        lblToolTip.text = txt;
    }
    private IEnumerator DisableTips()
    {
        yield return new WaitForSeconds(2f);
        lblToolTip.ToggleInClassList("hide");
        yield return new WaitForSeconds(2f);
        lblToolTip.style.display = DisplayStyle.None;
    }

}
