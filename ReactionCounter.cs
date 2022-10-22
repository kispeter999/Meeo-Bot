using DSharpPlus;
using DSharpPlus.EventArgs;
using Newtonsoft.Json.Linq;
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
            catch (Exception)
            {
                throw;
            }
        }

        private async Task TrueReactionAddHandler(DiscordClient dc, MessageReactionAddEventArgs e)
        {
            List<ulong> bannedChannels = new List<ulong>();
            bannedChannels.Add(653269190249152553);
            bannedChannels.Add(527253943839883275);
            bannedChannels.Add(348212333241171970);

            if (e.Emoji.Name.Equals("True"))
            {
                var currMsg = await e.Message.Channel.GetMessageAsync(e.Message.Id);
                if (bannedChannels.Contains(currMsg.Channel.Id))
                {
                    return;
                }
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

        public static void CalculateTrueReactions()
        {
            try
            {
                List<ReactObject> reactlist = GetReactList();
                List<string> messageFiles = Directory.GetFiles("Utils/dc_messages/").ToList();
                foreach (string file in messageFiles)
                {
                    string jsonString = File.ReadAllText(file);
                    DcMessageObject dcMessageObject = System.Text.Json.JsonSerializer.Deserialize<DcMessageObject>(jsonString);
                    foreach (Message msg in dcMessageObject.messages)
                    {
                        //Console.WriteLine("NEMTOM");
                        if (!(msg.reactions.Count == 0))
                        {
                            //Console.WriteLine("NEMNULL");
                            foreach (Reaction reac in msg.reactions)
                            {
                                if (reac.emoji.name.Equals("True"))
                                {
                                    Console.WriteLine("TRUEEE");
                                    ulong authorId = Convert.ToUInt64(msg.author.id);
                                    ulong reacCount = (ulong)reac.count;

                                    ReactObject ro = new ReactObject();
                                    ro.DiscordId = authorId;

                                    var options = new JsonSerializerOptions
                                    {
                                        WriteIndented = true,
                                    };

                                    var desiredObject = reactlist.Where(i => i.DiscordId == ro.DiscordId).FirstOrDefault();
                                    if (desiredObject != null)
                                    {
                                        ro.TrueCounter = reactlist.Where(i => i.DiscordId == ro.DiscordId).FirstOrDefault().TrueCounter;
                                        ro.TrueCounter += reacCount;
                                        var ind = reactlist.IndexOf(desiredObject);
                                        reactlist.RemoveAt(ind);
                                        reactlist.Insert(ind, ro);
                                        string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<ReactObject>>(reactlist, options);
                                        File.WriteAllText("Utils/Reacts.json", jsonOutString);
                                    }
                                    else
                                    {
                                        Console.WriteLine("lulge");
                                        ro.TrueCounter = 1;
                                        reactlist.Add(ro);
                                        string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<ReactObject>>(reactlist, options);
                                        File.WriteAllText("Utils/Reacts.json", jsonOutString);
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine("file '" + file + "' succesfully loaded");
                    //var messagesThatHaveTrueReact = dcMessageObjects.Where(o => o.messages.Where(msg => msg.reactions.Where(reac => reac.emoji.name.Equals("true"))));
                    //var messagesThatHaveTrueReact = from dcMsgObj in dcMessageObjects where dcMsgObj.messages.Contains(msg => msg.reactions)
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class ReactObject
    {
        public ulong DiscordId { get; set; }
        public ulong TrueCounter { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Attachment
    {
        public string id { get; set; }
        public string url { get; set; }
        public string fileName { get; set; }
        public int fileSizeBytes { get; set; }
    }

    public class Author
    {
        public string id { get; set; }
        public string name { get; set; }
        public string discriminator { get; set; }
        public string nickname { get; set; }
        public string color { get; set; }
        public bool isBot { get; set; }
        public string avatarUrl { get; set; }
    }

    public class Channel
    {
        public string id { get; set; }
        public string type { get; set; }
        public string categoryId { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public object topic { get; set; }
    }

    public class DateRange
    {
        public object after { get; set; }
        public object before { get; set; }
    }

    public class Emoji
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool isAnimated { get; set; }
        public string imageUrl { get; set; }
    }

    public class Guild
    {
        public string id { get; set; }
        public string name { get; set; }
        public string iconUrl { get; set; }
    }

    public class Mention
    {
        public string id { get; set; }
        public string name { get; set; }
        public string discriminator { get; set; }
        public string nickname { get; set; }
        public bool isBot { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string type { get; set; }
        public DateTime timestamp { get; set; }
        public object timestampEdited { get; set; }
        public object callEndedTimestamp { get; set; }
        public bool isPinned { get; set; }
        public string content { get; set; }
        public Author author { get; set; }
        public List<Attachment> attachments { get; set; }
        public List<object> embeds { get; set; }
        public List<object> stickers { get; set; }
        public List<Reaction> reactions { get; set; }
        public List<Mention> mentions { get; set; }
    }

    public class Reaction
    {
        public Emoji emoji { get; set; }
        public int count { get; set; }
    }

    //one channel of messages dumped by Discord Channel Message Dumper
    public class DcMessageObject
    {
        public Guild guild { get; set; }
        public Channel channel { get; set; }
        public DateRange dateRange { get; set; }
        public List<Message> messages { get; set; }
        public int messageCount { get; set; }
    }


}
