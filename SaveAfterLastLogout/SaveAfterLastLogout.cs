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
        public bool     canSave;
        /// <summary>
        /// The UTC time at which the server was saved
        /// </summary>
        public DateTime lastSaved;

        /// <summary>
        /// Plugin version
        /// </summary>
        public override Version Version
        {
            get { return new Version("1.0.0.2");  }
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

            // After initialization the server is able to save
            canSave         = true;
            lastSaved       = DateTime.MinValue;
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
            if (canSave == false)
            {
                if ( lastSaved != DateTime.MinValue)
                {
                    // is the time between lastSaved and now greather than 1 minute
                    if ((DateTime.Now - lastSaved).TotalMinutes >= 1)
                    {
                        canSave = true;
                    }
                }
                else
                {
                    // Timer has not been set
                    // Set it, to prevent a saving deadlock ( should not occur )
                    lastSaved = DateTime.Now;
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
            // Get the currrent player count
            int playerCount = TShock.Utils.ActivePlayers();

            // is this the last player and is able to save?
            if (playerCount <= 1 && canSave == true)
            {
                TShock.Utils.SaveWorld();

                lastSaved   = DateTime.Now;
                canSave     = false;
            }
        }
    }
}
