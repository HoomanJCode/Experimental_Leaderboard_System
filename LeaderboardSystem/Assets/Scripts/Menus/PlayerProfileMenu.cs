using MenuViews;
using Repositories.Models;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileMenu : MenuView
{
    [SerializeField]
    private LeaderboardJunction leaderboardJunc;
    [SerializeField]
    private TMP_InputField _name;
    [SerializeField]
    private TMP_InputField description;
    [SerializeField]
    private TMP_InputField score;
    [SerializeField]
    private Button SubmitBtn;
    [SerializeField]
    private Button BackBtn;
    [SerializeField]
    private Button DeletePlayerBtn;
    private bool _createMode;
    public bool CreateMode {
        get => _createMode;
        set
        {
            if (value)
            {
                PlayerId = 0;
                Name = "";
                Description = "";
                Score = 0;
            }
            DeletePlayerBtn.gameObject.SetActive(!value);
            _createMode = value;
        }
    }
    public int PlayerId { get; set; }
    public string Name { get => _name.text; set => _name.text = value; }
    public string Description { get => description.text; set => description.text = value; }
    public int Score { get => int.Parse(score.text); set => score.text = value.ToString(); }
    protected override void Init()
    {
        BackBtn.onClick.AddListener(() =>
        {
            GetView<LeaderboardViewMenu>().ChangeToThisView();
        });
        DeletePlayerBtn.onClick.AddListener(async () =>
        {
            await leaderboardJunc.Service.DeleteScoreAsync(PlayerId);
            await PlayersAuthentication.RemovePlayer(PlayerId);
            GetView<LeaderboardViewMenu>().ChangeToThisView();
        });
        SubmitBtn.onClick.AddListener(async () =>
        {
            if (CreateMode)
            {
                var player =await PlayersAuthentication.RegisterPlayer(Name, Description);
                await leaderboardJunc.Service.PushScoreAsync(new PlayerScore(player.Id, Score));
            }
            GetView<LeaderboardViewMenu>().ChangeToThisView();
        });
    }
}
