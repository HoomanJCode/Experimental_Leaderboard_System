using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MenuViews;
using Repositories.Models;
using Services;
using UnityEngine;

public class LeaderboardViewMenu : MenuView
{
    [SerializeField]
    private PlayerLeaderboardRecord leaderboardRecordExample;
    [SerializeField] 
    private LeaderboardJunction leaderboardJunc;
    private readonly List<PlayerLeaderboardRecord> allRecords=new();
    protected override void Init()
    {
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
            if(allRecords.Count < recordIndex)
                allRecords[recordIndex].SetPlayer(recordIndex,getPlayerTask.Result.Name, avatar);
            else
            {
                var newRecordGobj = Instantiate(leaderboardRecordExample.gameObject);
                newRecordGobj.transform.SetParent(leaderboardRecordExample.transform.parent);
                var newRecord = newRecordGobj.GetComponent<PlayerLeaderboardRecord>();
                newRecord.SetPlayer(recordIndex, getPlayerTask.Result.Name, avatar);
                newRecord.SetScore(item.Score);
                allRecords.Add(newRecord);
                newRecordGobj.SetActive(true);
            }
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
