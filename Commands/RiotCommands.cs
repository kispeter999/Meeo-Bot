using Camille.Enums;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MingweiSamuel.Camille.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camille.RiotGames;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;

namespace MeeoBotDSharpPlus.Commands
{
    public class RiotCommands : BaseCommandModule
    {
        //RIOTAPI CREATION
        static string json = File.ReadAllText("Config.json");
        static ConfigJson CfgJson = JsonConvert.DeserializeObject<ConfigJson>(json);
        readonly RiotGamesApi riotApi = RiotGamesApi.NewInstance(CfgJson.RiotDevToken);

        [Command("top")]
        [Description("Displays xy's top 10 mastery lvl champs")]
        public async Task Top(CommandContext ctx, params string[] name)
        {
            await Top(ctx, string.Concat(name));
            Console.WriteLine(string.Concat(name));
        }
        public async Task Top(CommandContext ctx,  string name = "")
        {
            try
            {
                if (name == "")
                {
                    name = GetLinkedSummonerNameById(ctx.Member.Id);
                }

                StringBuilder sb = new StringBuilder("");
                Camille.RiotGames.SummonerV4.Summoner summoner = await riotApi.SummonerV4().GetBySummonerNameAsync(PlatformRoute.EUN1, name);
                await ctx.Channel.SendMessageAsync($"{summoner.Name}'s top Champions:");
                Console.WriteLine($"{summoner.Name}'s top Champions:");


                Camille.RiotGames.ChampionMasteryV4.ChampionMastery[] masteries = riotApi.ChampionMasteryV4().GetAllChampionMasteries(PlatformRoute.EUN1, summoner.Id);
                for (var i = 0; i < 10; i++)
                {
                    var mastery = masteries[i];
                    var champ = (MingweiSamuel.Camille.Enums.Champion)mastery.ChampionId;
                    sb.AppendFormat("{0}) {1,-16} {2,10:N0} ({3})\n", i + 1, champ.Name(), mastery.ChampionPoints, mastery.ChampionLevel);
                    //sb.Append(champ.Name() + " " + mastery.ChampionPoints + " (" + mastery.ChampionLevel + ")\n" );
                    Console.WriteLine("{0,3}) {1,-16} {2,10:N0} ({3})", i + 1, champ.Name(), mastery.ChampionPoints, mastery.ChampionLevel);
                }
                await ctx.Channel.SendMessageAsync(sb.ToString());
            }
            catch (NullReferenceException)
            {
                await ctx.Channel.SendMessageAsync("Summoner not found");
            }
            catch (InvalidOperationException)
            {
                await ctx.Channel.SendMessageAsync("You have not linked a SummonerName to your discord account yet. Use the '>link' command!");
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync("Error: \n" + e);
            }
        }



        [Command("clashteam")]
        [Description("Displays xy's clash team members in role order")]
        public async Task Clashteam(CommandContext ctx, params string[] name)
        {
            await Clashteam(ctx, string.Concat(name));
        }
        public async Task Clashteam(CommandContext ctx, string name = "")
        {
            try
            {
                if (name == "")
                {
                    name = GetLinkedSummonerNameById(ctx.Member.Id);
                }

                //Camille.RiotGames.SummonerV4.Summoner summoner = await riotApi.SummonerV4().GetBySummonerNameAsync(PlatformRoute.EUN1, name);
                Camille.RiotGames.ClashV1.Player[] players = await riotApi.ClashV1().GetPlayersBySummonerAsync(PlatformRoute.EUN1, riotApi.SummonerV4().GetBySummonerNameAsync(PlatformRoute.EUN1, name).Result.Id);
                var teamid1 = players.ElementAt(0).TeamId;
                Camille.RiotGames.ClashV1.Team team1 = await riotApi.ClashV1().GetTeamByIdAsync(PlatformRoute.EUN1, teamid1);
                players = team1.Players;
                //var tourneyid = team1.TournamentId;
                for (int i = 0; i < players.Length; i++)
                {
                    if (players.ElementAt(i).Position.Equals("TOP"))
                    {
                        var fixedname = riotApi.SummonerV4().GetBySummonerId(PlatformRoute.EUN1, players.ElementAt(i).SummonerId).Name;
                        fixedname = fixedname.Replace(' ', '+');
                        await ctx.Channel.SendMessageAsync("TOP: https://eune.op.gg/summoner/userName=" + fixedname);
                    }
                }
                for (int i = 0; i < players.Length; i++)
                {
                    if (players.ElementAt(i).Position.Equals("JUNGLE"))
                    {
                        var fixedname = riotApi.SummonerV4().GetBySummonerId(PlatformRoute.EUN1, players.ElementAt(i).SummonerId).Name;
                        fixedname = fixedname.Replace(' ', '+');
                        await ctx.Channel.SendMessageAsync("JUNGLE: https://eune.op.gg/summoner/userName=" + fixedname);
                    }
                }
                for (int i = 0; i < players.Length; i++)
                {
                    if (players.ElementAt(i).Position.Equals("MIDDLE"))
                    {
                        var fixedname = riotApi.SummonerV4().GetBySummonerId(PlatformRoute.EUN1, players.ElementAt(i).SummonerId).Name;
                        fixedname = fixedname.Replace(' ', '+');
                        await ctx.Channel.SendMessageAsync("MID: https://eune.op.gg/summoner/userName=" + fixedname);
                    }
                }
                for (int i = 0; i < players.Length; i++)
                {
                    if (players.ElementAt(i).Position.Equals("BOTTOM"))
                    {
                        var fixedname = riotApi.SummonerV4().GetBySummonerId(PlatformRoute.EUN1, players.ElementAt(i).SummonerId).Name;
                        fixedname = fixedname.Replace(' ', '+');
                        await ctx.Channel.SendMessageAsync("BOT: https://eune.op.gg/summoner/userName=" + fixedname);
                    }
                }
                for (int i = 0; i < players.Length; i++)
                {
                    if (players.ElementAt(i).Position.Equals("UTILITY"))
                    {
                        var fixedname = riotApi.SummonerV4().GetBySummonerId(PlatformRoute.EUN1, players.ElementAt(i).SummonerId).Name;
                        fixedname = fixedname.Replace(' ', '+');
                        await ctx.Channel.SendMessageAsync("SUPPORT: https://eune.op.gg/summoner/userName=" + fixedname);
                    }
                }
                for (int i = 0; i < players.Length; i++)
                {
                    if (players.ElementAt(i).Position.Equals("UNSELECTED") || players.ElementAt(i).Role.Equals("FILL"))
                    {
                        var fixedname = riotApi.SummonerV4().GetBySummonerId(PlatformRoute.EUN1, players.ElementAt(i).SummonerId).Name;
                        fixedname = fixedname.Replace(' ', '+');
                        await ctx.Channel.SendMessageAsync("FILL: https://eune.op.gg/summoner/userName=" + fixedname);
                    }
                }
            }
            catch (NullReferenceException)
            {
                await ctx.Channel.SendMessageAsync("Summoner not found");
            }
            catch (InvalidOperationException)
            {
                await ctx.Channel.SendMessageAsync("You have not linked a SummonerName to your discord account yet. Use the '>link' command!");
            }
            catch (Exception e)
            {

                await ctx.Channel.SendMessageAsync("Error: \n" + e);
            }
        }


        [Command("opgg")]
        [Description("Links xy's op.gg site")]
        public async Task Opgg(CommandContext ctx, params string[] name)
        {
            await Opgg(ctx, string.Concat(name));
        }
        public async Task Opgg(CommandContext ctx, string name = "")
        {
            try
            {
                if (name == "")
                {
                    name = GetLinkedSummonerNameById(ctx.Member.Id);
                }

                await ctx.Channel.SendMessageAsync("https://eune.op.gg/summoner/userName=" + name);
            }
            catch (NullReferenceException)
            {
                await ctx.Channel.SendMessageAsync("Summoner not found");
            }
            catch (InvalidOperationException)
            {
                await ctx.Channel.SendMessageAsync("You have not linked a SummonerName to your discord account yet. Use the '>link' command!");
            }
            catch (Exception e)
            {

                await ctx.Channel.SendMessageAsync("Error: \n" + e);
            }
        }


        [Command("link")]
        [Description("Links your discord account to a League of Legends SummonerName")]
        public async Task Link(CommandContext ctx, params string[] name)
        {
            await Link(ctx, string.Concat(name));
        }
        public async Task Link(CommandContext ctx, string SummonerName)
        {
            try
            {
                List<LinkObject> linkList = GetLinkList();
                LinkObject lo = new LinkObject();
                lo.DiscordId = ctx.Member.Id;
                lo.LeagueSummonerName = SummonerName;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };


                var desiredObject = linkList.Where(i => i.DiscordId == lo.DiscordId).FirstOrDefault();
                if (desiredObject != null)
                {
                    var ind = linkList.IndexOf(desiredObject);
                    linkList.RemoveAt(ind);
                    linkList.Insert(ind, lo);
                    string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<LinkObject>>(linkList, options);
                    File.WriteAllText("Utils/Links.json", jsonOutString);
                    await ctx.Channel.SendMessageAsync("Link name changed to " + lo.LeagueSummonerName);
                }
                else
                {
                    linkList.Add(lo);
                    string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<LinkObject>>(linkList, options);
                    File.WriteAllText("Utils/Links.json", jsonOutString);
                    await ctx.Channel.SendMessageAsync("You linked your discord account to " + lo.LeagueSummonerName);
                }

            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync("Error: \n" + e);
            }
        }


        [Command("forcelink")]
        [RequireRoles(RoleCheckMode.Any, "Főnök")]
        public async Task ForceLink(CommandContext ctx, ulong DiscordId, params string[] name)
        {
            await ForceLink(ctx, DiscordId, string.Concat(name));
        }
        public async Task ForceLink(CommandContext ctx, ulong DiscordId, string SummonerName)
        {
            try
            {
                List<LinkObject> linkList = GetLinkList();
                LinkObject lo = new LinkObject();
                lo.DiscordId = DiscordId;
                lo.LeagueSummonerName = SummonerName;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                var desiredObject = linkList.Where(i => i.DiscordId == lo.DiscordId).FirstOrDefault();
                if (desiredObject != null)
                {
                    var ind = linkList.IndexOf(desiredObject);
                    linkList.RemoveAt(ind);
                    linkList.Insert(ind, lo);
                    string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<LinkObject>>(linkList, options);
                    File.WriteAllText("Utils/Links.json", jsonOutString);
                    await ctx.Channel.SendMessageAsync("Forced " + ctx.Guild.GetMemberAsync(DiscordId).Result.DisplayName + " to " + lo.LeagueSummonerName);
                }
                else
                {
                    linkList.Add(lo);
                    string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<LinkObject>>(linkList, options);
                    File.WriteAllText("Utils/Links.json", jsonOutString);
                    await ctx.Channel.SendMessageAsync("Forced " + ctx.Guild.GetMemberAsync(DiscordId).Result.DisplayName + " to " + lo.LeagueSummonerName);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Command]
        [RequireRoles(RoleCheckMode.Any, "Főnök")]
        public async Task RemoveLink(CommandContext ctx, ulong DiscordId)
        {
            List<LinkObject> linkList = GetLinkList();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var desiredObject = linkList.Where(i => i.DiscordId == DiscordId).First();
            linkList.Remove(desiredObject);
            string jsonOutString = System.Text.Json.JsonSerializer.Serialize<List<LinkObject>>(linkList, options);
            File.WriteAllText("Utils/Links.json", jsonOutString);
            await ctx.Channel.SendMessageAsync("Deleted link");
        }


        private List<LinkObject> GetLinkList()
        {
            try
            {
                List<LinkObject> linkList = new List<LinkObject>();
                string jsonString = File.ReadAllText("Utils/Links.json");
                linkList = System.Text.Json.JsonSerializer.Deserialize<List<LinkObject>>(jsonString);
                return linkList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string GetLinkedSummonerNameById(ulong id)
        {
            try
            {
                List<LinkObject> linkList = GetLinkList();
                return linkList.Where(i => i.DiscordId == id).First().LeagueSummonerName;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("User have not linked a SummonerName to their discord account yet");
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    class LinkObject
    {
        public ulong DiscordId { get; set; }
        public string LeagueSummonerName { get; set; }
    }
}
