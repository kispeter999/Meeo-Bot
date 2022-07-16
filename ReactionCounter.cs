using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MeeoBotDSharpPlus
{
    public class ReactionCounter
    {
        public async Task ReactionsAddHandler(DiscordClient dc, MessageReactionAddEventArgs e)
        {
            try
            {
                await TrueReactionAddHandler(dc, e);
                await OkReactionAddHandler(dc, e);
                
            }
            catch (Exception ex)
            {
                await e.Channel.SendMessageAsync("Error: \n" + ex);
            }
            
        }
        public async Task MessagesAddHandler(DiscordClient dc, MessageCreateEventArgs e)
        {
            try
            {
                await OkMessageAddHandler(dc, e);
            }
            catch (Exception ex)
            {
                await e.Channel.SendMessageAsync("Error: \n " + ex);
            }
        }

        public async Task ReactionsRemoveHandler(DiscordClient dc, MessageReactionRemoveEventArgs e)
        {
            try
            {
                await TrueReactionRemoveHandler(dc, e);
            }
            catch (Exception ex)
            {
                await e.Channel.SendMessageAsync("Error: \n" + ex);
            }
        }
        public static List<ReactObject> GetReactList()
        {
            try
            {
                List<ReactObject> reactlist = new List<ReactObject>();
                string jsonString = File.ReadAllText("Utils/Reacts.json");
                reactlist = System.Text.Json.JsonSerializer.Deserialize<List<ReactObject>>(jsonString);
                return reactlist;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task TrueReactionAddHandler(DiscordClient dc, MessageReactionAddEventArgs e)
        {
            if (e.Emoji.Name.Equals("True"))
            {
                var currMsg = await e.Message.Channel.GetMessageAsync(e.Message.Id);
                if (currMsg.Author.Id == e.User.Id)
                {
                    return;
                }
                else
                {
                    List<ReactObject> reactlist = GetReactList();
                    ReactObject ro = new ReactObject();
                    ro.DiscordId = currMsg.Author.Id;

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    };

                    var desiredObject = reactlist.Where(i => i.DiscordId == ro.DiscordId).FirstOrDefault();
                    if (desiredObject != null)
                    {
                        ro.TrueCounter = reactlist.Where(i => i.DiscordId == ro.DiscordId).FirstOrDefault().TrueCounter;
                        ro.TrueCounter += 1;
                        if (ro.TrueCounter%100 == 0) {await e.Channel.SendMessageAsync("wow, " + currMsg.Author.Username + " spitting straight facts >>>" + ro.TrueCounter + " true reacts<<<");}
                        var ind = reactlist.IndexOf(desiredObject);
                        reactlist.RemoveAt(ind);
                        reactlist.Insert(ind, ro);
                        string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<ReactObject>>(reactlist, options);
                        File.WriteAllText("Utils/Reacts.json", jsonOutString);
                        //await e.Channel.SendMessageAsync("True reacts: " + ro.TrueCounter + " for user: " + dc.GetUserAsync(ro.DiscordId).Result.Username);
                    }
                    else
                    {
                        ro.TrueCounter = 1;
                        reactlist.Add(ro);
                        string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<ReactObject>>(reactlist, options);
                        File.WriteAllText("Utils/Reacts.json", jsonOutString);
                        //await e.Channel.SendMessageAsync("True react created " + ro.TrueCounter);
                    }
                }
            }
        }

        private async Task TrueReactionRemoveHandler(DiscordClient dc, MessageReactionRemoveEventArgs e)
        {
            if (e.Emoji.Name.Equals("True"))
            {
                var currMsg = await e.Message.Channel.GetMessageAsync(e.Message.Id);
                if (currMsg.Author.Id == e.User.Id)
                {
                    return;
                }
                else
                {
                    List<ReactObject> reactlist = GetReactList();
                    ReactObject ro = new ReactObject();
                    ro.DiscordId = currMsg.Author.Id;

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    };

                    var desiredObject = reactlist.Where(i => i.DiscordId == ro.DiscordId).FirstOrDefault();
                    if (desiredObject != null)
                    {
                        ro.TrueCounter = reactlist.Where(i => i.DiscordId == ro.DiscordId).FirstOrDefault().TrueCounter;
                        ro.TrueCounter -= 1;
                        var ind = reactlist.IndexOf(desiredObject);
                        reactlist.RemoveAt(ind);
                        reactlist.Insert(ind, ro);
                        string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<ReactObject>>(reactlist, options);
                        File.WriteAllText("Utils/Reacts.json", jsonOutString);
                        //await e.Channel.SendMessageAsync("True reacts: " + ro.TrueCounter + " for user: " + dc.GetUserAsync(ro.DiscordId).Result.Username);
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("Something is seriously wrong because there's no reaction to delete??????????????????");
                    }
                }
            }
        }

        private async Task OkReactionAddHandler(DiscordClient dc, MessageReactionAddEventArgs e)
        {
            if (e.Emoji == DSharpPlus.Entities.DiscordEmoji.FromName(dc, ":ok:"))
            {
                var currMsg = await e.Message.Channel.GetMessageAsync(e.Message.Id);
                await currMsg.DeleteReactionAsync(e.Emoji, e.User);
                
            }
        }
        private async Task OkMessageAddHandler(DiscordClient dc, MessageCreateEventArgs e)
        {
            if (e.Message.Content.Contains(DSharpPlus.Entities.DiscordEmoji.FromName(dc, ":ok:")))
            {
                var currMsg = await e.Message.Channel.GetMessageAsync(e.Message.Id);
                await e.Channel.DeleteMessageAsync(currMsg);
            }
        }
    }

    public class ReactObject
    {
        public ulong DiscordId { get; set; }
        public ulong TrueCounter { get; set; }
    }
}
