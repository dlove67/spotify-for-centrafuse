using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotiFire.SpotifyLib;
using System.Windows.Forms;

namespace Spotify
{
    public partial class Spotify
    {
        private void SubscribeSessionEvents(ISession session)
        {
            session.LoginComplete += new SessionEventHandler(session_LoginComplete);
            session.LogoutComplete += new SessionEventHandler(session_LogoutComplete);
            session.MessageToUser += new SessionEventHandler(session_MessageToUser);
            session.ConnectionError += new SessionEventHandler(session_ConnectionError);
            session.Exception += new SessionEventHandler(session_Exception);
            session.PlayTokenLost += new SessionEventHandler(session_PlayTokenLost);
        }

        void session_LogoutComplete(ISession sender, SessionEventArgs e)
        {
            loggedIn = false;
        }

        void session_PlayTokenLost(ISession sender, SessionEventArgs e)
        {
            CF_displayMessage("Play token lost! What do we do???" + Environment.NewLine + e.Status.ToString() + Environment.NewLine + e.Message);
        }

        void session_Exception(ISession sender, SessionEventArgs e)
        {
            WriteError(e.Message);
            CF_displayMessage(e.Status.ToString() + Environment.NewLine + e.Message);
        }

        void session_ConnectionError(ISession sender, SessionEventArgs e)
        {
            loggedIn = false; //is this necessary?
            CF_displayMessage("Connection Error: "+ Environment.NewLine + e.Status.ToString() + Environment.NewLine + e.Message);
        }

        void session_MessageToUser(ISession sender, SessionEventArgs e)
        {
            CF_displayMessage(e.Message);
        }

        bool loggedIn = false;
        void session_LoginComplete(ISession sender, SessionEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    if (e.Status != sp_error.OK)
                    {
                        CF_displayMessage("Login Failed: " + e.Message);
                    }
                    else
                    {
                        OnLoginComplete();
                    }
                }));
        }

        private void OnLoginComplete()
        {
            loggedIn = true;
            CF_systemCommand(centrafuse.Plugins.CF_Actions.HIDEINFO);
        }

        
    }
}
