#define GAME_TIME

using LiveSplit.GrooveCity;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI.Components;
using LiveSplit.Web;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;

namespace LiveSplit.UI.Components
{
    class Component : IComponent
    {
        public ComponentSettings Settings { get; set; }

        public string ComponentName
        {
            get { return "NightSky Auto Splitter"; }
        }

        public float PaddingBottom { get { return 0; } }
        public float PaddingTop { get { return 0; } }
        public float PaddingLeft { get { return 0; } }
        public float PaddingRight { get { return 0; } }

        public bool Refresh { get; set; }

        public IDictionary<string, Action> ContextMenuControls { get; protected set; }

        public Process Game { get; set; }

        protected static readonly DeepPointer IsLoading = new DeepPointer("Nightsky.exe", 0x00211490, 0x498, 0x35);
        protected static readonly DeepPointer ChapterID = new DeepPointer("Nightsky.exe", 0x00211490, 0xa4, 0x1b0);

        public TimeSpan GameTime { get; set; }

        public bool WasLoading { get; set; }
        public int? OldChapterID { get; set; }

        protected TimerModel Model { get; set; }

        public Component()
        {
            Settings = new ComponentSettings();
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (Game == null || Game.HasExited)
            {
                Game = null;
                var process = Process.GetProcessesByName("NightSky").FirstOrDefault();
                if (process != null)
                {
                    Game = process;
                }
                WasLoading = false;
                OldChapterID = null;
            }

            if (Model == null)
            {
                Model = new TimerModel() { CurrentState = state };
                state.OnStart += state_OnStart;
            }

            if (Game != null)
            {
                bool isLoading;
                IsLoading.Deref<bool>(Game, out isLoading);

                int chapterID;
                ChapterID.Deref<int>(Game, out chapterID);

                if (OldChapterID != null)
                {
                    if (state.CurrentPhase == TimerPhase.NotRunning && OldChapterID == 20239 && chapterID == 0)
                    {
                        Model.Start();
                    }
                    else if (state.CurrentPhase == TimerPhase.Running)
                    {
                        if (!WasLoading && isLoading && chapterID != 0)
                        {
                            Model.Split();
                        }

                        //if (titleScreenShowing && !creditsPlaying)
                        //Model.Reset();
                    }
                }

#if GAME_TIME
                state.IsLoading = isLoading;
#endif

                OldChapterID = chapterID;
                WasLoading = isLoading;
            }
        }

        void state_OnStart(object sender, EventArgs e)
        {
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
        }

        public float VerticalHeight
        {
            get { return 0; }
        }

        public float MinimumWidth
        {
            get { return 0; }
        }

        public float HorizontalWidth
        {
            get { return 0; }
        }

        public float MinimumHeight
        {
            get { return 0; }
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return document.CreateElement("x");
        }

        public System.Windows.Forms.Control GetSettingsControl(UI.LayoutMode mode)
        {
            return null;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
        }

        public void RenameComparison(string oldName, string newName)
        {
        }
    }
}
