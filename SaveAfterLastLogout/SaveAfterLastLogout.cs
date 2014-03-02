﻿/* ###############################################################################
 * The MIT License (MIT)
 * 
 * Copyright (c) [2014] [Patrick Reif (Wingman223)]
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * ############################################################################### */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TShock and Terraria Plugin Libraries
using TShockAPI;

using Terraria;
using TerrariaApi;
using TerrariaApi.Server;

namespace SaveAfterLastLogout
{
    [ApiVersion(1, 15)]
    public class SaveAfterLastLogout : TerrariaPlugin
    {
        /// <summary>
        /// Plugin version
        /// </summary>
        public override Version Version
        {
            get { return new Version("1.0.0.0");  }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public override string Name
        {
            get { return "Save after last logout"; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public override string Author
        {
            get { return "Wingman223"; }
        }

        /// <summary>
        /// Description of the server
        /// </summary>
        public override string Description
        {
            get { return "Automatically executes the save command after the last user has logged out"; }
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="game">Game instance</param>
        public SaveAfterLastLogout(Main game)
            : base(game)
        {
            // Plugin should be loaded after tshock has been initialized
            Order = 0;
        }

        /// <summary>
        /// Automatically registers for the "ServerLeave" event. If a player
        /// leaves the server this event is called
        /// </summary>
        public override void Initialize()
        {
            ServerApi.Hooks.ServerLeave.Register(this, OnPlayerLeaveServer);
        }

        /// <summary>
        /// ServerLeave Event
        /// Automatically executes the "save" command when
        /// the last player has left the server.
        /// </summary>
        /// <param name="args">LeaveEvent Parameters. Has a "who" parameter with the remaining player count</param>
        private void OnPlayerLeaveServer(LeaveEventArgs args)
        {
            int playerCount = TShock.Utils.ActivePlayers();

            // is this the last player online?
            if (playerCount <= 1)
            {
                // execute save command
                TShock.Utils.SaveWorld();
            }
        }
    }
}