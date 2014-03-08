/* ###############################################################################
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
        /// Whether the server has been saved by the PlayerLeaveServer method
        /// </summary>
        public bool isSaved;
        /// <summary>
        /// The UTC time at which the server was saved
        /// </summary>
        public DateTime countDown;


        /// <summary>
        /// Plugin version
        /// </summary>
        public override Version Version
        {
            get { return new Version("1.0.0.1");  }
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
            Order = -1;
        }

        /// <summary>
        /// Automatically registers for the "ServerLeave" event. If a player
        /// leaves the server this event is called
        /// </summary>
        public override void Initialize()
        {
            ServerApi.Hooks.ServerLeave.Register(this, OnPlayerLeaveServer);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        /// <summary>
        /// Dispose hooks registered by the plugin
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerLeave.Deregister(this, OnPlayerLeaveServer);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Triggers every game update.
        /// </summary>
        /// <param name="args"></param>
        private void OnUpdate(EventArgs args)
        {
            DateTime now = DateTime.Now;

            /* 
             * Checks if the server has been saved in the last 1 minutes
             * If the server has been saved and the 1 minute interval elapses, then saving can be done again
             */
            if (isSaved)
            {
                if (countDown != DateTime.MinValue)
                {
                    if ((now - countDown).TotalMinutes >= 1)
                    {
                        isSaved = false;
                    }
                }
            }  
        }

        /// <summary>
        /// ServerLeave Event
        /// Automatically executes the "save" command when
        /// the last player has left the server.
        /// </summary>
        /// <param name="args">LeaveEvent Parameters. Has a "who" parameter which corresponds
        /// to the leaving player's index</param>
        private void OnPlayerLeaveServer(LeaveEventArgs args)
        {
            int playerCount = TShock.Utils.ActivePlayers();

            // is this the last player online, 
            // and has the server saved in the last 1 minutes due to the last player leaving?
            if (playerCount <= 1 && !isSaved)
            {
                // Server has been saved, so set isSaved to true
                isSaved = true;
                // Set countDown to this moment on the universal time clock
                countDown = DateTime.UtcNow;

                // execute save command
                TShock.Utils.SaveWorld();
            }
        }
    }
}
