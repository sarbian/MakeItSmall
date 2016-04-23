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
using KSP.UI.Screens.Flight;
using UnityEngine;

namespace MakeItSmall
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MakeItSmall : MonoBehaviour
    {
        [Persistent]
        private float navBallScale = 1;

        [Persistent]
        private Vector2 navBallOffset = Vector2.zero;

        [Persistent]
        private float altimeterScale = 1;

        [Persistent]
        private float timeScale = 1;

        [Persistent]
        private float mapOptionsScale = 1;

        [Persistent]
        private float stagingScale = 1;

        [Persistent]
        private float crewScale = 1;

        private float uiScale;

        //private float activeNavBallScale    = 0;
        //private float activeAltimeterScale  = 0;
        //private float activeTimeScale       = 0;
        ////private float activeMapOptionsScale = 0;
        ////private float activeStagingScale    = 0;
        ////private float activedockingRotScale = 0;
        ////private float activedockingLinScale = 0;
        ////private float activeUiModScale = 0;
        //private float activeCrewScale       = 0;

        private bool initialSaved = false;

        private InitialState initialNavBallState = new InitialState();
        private InitialState initialAltimeterState = new InitialState();
        private InitialState initialTimeState = new InitialState();
        private InitialState initialMapOptionsState = new InitialState();
        private InitialState initialStagingState = new InitialState();
        private InitialState initialdockingRotState = new InitialState();
        private InitialState initialdockingLinState = new InitialState();
        private InitialState initialUiModState = new InitialState();
        private InitialState initialCrewState = new InitialState();

        private NavBallBurnVector navBallBurnVector;

        private class InitialState
        {
            public Vector3 scale;
            public UIPanelTransition.PanelPosition[] states;
        }

        private Rect windowPos = new Rect(80, 80, 220, 80);
        private bool showUI = false;

        public void Start()
        {
            if (File.Exists<MakeItSmall>("MakeItSmall.cfg"))
            {
                ConfigNode config = ConfigNode.Load(IOUtils.GetFilePathFor(this.GetType(), "MakeItSmall.cfg"));
                ConfigNode.LoadObjectFromConfig(this, config);
            }
            GameEvents.onLevelWasLoadedGUIReady.Add(OnLevelLoadedGUIReady);
            GameEvents.onGameStateSaved.Add(data => SaveConfig());
            GameEvents.onGameSceneLoadRequested.Add(scene => SaveConfig());
        }

        public void OnDestroy()
        {
            GameEvents.onGameStateSaved.Remove(data => SaveConfig());
            GameEvents.onGameSceneLoadRequested.Remove(scene => SaveConfig());
            GameEvents.onLevelWasLoadedGUIReady.Remove(OnLevelLoadedGUIReady);
        }

        private void SaveConfig()
        {
            ConfigNode node = new ConfigNode("MakeItSmall");
            ConfigNode.CreateConfigFromObject(this, node);
            node.Save(IOUtils.GetFilePathFor(this.GetType(), "MakeItSmall.cfg"));
        }

        private void OnLevelLoadedGUIReady(GameScenes data)
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT)
                return;

            uiScale = GameSettings.UI_SCALE;
            
            SaveState(FlightUIModeController.Instance.navBall, initialNavBallState);
            SaveState(FlightUIModeController.Instance.altimeterFrame, initialAltimeterState);
            SaveState(FlightUIModeController.Instance.timeFrame, initialTimeState);
            SaveState(FlightUIModeController.Instance.MapOptionsQuadrant, initialMapOptionsState);
            SaveState(FlightUIModeController.Instance.stagingQuadrant, initialStagingState);
            SaveState(FlightUIModeController.Instance.dockingRotQuadrant, initialdockingRotState);
            SaveState(FlightUIModeController.Instance.dockingLinQuadrant, initialdockingLinState);
            SaveState(FlightUIModeController.Instance.uiModeFrame, initialUiModState);
            SaveState(FlightUIModeController.Instance.crew, initialCrewState);

            //printRectInfo(FlightUIModeController.Instance.navBall, "navBall");
            //printRectInfo(FlightUIModeController.Instance.altimeterFrame,"altimeterFrame");
            //printRectInfo(FlightUIModeController.Instance.timeFrame,"timeFrame");
            //printRectInfo(FlightUIModeController.Instance.MapOptionsQuadrant,"MapOptionsQuadrant");
            //printRectInfo(FlightUIModeController.Instance.stagingQuadrant,"stagingQuadrant");
            //printRectInfo(FlightUIModeController.Instance.crew, "crew");

            navBallBurnVector = GameObject.FindObjectOfType<NavBallBurnVector>();
            
            // The node remaining dv cursor is moved in LateUpdate
            // So I need an new component that run after the stock one
            // to move it in the right position
            navBallBurnVector.deltaVGauge.gameObject.AddComponent<HelixGaugeMover>();

            initialSaved = true;
        }

        public void Update()
        {
            if (!initialSaved || HighLogic.LoadedScene != GameScenes.FLIGHT || !FlightUIModeController.Instance || navBallBurnVector.deltaVGauge == null)
                return;
            
            SetScale(FlightUIModeController.Instance.navBall, initialNavBallState, navBallScale, navBallOffset);
            SetScale(FlightUIModeController.Instance.altimeterFrame, initialAltimeterState, altimeterScale);
            SetScale(FlightUIModeController.Instance.timeFrame, initialTimeState, timeScale);
            SetScale(FlightUIModeController.Instance.MapOptionsQuadrant, initialMapOptionsState, mapOptionsScale);
            SetScale(FlightUIModeController.Instance.stagingQuadrant, initialStagingState, stagingScale);
            SetScale(FlightUIModeController.Instance.dockingRotQuadrant, initialdockingRotState, stagingScale);
            //SetScale(FlightUIModeController.Instance.dockingLinQuadrant, initialdockingLinState, stagingScale);
            SetScale(FlightUIModeController.Instance.uiModeFrame, initialUiModState, stagingScale);
            SetScale(FlightUIModeController.Instance.crew, initialCrewState, crewScale);

            if (uiScale != GameSettings.UI_SCALE)
            {
                UIMasterController.Instance.SetScale(uiScale);
                GameSettings.UI_SCALE = uiScale;
                GameSettings.SaveSettings();
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

        private void SetScale(UIPanelTransition panel, InitialState initialState, float scale, Vector2 offset = default(Vector2))
        {
            if (panel)
            {
                panel.panelTransform.localScale = initialState.scale * scale;

                for (int i = 0; i < panel.states.Length; i++)
                {
                    panel.states[i].position = initialState.states[i].position * scale + offset;
                }

                panel.panelTransform.anchoredPosition = panel.states[panel.StateIndex].position;
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

            ScaleUI("All"  , ref uiScale);
            ScaleUI("NavBall"  , ref navBallScale);
            ScaleUI("NavBall Offset", ref navBallOffset.x, 10f, "F0", 0);
            ScaleUI("Altimeter", ref altimeterScale);
            ScaleUI("Time"     , ref timeScale);
            ScaleUI("Map"      , ref mapOptionsScale);
            ScaleUI("Staging"  , ref stagingScale);
            ScaleUI("Crew"     , ref crewScale);

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        private void ScaleUI(string V, ref float scale, float step=0.05f, string format="F2", float def = 1f)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(V, GUILayout.MinWidth(60));

            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
            {
                scale -= step;
            }

            GUILayout.Label(scale.ToString(format), GUILayout.MinWidth(30));

            if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
            {
                scale += step;
            }

            if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
            {
                scale = def;
            }

            GUILayout.EndHorizontal();
        }

        public new static void print(object message)
        {
            MonoBehaviour.print("[MakeItSmall] " + message.ToString());
        }

    }
}
