using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey {
    public class PayLineHelper : MonoBehaviour
    {

        [SerializeField]
        private Image imagePanel;
        [SerializeField]
        private RectTransform Line;
        [SerializeField] private Image simpleBoxImage;

        public void Create(LineBehavior lb, SlotGroupBehavior[] sGB)
        {
            if (lb == null || lb.rayCasters.Length == 0 || sGB == null || sGB.Length == 0) return;

            int rows = 0;
            foreach (var item in sGB)
            {
                rows = Mathf.Max(item.RayCasters.Length, rows);
            }
            int cols = lb.rayCasters.Length;

            Color[] colors;
            for (int r = 0; r < rows; r++)
            {
                colors = new Color[cols];
                for (int c = 0; c < cols; c++)
                {
                    colors[c] = Color.white;
                    if(sGB[c].RayCasters.Length <= r)
                    {
                        colors[c] = Color.clear;
                    }
                    else if(lb.rayCasters[c] == sGB[c].RayCasters[r])
                    {
                        colors[c] = lb.lineInfoColor;
                    }
                }
                CreateRow(colors);
            }
        }

        private void CreateRow(Color [] colors)
        {
            RectTransform l = Instantiate(Line);
            l.localScale = transform.lossyScale;
            l.SetParent(transform);
            for (int i = 0; i <colors.Length; i++)
            {
                Image im = Instantiate(imagePanel);
                if (colors[i] == Color.white)
                    im.GetComponent<Image>().sprite = simpleBoxImage.sprite;

                im.GetComponent<RectTransform>().localScale = transform.lossyScale;
                im.GetComponent<RectTransform>().SetParent(l);
                im.color = colors[i];
            }
        }
    }
}