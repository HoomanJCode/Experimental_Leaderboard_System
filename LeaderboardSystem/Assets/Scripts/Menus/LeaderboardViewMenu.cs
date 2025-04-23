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
    private Coroutine RefreshLeaderboardTask;
    private readonly List<PlayerLeaderboardRecord> allRecords=new();
    protected override void Init()
    {
        RegisterPlayerBtn.onClick.AddListener(() =>
        {
            var registerMenuView = GetView<PlayerProfileMenu>();
            registerMenuView.CreateMode = true;
            registerMenuView.ChangeToThisView();
        });
        RefreshLeaderboardBtn.onClick.AddListener(() => {
            StartRefreshLeaderboard();
        });
    }
    public override void ChangeToThisView()
    {
        base.ChangeToThisView();
        StartRefreshLeaderboard();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        leaderboardRecordExample.gameObject.SetActive(false);
        StartRefreshLeaderboard();

    }
    public void ClearLeaderboard()
    {
        foreach (var item in allRecords) Destroy(item.gameObject);
    }
    public void StartRefreshLeaderboard()
    {
        if(RefreshLeaderboardTask!=null) StopCoroutine(RefreshLeaderboardTask);
        RefreshLeaderboardTask=StartCoroutine(RefreshLeaderboard());
    }
    private IEnumerator RefreshLeaderboard()
    {
        var getScoresTask= leaderboardJunc.Service.GetSortedScores(); 
        yield return new WaitUntil(()=> getScoresTask.IsCompleted);
        var scores = getScoresTask.Result;
        var trashRecordsCount = allRecords.Count- scores.Count;
        int recordIndex = 0;
        foreach (var item in scores)
        {
            var getPlayerTask= PlayersAuthenticationService.Instance.GetPlayerById(item.PlayerId);
            var getAvatarTask = PlayersAuthenticationService.Instance.GetPlayerAvatarById(item.PlayerId);
            yield return new WaitUntil(() => getPlayerTask.IsCompleted);
            var avatar = getAvatarTask.Result==null? null: getAvatarTask.Result;
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
                await leaderboardJunc.Service.DeleteScoreAsync(getPlayerTask.Result.Id);
                StartRefreshLeaderboard();
            };
            record.EditAction = () => {
                var editPageView = GetView<PlayerProfileMenu>();
                editPageView.Name = getPlayerTask.Result.Name;
                editPageView.Description = getPlayerTask.Result.Description;
                editPageView.Score = item.Score;
                editPageView.PlayerId = getPlayerTask.Result.Id;
                editPageView.CreateMode = false;
                editPageView.ChangeToThisView();
            };
            recordIndex++;
            yield return null;
        }
        //remove aditional Trash Records
        if (trashRecordsCount > 0)
            for (var i = 0; i < trashRecordsCount; i++)
            {
                var targetIndex = allRecords.Count - 1;
                Destroy(allRecords[targetIndex].gameObject);
                allRecords.RemoveAt(targetIndex);
                yield return null;
            }
    }
}
