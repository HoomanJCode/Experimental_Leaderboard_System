using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLeaderboardRecord : MonoBehaviour
{
    
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text index;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Button SelectBtn;
    [SerializeField] private Button DeleteBtn;
    [SerializeField] private Button EditBtn;
    private bool selected;
    private static readonly List<PlayerLeaderboardRecord> AllRecords =new();

    public void SetPlayer(int index,string name,Sprite avatar)
    {
        playerName.text = name;
        this.index.text = index.ToString();
        this.avatar.sprite = avatar;
    }
    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }
    private void Awake()
    {
        AllRecords.Add(this);
        SelectBtn.onClick.AddListener(() =>
        {
            //deselect other buttons in this leaderboard
            var otherSelected = AllRecords.FindAll(x => x.selected && x.transform.parent == transform.parent);
            foreach (var item in otherSelected)
                item.SetSelectedMode(false);
            SetSelectedMode(true);
        });
        SetSelectedMode(false);
    }
    private void OnDestroy()
    {
        AllRecords.Remove(this);
    }
    private void SetSelectedMode(bool status)
    {
        selected = status;
        DeleteBtn.gameObject.SetActive(status);
        EditBtn.gameObject.SetActive(status);
        score.gameObject.SetActive(!status);
    }
}