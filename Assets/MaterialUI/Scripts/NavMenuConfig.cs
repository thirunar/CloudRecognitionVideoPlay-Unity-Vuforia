using UnityEngine;
using System;
using UnityEngine.UI;

namespace MaterialUI
{
    public class NavMenuConfig : MonoBehaviour
    {
        private Image thisImage;
        private RippleConfig thisRippleConfig;
        private Color activeColor, normalColor;

        void Start()
        {
            thisImage = gameObject.GetComponent<Image>();
            thisRippleConfig = gameObject.GetComponent<RippleConfig>();
            Color.TryParseHexString("#0A0A0AFF", out activeColor);
            Color.TryParseHexString("#212121FF", out normalColor);
        }

        public void UnsetActive()
        {
            thisImage.color = normalColor;
            thisRippleConfig.SetNormalColor(normalColor);
        }

        public void SetActive()
        {
            thisImage.color = activeColor;
            thisRippleConfig.SetNormalColor(activeColor);
        }
    }
}