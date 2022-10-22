using Camille.Enums;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MeeoBotDSharpPlus.Commands
{
    public class Test : BaseCommandModule
    {
        [Command("ping")]
        [Description("returns pong!")]
        public async Task Ping(CommandContext ctx)
        {
            //await ctx.Channel.SendMessageAsync("Pong! (" + ctx.Client.Ping + " ms)").ConfigureAwait(false);
            await ctx.RespondAsync("Pong! (" + ctx.Client.Ping + " ms)");
        }


        [Command("prune")]
        [Description("deletes x messages in channel")]
        [RequireRoles (RoleCheckMode.Any, "Főnök")]
        public async Task Prune(CommandContext ctx, [Description("number of messages to delete")] int x)
        {
            var messages = ctx.Channel.GetMessagesAsync(x+1);
            await ctx.Channel.DeleteMessagesAsync(messages.Result).ConfigureAwait(false);
        }

        [Command("response")]
        public async Task Response(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }

        [Command("echo")]
        [Description("echoes a string back")]
        [RequireOwner]
        public async Task Echo(CommandContext ctx, [Description("the string you want to echo")] string x)
        {
            var message = await ctx.Channel.GetMessageAsync(ctx.Message.Id).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(x);
            await ctx.Channel.DeleteMessageAsync(message);
        }

        [Command("trues")]
        [Description("returns how many trues a user has")]
        public async Task Trues(CommandContext ctx, [Description("mention someone to get their true count")] string smth = null)
        {
            ulong id;
            var mentions = ctx.Message.MentionedUsers;
            if (mentions.Count == 0)
            {
                id = ctx.Member.Id;
            }
            else
            {
                //await ctx.Channel.SendMessageAsync(mentions[0].Id + "");
                id = mentions[0].Id;
            }

            List<ReactObject> reactlist = ReactionCounter.GetReactList();
            var desiredObject = reactlist.Where(i => i.DiscordId == id).FirstOrDefault();
            if (desiredObject != null)
            {
                await ctx.Channel.SendMessageAsync("True reacts: " + desiredObject.TrueCounter);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("You don't have any true reacts :(");
            }
        }

        [Command("calculatetrues")]
        [Description("recalculates truecounts")]
        [RequireOwner]
        public async Task CalculateTrues(CommandContext ctx)
        {
            ReactionCounter.CalculateTrueReactions();
            await ctx.Channel.SendMessageAsync("Calculating true reactions");
        }

        [Command("basedlist")]
        [Description("Displays the list of the most based people on the server")]
        [RequireOwner]
        public async Task BasedList(CommandContext ctx)
        {
            try
            {
                List<ReactObject> reactlist = ReactionCounter.GetReactList();

                StringBuilder sb = new StringBuilder("");
                sb.Append("Based people:\n");

                reactlist = reactlist.OrderByDescending(reac => reac.TrueCounter).ToList();
                for (var i = 0; i < 10; i++)
                {
                    var reaction = reactlist[i];
                    sb.AppendFormat("{0}: {1}\n", ctx.Client.GetUserAsync(reaction.DiscordId).Result.Username, reaction.TrueCounter);
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
    }


}
