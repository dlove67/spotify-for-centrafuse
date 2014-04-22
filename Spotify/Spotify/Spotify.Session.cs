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
            loginComplete = false;
        }

        void session_PlayTokenLost(ISession sender, SessionEventArgs e)
        {
            this.ParentForm.BeginInvoke(new MethodInvoker(delegate()
                {
                    CF_displayMessage("Play token lost! What do we do???" + Environment.NewLine + e.Status.ToString() + Environment.NewLine + e.Message);
                }));
        }

        void session_Exception(ISession sender, SessionEventArgs e)
        {
            this.ParentForm.BeginInvoke(new MethodInvoker(delegate()
                {
                    WriteError(e.Message);
                    CF_displayMessage(e.Status.ToString() + Environment.NewLine + e.Message);
                }));
        }

        void session_ConnectionError(ISession sender, SessionEventArgs e)
        {
            
        }

        void session_MessageToUser(ISession sender, SessionEventArgs e)
        {
            this.ParentForm.BeginInvoke(new MethodInvoker(delegate()
                {
                    CF_displayMessage(e.Message);
                }));
        }

        bool loginComplete = false;
        void session_LoginComplete(ISession sender, SessionEventArgs e)
        {
            this.ParentForm.BeginInvoke(new MethodInvoker(() =>
                {
                    if (e.Status != sp_error.OK)
                    {
                        CF_displayMessage("Login Failed: " + e.Status + Environment.NewLine + e.Message);
                    }
                    else
                    {
                        OnLoginComplete();
                    }
                }));
        }

        bool firstLogin = true;
        private void OnLoginComplete()
        {
            this.ParentForm.BeginInvoke(new MethodInvoker(delegate()
                {
                    loginComplete = true;
                    CF_systemCommand(centrafuse.Plugins.CF_Actions.HIDEINFO);
                    if (firstLogin)
                    {
                        firstLogin = false;
                        RestoreNowPlaying();
                    }
                }));
        }
    }
}
