using System.Collections;
using System.Collections.Generic;
using MenuViews;
using Repositories.Models;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardViewMenu : MenuView
{
    [SerializeField]
    private PlayerLeaderboardRecord leaderboardRecordExample;
    [SerializeField] 
    private LeaderboardJunction leaderboardJunc;
    [SerializeField]
    private Button RegisterPlayerBtn;
    [SerializeField]
    private Button RefreshLeaderboardBtn;
    private readonly List<PlayerLeaderboardRecord> allRecords=new();
    protected override void Init()
    {
        RegisterPlayerBtn.onClick.AddListener(() =>
        {
            var registerMenuView = GetView<CreatePlayerMenu>();
            registerMenuView.CreateMode = true;
            registerMenuView.ChangeToThisView();
        });
        RefreshLeaderboardBtn.onClick.AddListener(() => {
            StartCoroutine(CreateLeaderboard());
        });
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        leaderboardRecordExample.gameObject.SetActive(false);
        StartCoroutine(CreateLeaderboard());
            
    }
    public void ClearLeaderboard()
    {
        foreach (var item in allRecords) Destroy(item.gameObject);
    }
    public IEnumerator CreateLeaderboard()
    {
        if(leaderboardJunc.Service.Scores.Count==0) 
        for (var i = 0; i < 20; i++)
        {
            var addPlayerTask = PlayersAuthentication.RegisterPlayer($"Player {i}", $"Desc{i}");
            yield return new WaitUntil(() => addPlayerTask.IsCompleted);
            yield return leaderboardJunc.Service.PushScoreAsync(new PlayerScore(addPlayerTask.Result.Id, Random.Range(1, 1000)));
            yield return leaderboardJunc.Service.SortScoresAsync();
        }


        int recordIndex = 0;
        foreach (var item in leaderboardJunc.Service.Scores)
        {
            var getPlayerTask=PlayersAuthentication.GetPlayerById(item.PlayerId);
            var getAvatarTask = PlayersAuthentication.GetPlayerAvatarById(item.PlayerId);
            yield return new WaitUntil(() => getPlayerTask.IsCompleted);
            var avatar = getAvatarTask.Result==null? null:BytesToSprite(getAvatarTask.Result.ProfileImage);
            yield return new WaitUntil(() => getAvatarTask.IsCompleted);
            PlayerLeaderboardRecord record;
            if (allRecords.Count > recordIndex)
                record = allRecords[recordIndex];
            else
            {
                var newRecordGobj = Instantiate(leaderboardRecordExample.gameObject);
                newRecordGobj.transform.SetParent(leaderboardRecordExample.transform.parent, false);
                record = newRecordGobj.GetComponent<PlayerLeaderboardRecord>();
                allRecords.Add(record);
                newRecordGobj.SetActive(true);
            }

            record.SetPlayer(recordIndex + 1, getPlayerTask.Result.Name, avatar);
            record.SetScore(item.Score);
            record.DeleteAction = async () => {
                await PlayersAuthentication.RemovePlayer(getPlayerTask.Result.Id);
                StartCoroutine(CreateLeaderboard());
            };
            record.EditAction = () => {
                var editPageView = GetView<CreatePlayerMenu>();
                editPageView.Name = getPlayerTask.Result.Name;
                editPageView.Description = getPlayerTask.Result.Description;
                editPageView.Score = item.Score;
                editPageView.PlayerId = getPlayerTask.Result.Id;
                editPageView.CreateMode = false;
                editPageView.ChangeToThisView();
            };
            recordIndex++;
        }
    }
    private static Sprite BytesToSprite(byte[] bytes)
    {
        // Create a new texture
        var texture = new Texture2D(2, 2);

        // Load the image data into the texture
        texture.LoadImage(bytes);

        // Create a sprite from the texture
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));

        return sprite;
    }
}
