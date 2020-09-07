using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace KlassenBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // The classe that are available to pick
        private List<string> m_ClassesList = new List<string>()
        {
            "1gd",
            "1ga",
            "2gd",
            "2ga",
            "3gd",
            "3ga",
            "4gd",
            "4ga"
        };

        /// <summary>
        /// This will send you the instructions of how to use the bot
        /// </summary>
        [Command("help")]
        public async Task Help()
        {
            await ReplyAsync(
                "Type **!klas (jou klas)** om de correcte rol te krijgen.\n" +
                "Mocht je de verkeerde klas hebben gekozen, type dan opnieuw **!klas (verkeerde klas)**\n" +
                "Of type **!klassen** om alle klassen te kunnen bekijken.\n\n" +
                "Voorbeeld: **!klas 1GD**");
        }

        /// <summary>
        /// Set your class
        /// </summary>
        /// <param name="_class"> Your class </param>
        [Command("klas")]
        public async Task AssignClass(string _class)
        {
            // User that send the command
            var _user = Context.User;
            var _role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == _class.ToLower());

            // Try adding/removing the role from the user
            try
            {
                // Check if the role does exist
                if (m_ClassesList.Contains(_class.ToLower()))
                {
                    // If the user already has the role, remove it
                    if (_role.Members.Contains((IGuildUser)_user))
                    {
                        await ((IGuildUser)_user).RemoveRoleAsync(_role);
                        await ReplyAsync("Jij bent uit klas " + _class.ToUpper() + " gegaan!");
                    }
                    // If the user doesnt have the role, add it
                    else
                    {
                        await ((IGuildUser)_user).AddRoleAsync(_role);
                        await ReplyAsync("Jij zit nu in klas " + _class.ToUpper());

                    }
                }
                // If the role is invalid, let the user know
                else
                {
                    await ReplyAsync("Deze klas bestaat niet! Snap je het niet? Type **!help** voor informatie!");
                }
            }
            // If there does anything wrong, the developer will know throughout a private message
            catch (Exception _e)
            {
                await ReplyAsync("Oeps er ging iets helemaal verkeerd!");

                SocketUser _userID = Program.S_SOCKET_CLIENT.GetUser(368317619838779393);
                await UserExtensions.SendMessageAsync(_userID, _e.Message);
            }
        }

        /// <summary>
        /// Preview all the available classes
        /// </summary>
        /// <returns></returns>
        [Command("klassen")]
        public async Task Classes()
        {
            string _reply = "Je kan kiezen tussen de volgende klassen: ";

            // Loop though each class and add each class to the string
            int _currClassIndex = 0;
            foreach (string _class in m_ClassesList)
            {
                if (m_ClassesList.Count - 1 == _currClassIndex)
                    _reply += " en " + _class.ToUpper();
                else
                {
                    _reply += _class.ToUpper();

                    if (m_ClassesList.Count - 3 >= _currClassIndex)
                        _reply += ", ";
                }

                _currClassIndex++;
            }

            // Reply with each class added to the string
            await ReplyAsync(_reply);
        }
    }
}
