using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Buttons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Skill correspondingSkill;
    public Item correspondingItem;
    public Buttons correspondingButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMenu()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().LoadMainMenu();
    }

    public void LoadBattle()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().LoadBattle();
    }
    public void LoadItemMenu()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().LoadItems();
    }

    public void LoadSkillMenu()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().LoadSkills();
    }

    public void ShowSkillsInBattle()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().ShowSkillsInBattle();
    }

    public void UseSkill()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SkillEffectHelper(correspondingSkill);
    }

    public void BackToBattleMenu()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().BackToBattleMenu();
    }

    public void LevelUpSkill()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().LevelUpSkill(correspondingButton.correspondingSkill);
    }

    public void SaveData()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SaveData();
    }

    public void LoadData()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().LoadData();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (correspondingItem != null)
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().DisplayText(correspondingItem.description);

        if(correspondingSkill != null)
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().DisplayText(correspondingSkill.ToString());
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (correspondingItem != null)
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().DisplayText("");

        if (correspondingSkill != null)
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().DisplayText("");
    }
}
