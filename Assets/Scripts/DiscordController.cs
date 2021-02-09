using UnityEngine;
using Discord;

public class DiscordController : MonoBehaviour
{
    private Discord.Discord discord;

    private void Start()
    {
        discord = new Discord.Discord(808558266434715648, (ulong) CreateFlags.Default);

        UpdateActivity("Nothing to play", "Idling");
    }

    private void Update()
    {
        discord.RunCallbacks();
    }

    public void UpdateActivity(string title)
    {
        var activityManager = discord.GetActivityManager();
        activityManager.UpdateActivity(new Activity()
        {
            Details = title,
            Type = ActivityType.Listening,
            Assets = new ActivityAssets()
            {
                LargeImage = "logo_large",
                LargeText = "MUnity"
            },
            Instance = true
        }, res =>
        {
            if (res != Result.Ok)
                Debug.LogError(res);
        });
    }

    public void UpdateActivity(string title, string desc)
    {
        var activityManager = discord.GetActivityManager();
        activityManager.UpdateActivity(new Activity()
        {
            Details = title,
            State = desc,
            Type = ActivityType.Listening,
            Assets = new ActivityAssets()
            {
                LargeImage = "logo_large",
                LargeText = "MUnity"
            },
            Instance = true
        }, res =>
        {
            if (res != Result.Ok)
                Debug.LogError(res);
        });
    }

    public void UpdateActivity(Activity activity)
    {
        var activityManager = discord.GetActivityManager();
        activityManager.UpdateActivity(activity, res =>
        {
            if (res != Result.Ok)
                Debug.LogError(res);
        });
    }

    public void ClearActivity()
    {
        discord.GetActivityManager().ClearActivity(res =>
        {
            if (res != Result.Ok)
                Debug.LogError(res);
        });
    }
}