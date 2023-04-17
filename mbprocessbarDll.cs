 
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using wyDay.Controls;
using System.Threading;


namespace MusicBeePlugin {

    public partial class Plugin {
        private Windows7ProgressBar win7Processbar;
        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();
        System.Threading.Timer threadTimer;
        Form MusicBeeApp;

        /*
        public static void Debug(String info) {
            FileInfo f = new FileInfo("MBdebug.txt");
            StreamWriter debugFileWriter;
            debugFileWriter = f.AppendText();
            debugFileWriter.WriteLine(info);
            debugFileWriter.Close();

        }
        */

        public PluginInfo Initialise(IntPtr apiInterfacePtr) {
            //Debug("Init " + this.ToString() + this.GetHashCode());
            mbApiInterface = (MusicBeeApiInterface)Marshal.PtrToStructure(apiInterfacePtr, typeof(MusicBeeApiInterface));
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "MBprocessbar";
            about.Description = "Adds playing progress in the taskbar ";
            about.Author = "zzh1989829@gmail.com with fixes by TReKiE.net";
            about.TargetApplication = "Windows7";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type = PluginType.General;
            about.VersionMajor = 1;  // your plugin version
            about.VersionMinor = 0;
            about.Revision = 5;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;

            about.ConfigurationPanelHeight = 0;   // not implemented yet: height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function
            PluginStartup();
            return about;
        }

        private void PluginStartup() {
            try {
                IntPtr wndHandle = GUIAPIs.GetMBHandle();
                MusicBeeApp = (Form)Form.FromHandle(wndHandle);
                MusicBeeApp.Invoke(new MethodInvoker(ExecutePluginStartup));
            }
            catch {
            }
            
        }

        private void ExecutePluginStartup() {            
            try {
                win7Processbar = new Windows7ProgressBar();
                this.win7Processbar.ContainerControl = MusicBeeApp;
                this.win7Processbar.ShowInTaskbar = true;            
                this.win7Processbar.MarqueeAnimationSpeed = 20;
                this.win7Processbar.State = (mbApiInterface.Player_GetPlayState() == PlayState.Paused) ? ProgressBarState.Pause : ProgressBarState.Normal;
                this.win7Processbar.Name = "windows7ProgressBar";
                threadTimer = new System.Threading.Timer(threadTimer_Tick, null, 1000, 1000);
            }
            catch {
            }
            
        }

        private void threadTimer_Tick(object state) {
            try {
                MusicBeeApp.Invoke(new MethodInvoker(ExecuteTick));
            }
            catch {
            }
        }

        private void ExecuteTick() {
            int tmp =  (int)(mbApiInterface.Player_GetPosition() * 100 / mbApiInterface.NowPlaying_GetDuration());
            this.win7Processbar.ShowInTaskbar = true;
            if (tmp==0)
            {
                this.win7Processbar.ShowInTaskbar = false;
            }
            this.win7Processbar.Value = tmp;   
            //Debug(System.DateTime.Now.ToString() + " BarSet" + win7Processbar.Value + " " + mbApiInterface.Player_GetPosition() + "/" + mbApiInterface.NowPlaying_GetDuration());
        }

        public bool Configure(IntPtr panelHandle) {
            // save any persistent settings in a sub-folder of this path
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
            return false;
            
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason) {
            //debugFileWriter.Close();
            if (threadTimer != null) {
                threadTimer.Dispose();
            }
            if (win7Processbar != null) {
                win7Processbar.Dispose();
            }
        }

        // uninstall this plugin - clean up any persisted files
        public void Uninstall() {
        }

        // receive event notifications from MusicBee
        // only required if about.ReceiveNotificationFlags = PlayerEvents
        public void ReceiveNotification(string sourceFileUrl, NotificationType type) {
            // perform some action depending on the notification type
            switch (type) {
                case NotificationType.PluginStartup:
                    // perform startup initialization
                    PluginStartup();
                    break;
                case NotificationType.VolumeLevelChanged:
                    MusicBeeApp.Invoke(new MethodInvoker(SetVolume));
                    break;
                case NotificationType.PlayStateChanged:
                    MusicBeeApp.Invoke(new MethodInvoker(SetState));
                    break;
                case NotificationType.TrackChanged:
                    MusicBeeApp.Invoke(new MethodInvoker(ResetPosition));
                    break;
            }
        }

        private void ResetPosition() {
            win7Processbar.Value = 0;
            win7Processbar.ShowInTaskbar = false;
        }

        private void SetVolume() {
            ProgressBarState tmp = win7Processbar.State;
            win7Processbar.Value = (int)(mbApiInterface.Player_GetVolume() * 100);
            win7Processbar.State = ProgressBarState.Error;
            //Thread.Sleep(100);
            win7Processbar.State = tmp;
        }

        private void SetState() {
            switch (mbApiInterface.Player_GetPlayState()) {
                case PlayState.Playing:
                    win7Processbar.State = ProgressBarState.Normal;
                    break;
                case PlayState.Paused:
                    win7Processbar.State = ProgressBarState.Pause;
                    break;
                case PlayState.Stopped:
                    win7Processbar.State = ProgressBarState.Normal;
                    break;
            }
        }   

        // return lyrics for the requested artist/title
        // only required if PluginType = LyricsRetrieval
        // return null if no lyrics are found
        public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred) {
            return null;
        }

        // return Base64 string representation of the artwork binary data
        // only required if PluginType = ArtworkRetrieval
        // return null if no artwork is found
        public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album) {
            //Return Convert.ToBase64String(artworkBinaryData)
            return null;
        }
    }
}