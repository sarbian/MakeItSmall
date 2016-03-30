/*
The MIT License (MIT)

Copyright (c) 2016 Sarbian

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using KSP.IO;
using KSP.UI;
using UnityEngine;

namespace MakeItSmall
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MakeItSmall : MonoBehaviour
    {
        [Persistent]
        private float navBallScale = 1;

        [Persistent]
        private float altimeterScale = 1;

        [Persistent]
        private float timeScale = 1;

        //[Persistent]
        //private float mapOptionsScale = 1;

        [Persistent]
        private float stagingScale = 1;

        [Persistent]
        private float crewScale = 1;

        private float scale;

        private float activeNavBallScale    = 0;
        private float activeAltimeterScale  = 0;
        private float activeTimeScale       = 0;
        //private float activeMapOptionsScale = 0;
        //private float activeStagingScale    = 0;
        //private float activedockingRotScale = 0;
        //private float activedockingLinScale = 0;
        //private float activeUiModScale = 0;
        private float activeCrewScale       = 0;

        private bool initialSaved = false;

        private InitialState initialNavBallState = new InitialState();
        private InitialState initialAltimeterState = new InitialState();
        private InitialState initialTimeState = new InitialState();
        //private InitialState initialMapOptionsState = new InitialState();
        //private InitialState initialStagingState = new InitialState();
        //private InitialState initialdockingRotState = new InitialState();
        //private InitialState initialdockingLinState = new InitialState();
        //private InitialState initialUiModState = new InitialState();
        private InitialState initialCrewState = new InitialState();

        private class InitialState
        {
            public Vector3 scale;
            public UIPanelTransition.PanelPosition[] states;
        }

        private Rect windowPos = new Rect(80, 80, 220, 80);
        private bool showUI = false;

        public void Start()
        {
            //DontDestroyOnLoad(gameObject);

            if (File.Exists<MakeItSmall>("MakeItSmall.cfg"))
            {
                ConfigNode config = ConfigNode.Load(IOUtils.GetFilePathFor(this.GetType(), "MakeItSmall.cfg"));
                ConfigNode.LoadObjectFromConfig(this, config);
            }
            GameEvents.onGameStateSaved.Add(onGameStateSaved);
        }

        void onGameStateSaved(Game game)
        {
            ConfigNode node = new ConfigNode("MakeItSmall");
            ConfigNode.CreateConfigFromObject(this, node);
            node.Save(IOUtils.GetFilePathFor(this.GetType(), "MakeItSmall.cfg"));
        }

        public void Update()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT || !FlightUIModeController.Instance)
                return;


            if (!initialSaved)
            {

                scale = GameSettings.UI_SCALE;

                SaveState(FlightUIModeController.Instance.navBall, initialNavBallState);
                SaveState(FlightUIModeController.Instance.altimeterFrame, initialAltimeterState);
                SaveState(FlightUIModeController.Instance.timeFrame, initialTimeState);
                //SaveState(FlightUIModeController.Instance.MapOptionsQuadrant, initialMapOptionsState);
                //SaveState(FlightUIModeController.Instance.stagingQuadrant, initialStagingState);
                //SaveState(FlightUIModeController.Instance.dockingRotQuadrant, initialdockingRotState);
                //SaveState(FlightUIModeController.Instance.dockingLinQuadrant, initialdockingLinState);
                //SaveState(FlightUIModeController.Instance.uiModeFrame, initialUiModState);
                SaveState(FlightUIModeController.Instance.crew, initialCrewState);
                
                //printRectInfo(FlightUIModeController.Instance.navBall, "navBall");
                //printRectInfo(FlightUIModeController.Instance.altimeterFrame,"altimeterFrame");
                //printRectInfo(FlightUIModeController.Instance.timeFrame,"timeFrame");
                //printRectInfo(FlightUIModeController.Instance.MapOptionsQuadrant,"MapOptionsQuadrant");
                //printRectInfo(FlightUIModeController.Instance.stagingQuadrant,"stagingQuadrant");
                //printRectInfo(FlightUIModeController.Instance.crew, "crew");

                initialSaved = true;
            }

            SetScale(FlightUIModeController.Instance.navBall, initialNavBallState, navBallScale, ref activeNavBallScale);
            SetScale(FlightUIModeController.Instance.altimeterFrame, initialAltimeterState, altimeterScale, ref activeAltimeterScale);
            SetScale(FlightUIModeController.Instance.timeFrame, initialTimeState, timeScale, ref activeTimeScale);
            //SetScale(FlightUIModeController.Instance.MapOptionsQuadrant, initialMapOptionsState, mapOptionsScale, ref activeMapOptionsScale);
            //SetScale(FlightUIModeController.Instance.stagingQuadrant, initialStagingState, stagingScale, ref activeStagingScale);
            //SetScale(FlightUIModeController.Instance.dockingRotQuadrant, initialdockingRotState, stagingScale, ref activedockingRotScale);
            //SetScale(FlightUIModeController.Instance.dockingLinQuadrant, initialdockingLinState, stagingScale, ref activedockingLinScale);
            //SetScale(FlightUIModeController.Instance.uiModeFrame, initialUiModState, stagingScale, ref activeUiModScale);
            SetScale(FlightUIModeController.Instance.crew, initialCrewState, crewScale, ref activeCrewScale);

            if (scale != GameSettings.UI_SCALE)
            {
                UIMasterController.Instance.SetScale(scale);
                GameSettings.UI_SCALE = scale;
            }


            if (GameSettings.MODIFIER_KEY.GetKey() && Input.GetKeyDown(KeyCode.U))
            {
                showUI = !showUI;
            }
        }

        private void SaveState(UIPanelTransition panel, InitialState save)
        {
            save.scale = panel.panelTransform.localScale;
            save.states = new UIPanelTransition.PanelPosition[panel.states.Length];
            for (int i = 0; i < panel.states.Length; i++)
            {
                save.states[i] = new UIPanelTransition.PanelPosition();
                save.states[i].position = panel.states[i].position;
            }
        }

        private void SetScale(UIPanelTransition panel, InitialState initialState, float newScale, ref float activeScale)
        {
            if (activeScale != newScale && panel)
            {
                panel.panelTransform.localScale = initialState.scale * newScale;

                for (int i = 0; i < panel.states.Length; i++)
                {
                    panel.states[i].position = initialState.states[i].position * newScale;
                }

                //panel. = initialState.states[panel.StateIndex].position;
                panel.panelTransform.anchoredPosition = panel.states[panel.StateIndex].position;

                activeScale = newScale;
            }
        }

        private void printRectInfo(UIPanelTransition panel, string name)
        {
            print(name + " " + panel.panelTransform.pivot + " " + panel.panelTransform.anchorMin + " " + panel.panelTransform.anchorMax);
            for (int i = 0; i < panel.states.Length; i++)
            {
                print("   " + i + " " + panel.states[i].name + " " + panel.states[i].position);
            }
        }

        public void OnGUI()
        {
            if (showUI)
            {
                windowPos = GUILayout.Window(8785488, windowPos, WindowGUI, "MakeItSmall");
            }
        }

        public void WindowGUI(int windowID)
        {
            GUILayout.BeginVertical();

            ScaleUI("All"  , ref scale);
            ScaleUI("NavBall"  , ref navBallScale);
            ScaleUI("Altimeter", ref altimeterScale);
            ScaleUI("Time"     , ref timeScale);
            //ScaleUI("Map"      , ref mapOptionsScale);
            //ScaleUI("Staging"  , ref stagingScale);
            ScaleUI("Crew"     , ref crewScale);

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        private void ScaleUI(string V, ref float scale)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(V, GUILayout.MinWidth(60));

            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
            {
                scale -= 0.05f;
            }

            GUILayout.Label(scale.ToString("F2"), GUILayout.MinWidth(30));

            if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
            {
                scale += 0.05f;
            }

            if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
            {
                scale = 1;
            }

            GUILayout.EndHorizontal();
        }

        public new static void print(object message)
        {
            MonoBehaviour.print("[MakeItSmall] " + message.ToString());
        }

    }
}
