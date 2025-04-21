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
    private LeaderboardJunction leaderboard;
    private readonly List<PlayerLeaderboardRecord> allRecords=new();
    protected override void Init()
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        leaderboardRecordExample.gameObject.SetActive(false);
        var addPlayerTask=PlayersAuthentication.RegisterPlayer("Player 1","Desc");
        addPlayerTask.Wait();
        leaderboard.Service.PushScoreAsync(new PlayerScore(addPlayerTask.Result.Id,Random.Range(1,1000))).Wait();
        StartCoroutine(CreateLeaderboard());
            
    }
    public void ClearLeaderboard()
    {
        foreach (var item in allRecords) Destroy(item.gameObject);
    }
    public IEnumerator CreateLeaderboard()
    {
        yield return leaderboard.Service.SortScoresAsync();
        int recordIndex = 0;
        foreach (var item in leaderboard.Service.Scores)
        {
            var getPlayerTask=PlayersAuthentication.GetPlayerById(item.PlayerId);
            var getAvatarTask = PlayersAuthentication.GetPlayerAvatarById(item.PlayerId);
            yield return new WaitUntil(() => getAvatarTask.IsCompleted);
            var avatar = BytesToSprite(getAvatarTask.Result.ProfileImage);
            yield return new WaitUntil(() => getPlayerTask.IsCompleted);
            if(allRecords.Count < recordIndex)
                allRecords[recordIndex].SetPlayer(recordIndex,getPlayerTask.Result.Name, avatar);
            else
            {
                var newRecordGobj = Instantiate(leaderboardRecordExample.gameObject);
                var newRecord = newRecordGobj.GetComponent<PlayerLeaderboardRecord>();
                newRecord.SetPlayer(recordIndex, getPlayerTask.Result.Name, avatar);
                allRecords.Add(newRecord);
            }
            recordIndex++;
        }
    }
    private IEnumerator AddRecord(Player player)
    {
        var getAvatarTask = PlayersAuthentication.GetPlayerAvatarById(player.Id);
        yield return new WaitUntil(() => getAvatarTask.IsCompleted);
        var avatar = BytesToSprite(getAvatarTask.Result.ProfileImage);
        var newRecordGobj = Instantiate(leaderboardRecordExample.gameObject);
        var newRecord = newRecordGobj.GetComponent<PlayerLeaderboardRecord>();
        newRecord.SetPlayer(0, player.Name, avatar);
        allRecords.Add(newRecord);
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
