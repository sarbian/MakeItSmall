using UnityEngine;

namespace MakeItSmall
{
    class HelixGaugeMover : MonoBehaviour
    {
        private KSP.UI.Screens.HelixGauge deltaVGauge;

        public void Start()
        {
            deltaVGauge = this.GetComponent<KSP.UI.Screens.HelixGauge>();
        }

        public void LateUpdate()
        {
            if (deltaVGauge != null)
            {
                Transform t = deltaVGauge.transform;
                deltaVGauge.readoutStandoff = (GameSettings.UI_SCALE * 150f + 20f) * transform.lossyScale.x;
                deltaVGauge.readoutField.position = t.position
                    + Quaternion.AngleAxis(deltaVGauge.currentAngle, -t.forward) * t.up * deltaVGauge.readoutStandoff;
            }
        }

    }
}
