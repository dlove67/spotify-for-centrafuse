using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using centrafuse.Plugins;
using System.Windows.Forms;
using SpotiFire.SpotifyLib;

namespace Spotify
{
    public class Setup : CFSetup
    {

        internal const string PASSWORD = "11965d46-aaf0-4f7d-aee1-1bd1dcd08a55";

        // The setup constructor will be called each time this plugin's setup is opened from the CF Setting Page
        // This setup is opened as a dialog from the CF_pluginShowSetup() call into the main plugin application form.
        public Setup(ICFMain mForm, ConfigReader config, LanguageReader lang)
        {
            // MainForm must be set before calling any Centrafuse API functions
            this.MainForm = mForm;

            // pluginConfig and pluginLang should be set before calling CF_initSetup() so this CFSetup instance 
            // will internally save any changed settings.
            this.pluginConfig = config;
            this.pluginLang = lang;

            // When CF_initSetup() is called, the CFPlugin layer will call back into CF_setupReadSettings() to read the page
            // Note that this.pluginConfig and this.pluginLang must be set before making this call
            CF_initSetup(1, 1);

            // Update the Settings page title
            this.CF_updateText("TITLE", this.pluginLang.ReadField("/APPLANG/SETUP/TITLE"));
        }

        public override void CF_setupReadSettings(int currentpage, bool advanced)
        {
            try
            {
                ButtonHandler[CFSetupButton.One] = new CFSetupHandler(SetUserName);
                ButtonText[CFSetupButton.One] = this.pluginLang.ReadField("/APPLANG/SETUP/USERNAME");
                ButtonValue[CFSetupButton.One] = this.pluginConfig.ReadField("/APPCONFIG/USERNAME");

                ButtonHandler[CFSetupButton.Two] = new CFSetupHandler(SetPassword);
                ButtonText[CFSetupButton.Two] = this.pluginLang.ReadField("/APPLANG/SETUP/PASSWORD");
                string encryptedPassword = this.pluginConfig.ReadField("/APPCONFIG/PASSWORD");
                ButtonValue[CFSetupButton.Two] = String.IsNullOrEmpty(encryptedPassword) ? String.Empty : new String('•', 8);

                ButtonHandler[CFSetupButton.Three] = new CFSetupHandler(SetLocation);
                ButtonText[CFSetupButton.Three] = this.pluginLang.ReadField("/APPLANG/SETUP/LOCATION");
                ButtonValue[CFSetupButton.Three] = GetLocation(); 

                ButtonHandler[CFSetupButton.Four] = new CFSetupHandler(SetBitrate);
                ButtonText[CFSetupButton.Four] = this.pluginLang.ReadField("/APPLANG/SETUP/BITRATE");
                ButtonValue[CFSetupButton.Four] = GetBitrateString();

               
            }
            catch (Exception errmsg) { CFTools.writeError(errmsg.Message, errmsg.StackTrace); }
        }

        private string GetLocation()
        {
            string location = this.pluginConfig.ReadField("/APPCONFIG/LOCATION");
            if (string.IsNullOrEmpty(location))
                location = Utility.GetDefaultLocationPath();
            return location;
        }

        private void SetUserName(ref object value)
        {
            try
            {
                object tempobject;
                string resultvalue, resulttext;

                // Display OSK for user to type display name
                DialogResult dialogResult = this.CF_systemDisplayDialog(CF_Dialogs.OSK, this.pluginLang.ReadField("/APPLANG/SETUP/USERNAME"), ButtonValue[(int)value], null, out resultvalue, out resulttext, out tempobject, null, true, true, true, true, false, false, 1);

                if (dialogResult == DialogResult.OK)
                {
                    // save user value, note this does not save to file yet, as this should only be done when user confirms settings
                    // being overwritten when they click the "Save" button.  Saving is done internally by the CFSetup instance if
                    // pluginConfig and pluginLang were properly set before callin CF_initSetup().
                    this.pluginConfig.WriteField("/APPCONFIG/USERNAME", resultvalue);

                    // Display new value on Settings Screen button
                    ButtonValue[(int)value] = resultvalue;
                }
            }
            catch (Exception errmsg) { CFTools.writeError(errmsg.Message, errmsg.StackTrace); }
        }

        private void SetPassword(ref object value)
        {
            try
            {
                object tempobject;
                string resultvalue, resulttext;

                // Display OSK for user to type password
                DialogResult dialogResult = this.CF_systemDisplayDialog(CF_Dialogs.OSK, this.pluginLang.ReadField("/APPLANG/SETUP/PASSWORD"), String.Empty, "PASSWORD", out resultvalue, out resulttext, out tempobject, null, true, true, true, true, false, false, 1);

                if (dialogResult == DialogResult.OK)
                {
                    // save user value, note this does not save to file yet, as this should only be done when user confirms settings
                    // being overwritten when they click the "Save" button.  Saving is done internally by the CFSetup instance if
                    // pluginConfig and pluginLang were properly set before callin CF_initSetup().
                    this.pluginConfig.WriteField("/APPCONFIG/PASSWORD", EncryptionHelper.EncryptString(resultvalue, PASSWORD));

                    // Display new value on Settings Screen button
                    ButtonValue[(int)value] = new String('•', 8);
                }
            }
            catch (Exception errmsg) { CFTools.writeError(errmsg.Message, errmsg.StackTrace); }
        }

        private void SetLocation(ref object value)
        {
            string location = GetLocation();

            CFDialogParams dialogParams = new CFDialogParams("Choose a folder for Spotify data", location);
            dialogParams.browseable = true;
            dialogParams.enablesubactions = true;
            dialogParams.showfiles = false;

            CFDialogResults results = new CFDialogResults();
            if (this.CF_displayDialog(CF_Dialogs.FileBrowser, dialogParams, results) == DialogResult.OK)
            {
                string newPath = results.resultvalue;
                this.pluginConfig.WriteField("/APPCONFIG/LOCATION", newPath);
                ButtonValue[(int)value] = newPath;
            }
                
        }

        private string GetBitrateString()
        {
            string bitrateString = this.pluginConfig.ReadField("/APPCONFIG/BITRATE");
            if (string.IsNullOrEmpty(bitrateString))
            {
                return sp_bitrate.BITRATE_160k.ToString();
            }
            else
            {
                try
                {
                    object bitrate = Enum.Parse(typeof(sp_bitrate), bitrateString);
                    return bitrate.ToString();
                }
                catch
                {
                    return sp_bitrate.BITRATE_160k.ToString();
                }
            }
        }

        private void SetBitrate(ref object value)
        {

            try
            {
                string currentBitrate = GetBitrateString();

                CFControls.CFListViewItem[] audioFormatItems = new CFControls.CFListViewItem[3];
                audioFormatItems[0] = new CFControls.CFListViewItem(sp_bitrate.BITRATE_96k.ToString(), sp_bitrate.BITRATE_96k.ToString(), false);
                audioFormatItems[1] = new CFControls.CFListViewItem(sp_bitrate.BITRATE_160k.ToString(), sp_bitrate.BITRATE_96k.ToString(), false);
                audioFormatItems[2] = new CFControls.CFListViewItem(sp_bitrate.BITRATE_320k.ToString(), sp_bitrate.BITRATE_96k.ToString(), false);

                object resultObject;
                string resultvalue, resulttext;

                DialogResult result = this.CF_systemDisplayDialog(CF_Dialogs.FileBrowser, this.pluginLang.ReadField("/APPLANG/SETUP/BITRATE"), null, currentBitrate, out resultvalue, out resulttext, out resultObject, audioFormatItems, false, true, false, false, false, false, 1);

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.pluginConfig.WriteField("/APPCONFIG/BITRATE", resultvalue);
                    ButtonValue[(int)value] = resultvalue;
                }

            }
            catch (Exception errmsg) { CFTools.writeError(errmsg.Message, errmsg.StackTrace); }
        }
    }
}
